using System;
using UnityEngine;

[Serializable]
public class Conversation
{
    [SerializeField] private string[] dialogTexts;
    [SerializeField] private bool activateOnEnter = false;
    private int dialogIndex = -1;
    private bool isDone = false;

    public Conversation()
    {
        dialogTexts = new string[] { };
        Reset();
    }
    public Conversation(string[] dialogTexts)
    {
        this.dialogTexts = dialogTexts;
        Reset();
    }

    public (string, bool) getNext()
    {
        dialogIndex++;
        string next;
        if (dialogIndex >= dialogTexts.Length)
        {
            isDone = true;
            next = "";
        }
        else
        {
            next = dialogTexts[dialogIndex];
        }
        return (next, isDone);
    }

    public void Reset()
    {
        dialogIndex = -1;
        if (dialogTexts.Length == 0)
        {
            isDone = true;
        }
        else
        {
            isDone = false;
        }
    }

    public void FinishConversation()
    {
        isDone = true;
    }

    public bool IsDone()
    {
        return isDone;
    }
    public bool IsStarted()
    {
        return dialogIndex >= 0;
    }
    public string[] GetDialogTexts()
    {
        return dialogTexts;
    }

    public bool ShouldActivateOnEnter()
    {
        return activateOnEnter;
    }
}
