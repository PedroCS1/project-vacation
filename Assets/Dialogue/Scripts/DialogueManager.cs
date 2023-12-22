using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject DialogueMenu;
    [SerializeField] private GameObject DialogueTextParent;
    [SerializeField] private GameObject DialogueChoicesParent;
    [SerializeField] private DialogueChoiceButton[] ChoiceButtons;
    [Header("TESTING")]
    [SerializeField] private Dialogue TEST_DIALOGUE;

    private Dialogue _currentDialogue;
    private int _currentDialogueEntryIndex;
    private DialogueUIController _UIController;
    private DialogueAudioPlayer _dialogueAudioPlayer;
    private DialogueEventSystem _dialogueEventSystem;
    private Coroutine _audioPlayingCoroutine = null;
    private enum ButtonConfiguration { Choices, Continue, AudioOnly }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _UIController = GetComponent<DialogueUIController>();
        _UIController.Initialize(ChoiceButtons, GetCurrentDialogueChoices);

        _dialogueAudioPlayer = GetComponent<DialogueAudioPlayer>();
        _dialogueAudioPlayer.Initialize();

        _dialogueEventSystem = GetComponent<DialogueEventSystem>();
        _dialogueEventSystem.Initialize();

        foreach (var choice in ChoiceButtons)
        {
            choice.Initialize();
        }
    }

    private DialogueChoice[] GetCurrentDialogueChoices()
    {
        return _currentDialogue.Choices.ToArray();
    }

    public void OnShowDialogueClick()
    {
        ShowDialogue(TEST_DIALOGUE);
    }

    private void ShowDialogue(Dialogue dialogue)
    {
        DialogueMenu.SetActive(true);
        _currentDialogue = dialogue;
        _currentDialogueEntryIndex = 0;

        UpdateMenuInfo(dialogue);
        PlayDialogueAudio();
        UpdateChoiceButtons(dialogue);
    }

    private void UpdateMenuInfo(Dialogue dialogue)
    {
        var current = dialogue.DialogueEntries[_currentDialogueEntryIndex];
        _UIController.SetSpeakerNameText(current.Speaker);
        UpdateDialogueText();
    }

    private void UpdateDialogueText()
    {
        if (_currentDialogueEntryIndex < _currentDialogue.DialogueEntries.Count)
        {
            var entry = _currentDialogue.DialogueEntries[_currentDialogueEntryIndex];
            _UIController.SetDialogueText(entry.DialogueText);
        }

    }

    private void UpdateChoiceButtons(Dialogue dialogue)
    {
        ClearListeners();
        ButtonConfiguration configuration = DetermineButtonConfiguration(dialogue);

        switch (configuration)
        {
            case ButtonConfiguration.Choices:
                ConfigChoiceButtons(dialogue);
                break;
            case ButtonConfiguration.Continue:
                StartCoroutine(SetContinueDialogueChoice());
                break;
            case ButtonConfiguration.AudioOnly:
                StartCoroutine(SetAudioOnlyDialogue());
                break;
        }
    }

    private IEnumerator SetAudioOnlyDialogue()
    {
        foreach (var button in ChoiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        yield return WaitAudioToFinish();

        Dialogue nextDialogue = null;
        for (int i = 0; i < _currentDialogue.Choices.Count; i++)
        {
            DialogueChoice choice = _currentDialogue.Choices[i];
            if (choice.NextDialogue != null)
            {
                nextDialogue = choice.NextDialogue;
                int index = i;
                OnChoiceClick(index);
                break;
            }
        }
        if (nextDialogue == null)
        {
            CloseDialogue();
        }
    }


    private IEnumerator WaitAudioToFinish()
    {
        if (_audioPlayingCoroutine != null)
        {
            yield return _audioPlayingCoroutine;
            _audioPlayingCoroutine = null;
        }        
    }

    private IEnumerator SetContinueDialogueChoice()
    {
        foreach (var button in ChoiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        /*
        ** Old Setup to show a 'Continue' button **

        if (ChoiceButtons.Length > 0)
        {
            ChoiceButtons[0].gameObject.SetActive(true);
            ChoiceButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            ChoiceButtons[0].onClick.AddListener(ContinueDialogue);
        }
        */
        yield return WaitAudioToFinish();

        ContinueDialogue();
    }
    public void ContinueDialogue()
    {
        if (_currentDialogueEntryIndex < _currentDialogue.DialogueEntries.Count - 1)
        {
            _currentDialogueEntryIndex++;
            UpdateDialogueText();
            PlayDialogueAudio();
            UpdateChoiceButtons(_currentDialogue);
        }
    }
    private void PlayDialogueAudio()
    {
        if (_currentDialogueEntryIndex < _currentDialogue.DialogueEntries.Count)
        {
            var entry = _currentDialogue.DialogueEntries[_currentDialogueEntryIndex];
            _audioPlayingCoroutine = StartCoroutine(_dialogueAudioPlayer.PlayDialogueAudio(entry.DialogueAudio));
        }
    }
    private void ClearListeners()
    {
        foreach (var button in ChoiceButtons)
        {
            button.RemoveListeners();
        }
    }

    private ButtonConfiguration DetermineButtonConfiguration(Dialogue dialogue)
    {
        if (_currentDialogueEntryIndex < dialogue.DialogueEntries.Count - 1)
        {
            return ButtonConfiguration.Continue;
        }
        else if (dialogue.IsAudioOnly)
        {
            return ButtonConfiguration.AudioOnly;
        }
        else if (dialogue.Choices.Count > 0)
        {
            return ButtonConfiguration.Choices;
        }
        else
        {
            return ButtonConfiguration.AudioOnly;
        }
    }


    private void ConfigChoiceButtons(Dialogue dialogue)
    {
        var current = dialogue.DialogueEntries[_currentDialogueEntryIndex];
        for (int i = 0; i < ChoiceButtons.Length; i++)
        {
            int index = i;
            ChoiceButtons[i].gameObject.SetActive(i < dialogue.Choices.Count);

            if (index < dialogue.Choices.Count)
            {
                ChoiceButtons[i].SetText(dialogue.Choices[i].ChoiceText);

                ChoiceButtons[index].AddListener(OnChoiceClick, index);
            }
        }
    }

    public void OnChoiceClick(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= _currentDialogue.Choices.Count) return;

        DialogueChoice choice = _currentDialogue.Choices[choiceIndex];
        Dialogue nextDialogue = choice.NextDialogue;
        AudioClip choiceAudio = choice.ChoiceAudio;

        StartCoroutine(UpdateDialogue(nextDialogue, choiceAudio));
    }

    private IEnumerator UpdateDialogue(Dialogue nextDialogue, AudioClip audio)
    {
        EnableButtons(false);

        yield return _dialogueAudioPlayer.PlayChoiceAudio(audio);

        ShowNextDialogue(nextDialogue);
        EnableButtons(true);
    }

    private void ShowNextDialogue(Dialogue nextDialogue)
    {
        if (nextDialogue != null)
        {
            ShowDialogue(nextDialogue);
            InvokeDialogueEvent(nextDialogue);
        }
        else
        {
            CloseDialogue();
        }
    }

    private void InvokeDialogueEvent(Dialogue dialogue)
    {
        _dialogueEventSystem.InvokeEvent(dialogue.ConsequenceID);
    }

    public void CloseDialogue()
    {
        _UIController.SetDialogueText("");
        _UIController.SetSpeakerNameText("");

        DialogueMenu.SetActive(false);
        _dialogueAudioPlayer.StopDialogueAudio();
    }

    private void EnableButtons(bool enabled)
    {
        foreach (var button in ChoiceButtons)
        {
            button.SetInteractable(enabled);
        }
    }

}