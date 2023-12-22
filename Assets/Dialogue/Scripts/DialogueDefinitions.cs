using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/NPC Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Tooltip("Will this dialogue have no choices, only play audio and move automatically?")]
    public bool IsAudioOnly;
    [Tooltip("What event this dialogue will trigger?")]
    public DialogueActionID ConsequenceID;
    public List<DialogueEntry> DialogueEntries;
    public List<DialogueChoice> Choices;
}

[System.Serializable]
public class DialogueEntry
{
    public string Speaker;
    [TextArea(3, 3)]
    public string DialogueText;
    public AudioClip DialogueAudio;
}

[System.Serializable]
public class DialogueChoice
{
    public string ChoiceText;
    public Dialogue NextDialogue;
    public AudioClip ChoiceAudio;
}

[System.Serializable]
public class DialogueEvent
{
    public DialogueActionID ActionID;
    public UnityEvent Action;
}

[System.Serializable]
public enum DialogueActionID
{
    None,
    OnCorrectChoice,
    OnWrongChoice
}
