using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI DialogueText;
    [SerializeField] private TextMeshProUGUI SpeakerNameText;
    [SerializeField] private Image DialogueBackground;
    [SerializeField] private Image SpeakerBackground;

    public enum DialogueType { Player, NPC }
    public enum ChoiceState { Highlighted, Default }
    private DialogueChoiceButton[] ChoiceButtons;
    private Func<DialogueChoice[]> GetCurrentChoices = null;

    private const int HighlightNone = -1;
    public void Initialize(DialogueChoiceButton[] ChoiceButtons, Func<DialogueChoice[]> GetCurrentChoices)
    {
        this.ChoiceButtons = ChoiceButtons;
        this.GetCurrentChoices = GetCurrentChoices;
    }

    public void SetSpeakerNameText(string speakerName)
    {
        SpeakerNameText.text = speakerName;
    }

    public void SetDialogueText(string text)
    {
        DialogueText.text = text;
    }

}
