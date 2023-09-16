using UnityEngine;

public class Robot : InteractableObject
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float forceMagnitude = 1.0f;
    [SerializeField] float frozenSpeed = 0.1f;
    [SerializeField] float rapidDecelSpeedThreshold = 3.0f;
    [SerializeField] float lowDrag = 0.5f;
    [SerializeField] float highDrag = 10.0f;
    float epsilon = 1e-3f;
    private float forceDecay = 0.7f;
    private Vector3 force = Vector3.zero;
    private Conversation activeConversation = new Conversation();

    void Update()
    {
        if (body.isKinematic)
        {
            return;
        }
        bool isMoving = IsMoving();
        if (force.magnitude > epsilon)
        {
            if (isMoving)
            {
                force = forceDecay * force;
            }
            body.AddForce(force, ForceMode.Impulse);
        }
        else
        {
            if (!isMoving)
            {
                body.drag = lowDrag;
            }
            else if (body.velocity.magnitude < rapidDecelSpeedThreshold)
            {
                body.drag = highDrag;
            }
        }
    }

    public bool IsMoving()
    {
        return body.velocity.magnitude > frozenSpeed;
    }

    override public void OnEnterInteracting()
    {

    }

    override public void OnExitInteracting(InteractableObjectInput objectInput)
    {
        body.drag = lowDrag;
        Vector2 moveDirection = objectInput.GetMoveDirection();
        force = new Vector3
        {
            x = moveDirection.x,
            y = moveDirection.y,
            z = 0.0f
        };
        force *= forceMagnitude;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Conversation GetActiveConversation()
    {
        return activeConversation;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (Helpers.InTagInTree(collision.gameObject, Tags.DialogTrigger.Value))
        {
            ConversationObject conversation = collision.gameObject.GetComponent<ConversationObject>();
            if (conversation != null && activeConversation.IsDone())
            {
                activeConversation = conversation.GetConversation();
                Debug.Log($"{collision.gameObject.name} is setting the conversation. Is done: {activeConversation.IsDone()}");
                foreach (string line in activeConversation.GetDialogTexts())
                {
                    Debug.Log(line);
                }
            }
        }
    }
}
