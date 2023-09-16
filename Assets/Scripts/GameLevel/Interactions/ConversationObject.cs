using UnityEngine;
public class ConversationObject : MonoBehaviour
{
    [SerializeField] private Conversation conversation;

    void Start()
    {
        conversation.Reset();
    }

    public Conversation GetConversation()
    {
        return conversation;
    }
}
