using UnityEngine;
class DialogManager : InteractionManager
{
    private bool isDialogActive = false;
    private int labelBorderSize = 5;
    private Conversation conversation = new Conversation();
    private readonly Conversation noConversation = new Conversation();
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
        Conversation nextConvo = robot.GetNextConversation();
        if ((conversation.IsDone() || !conversation.IsStarted()) && nextConvo != conversation)
        {
            conversation = nextConvo;
            if (conversation.IsDone())
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

    public void SetDialog(Conversation conversation)
    {
        this.conversation = conversation;
        ForwardNextDialogToDisplay();
    }

    private void ForwardNextDialogToDisplay()
    {
        (string, bool) result = conversation.getNext();
        currentText = result.Item1;
        isDialogActive = !result.Item2;
        if (!isDialogActive)
        {
            conversation = noConversation;
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
