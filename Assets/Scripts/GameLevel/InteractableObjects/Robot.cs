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
    private Conversation nextConversation = new Conversation();
    private Robot nearbyRobot = null;

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

    public Conversation GetNextConversation()
    {
        return nextConversation;
    }

    public Robot GetNearbyRobot()
    {
        return nearbyRobot;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (Helpers.InTagInTree(collision.gameObject, Tags.DialogTrigger.Value))
        {
            ConversationObject conversation = Helpers.GetComponentInTree<ConversationObject>(collision.gameObject);
            if (conversation != null && nextConversation.IsDone())
            {
                nextConversation = conversation.GetConversation();
                Debug.Log($"{collision.gameObject.name} is setting the next conversation. Is done: {nextConversation.IsDone()}");
                foreach (string line in nextConversation.GetDialogTexts())
                {
                    Debug.Log(line);
                }
            }
        }
        else if (Helpers.InTagInTree(collision.gameObject, Tags.SwitchRobotTrigger.Value))
        {
            Debug.Log("Switch robot trigger entered");
            Robot otherRobot = Helpers.GetComponentInTree<Robot>(collision.gameObject);
            if (otherRobot != null)
            {
                bool shouldAssign;
                if (nearbyRobot == null)
                {
                    shouldAssign = true;
                }
                else
                {
                    float otherDistance = (otherRobot.GetPosition() - GetPosition()).magnitude;
                    float nearbyDistance = (nearbyRobot.GetPosition() - GetPosition()).magnitude;
                    shouldAssign = otherDistance < nearbyDistance;
                }
                if (shouldAssign)
                {
                    nearbyRobot = otherRobot;
                    Debug.Log($"{collision.gameObject.name} is setting the next active robot.");
                }
            }
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if (Helpers.InTagInTree(collision.gameObject, Tags.DialogTrigger.Value))
        {
            ConversationObject conversation = Helpers.GetComponentInTree<ConversationObject>(collision.gameObject);
            if (conversation != null && nextConversation == conversation.GetConversation())
            {
                nextConversation = new Conversation();
                Debug.Log($"{collision.gameObject.name} is clearing the next conversation.");
            }
        }
        else if (Helpers.InTagInTree(collision.gameObject, Tags.SwitchRobotTrigger.Value))
        {
            Robot otherRobot = Helpers.GetComponentInTree<Robot>(collision.gameObject);
            if (otherRobot != null && otherRobot == nearbyRobot)
            {
                nearbyRobot = null;
                Debug.Log($"{collision.gameObject.name} is clearing the next active robot.");
            }
        }
    }
}
