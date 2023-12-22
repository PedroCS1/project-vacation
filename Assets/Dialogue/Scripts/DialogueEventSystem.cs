using System;
using System.Collections;
using UnityEngine;

public class DialogueEventSystem : MonoBehaviour
{
    [SerializeField] private DialogueEvent[] Events;

    public void Initialize()
    {
    }

    public void InvokeEvent(DialogueActionID actionID)
    {
        if (actionID == DialogueActionID.None) return;

        foreach (var dialogueEvent in Events)
        {
            if (dialogueEvent.ActionID == actionID)
            {
                dialogueEvent.Action.Invoke();
                break;
            }
        }
    }

    public void OnCorrectChoice()
    {
        /*
        var npc = GetCurrentNPC();
        if (npc != null)
        {
            npc.OnCorrectDialogue();
        }
        */
        Debug.Log("OnCorrectChoice");
    }

    public void OnWrongChoice()
    {
        /*
        var npc = GetCurrentNPC();
        if (npc != null)
        {
            npc.OnWrongDialogue();
        }
        */
        Debug.Log("OnWrongChoice");
    }
}
