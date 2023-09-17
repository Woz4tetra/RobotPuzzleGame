using UnityEngine;

public class ConversationSequence : MonoBehaviour
{
    [SerializeField] private ConversationAction[] actions;
    [SerializeField] private bool activateOnEnter = false;
    private int index = -1;
    private bool isDone = false;

    private ConversationAction[] GetActions()
    {
        return actions;
    }

    public (ConversationAction, bool) getNext()
    {
        ConversationAction[] actions = GetActions();
        index++;
        ConversationAction next;
        if (index >= actions.Length)
        {
            isDone = true;
            next = null;
        }
        else
        {
            next = actions[index];
        }
        return (next, isDone);
    }

    public void Reset()
    {
        index = -1;
        if (GetActions().Length == 0)
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
        return index >= 0;
    }

    public bool ShouldActivateOnEnter()
    {
        return activateOnEnter;
    }
}
