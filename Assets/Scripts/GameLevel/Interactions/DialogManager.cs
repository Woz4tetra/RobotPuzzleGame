using UnityEngine;
class DialogManager : InteractionManager
{
    private bool isDialogActive = false;
    private ConversationSequence conversation = null;
    private ConversationAction currentAction = null;

    override protected void OnEnterInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} enter interacting");
        UpdateConvoTrigger();
    }

    override protected void OnIdle(InteractableObjectInput objectInput)
    {
        if (UpdateConvoTrigger())
        {
            if (conversation != null && conversation.ShouldActivateOnEnter())
            {
                ForwardNextDialogToDisplay();
            }
        }
    }
    override protected void OnInteracting(InteractableObjectInput objectInput)
    {

    }

    override protected void OnExitInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} exit interacting");
        ForwardNextDialogToDisplay();
    }

    override public bool IsInteracting()
    {
        return isDialogActive;
    }

    private bool UpdateConvoTrigger()
    {
        Robot robot = interactableObjectManager.GetActiveRobot();
        ConversationSequence nextConvo = robot.GetNextConversation();
        if ((conversation == null || conversation.IsDone() || !conversation.IsStarted()) && nextConvo != conversation)
        {
            conversation = nextConvo;
            if (conversation == null || conversation.IsDone())
            {
                Debug.Log("Robot is clearing next conversation");
                return false;
            }
            else
            {
                Debug.Log("Robot is starting a conversation");
                return true;
            }
        }
        return false;
    }

    public void SetConversation(ConversationSequence conversation)
    {
        Debug.Log($"Setting conversation to {conversation.gameObject.name}");
        this.conversation = conversation;
        ForwardNextDialogToDisplay();
    }

    private void ForwardNextDialogToDisplay()
    {
        if (conversation == null)
        {
            isDialogActive = false;
            return;
        }
        (ConversationAction, bool) result = conversation.getNext();
        isDialogActive = !result.Item2;
        if (currentAction != null)
        {
            currentAction.Despawn();
        }
        if (!isDialogActive)
        {
            conversation = null;
            Debug.Log("Conversation ended");
        }
        else
        {
            currentAction = result.Item1;
            currentAction.Render();
            Debug.Log($"Conversation text is {currentAction}");
        }
    }
}
