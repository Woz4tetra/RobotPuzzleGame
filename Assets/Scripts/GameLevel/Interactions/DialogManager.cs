using UnityEngine;
class DialogManager : InteractionManager
{
    private bool isDialogActive = false;
    private int labelBorderSize = 5;
    private Conversation conversation = new Conversation();
    private string currentText = "";

    override protected void OnEnterInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} enter interacting");
        UpdateConvoTrigger();
    }

    override protected void OnIdle(InteractableObjectInput objectInput)
    {
        if (UpdateConvoTrigger())
        {
            if (conversation.ShouldActivateOnEnter())
            {
                DequeueDialog();
            }
        }
    }
    override protected void OnInteracting(InteractableObjectInput objectInput)
    {

    }

    override protected void OnExitInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} exit interacting");
        DequeueDialog();
    }

    override public bool IsInteracting()
    {
        return isDialogActive;
    }

    private bool UpdateConvoTrigger()
    {
        Robot robot = interactableObjectManager.GetActiveRobot();
        Conversation nextConvo = robot.GetActiveConversation();
        if (conversation.IsDone() && !nextConvo.IsDone())
        {
            conversation = nextConvo;
            Debug.Log("Robot is starting a conversation");
            return true;
        }
        return false;
    }

    public void QueueDialog(Conversation conversation)
    {
        this.conversation = conversation;
        DequeueDialog();
    }

    private void DequeueDialog()
    {
        (string, bool) result = conversation.getNext();
        currentText = result.Item1;
        isDialogActive = !result.Item2;
        if (!isDialogActive)
        {
            conversation = new Conversation();
            Debug.Log("Conversation ended");
        }
        else
        {
            Debug.Log($"Conversation text is {currentText}");
        }
    }

    void OnGUI()
    {
        if (!isDialogActive)
        {
            return;
        }
        string text = currentText;
        GUIStyle labelStyle = new GUIStyle
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        Vector2 size = labelStyle.CalcSize(new GUIContent(text));

        Rect boxRect = new Rect(0, Screen.height - size.y - 2 * labelBorderSize, size.x + 2 * labelBorderSize, size.y + 2 * labelBorderSize);
        Rect labelRect = new Rect(boxRect.x + labelBorderSize, boxRect.y + labelBorderSize, size.x, size.y);
        GUI.Box(boxRect, GUIContent.none);
        GUI.Label(labelRect, text, labelStyle);
    }
}
