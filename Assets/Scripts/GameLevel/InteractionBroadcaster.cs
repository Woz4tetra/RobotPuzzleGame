using UnityEngine;
using UnityEngine.Assertions;

public class InteractionBroadcaster : MonoBehaviour
{
    private Vector3 startingPosition = Vector3.zero;
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float interactionRadius = 1.0f;
    [SerializeField] private float broadcastRadius = 10.0f;
    private InteractableObject[] interactableObjects;
    void Start()
    {

    }

    void Update()
    {

    }

    public void SetInteractableObjects(InteractableObject[] interactableObjects)
    {
        this.interactableObjects = interactableObjects;
    }

    public void MovePointer(InteractableObjectInput inputs)
    {
        Vector3 moveDirection = new Vector3(inputs.moveDirection.x, inputs.moveDirection.y, 0.0f);
        moveDirection *= movementSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + moveDirection;
        if (Vector3.Distance(newPosition, startingPosition) < broadcastRadius)
        {
            transform.position = newPosition;
        }
        else
        {
            Vector3 direction = newPosition - startingPosition;
            direction.Normalize();
            transform.position = startingPosition + direction * broadcastRadius;
        }
    }

    public InteractableObject GetInteractionObject()
    {
        InteractableObject closestObject = null;
        float closestDistance = 0.0f;
        foreach (InteractableObject interactableObject in interactableObjects)
        {
            Vector3 objectPosition = interactableObject.GetPosition();
            float distance = Vector3.Distance(objectPosition, transform.position);
            if (distance < interactionRadius &&
                (closestObject == null || distance < closestDistance) &&
                !interactableObject.IsActive())
            {
                closestDistance = distance;
                closestObject = interactableObject;
            }
        }
        return closestObject;
    }

    public bool IsInteractionObjectRobot()
    {
        return GetInteractionObject() != null && IsTypeOfInteractionObjectRobot();
    }

    public bool IsInteractionObjectRobotOrNull()
    {
        return GetInteractionObject() == null || IsTypeOfInteractionObjectRobot();
    }

    private bool IsTypeOfInteractionObjectRobot()
    {
        return typeof(Robot).IsInstanceOfType(GetInteractionObject());
    }
    public void OnActiveChange(bool active, Vector3 robotPosition)
    {
        SetGameObjectActive(active);
        if (active)
        {
            Assert.IsTrue(interactableObjects != null && interactableObjects.Length > 0);
            TeleportVisualPointer(robotPosition);
        }
    }

    private void SetGameObjectActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void TeleportVisualPointer(Vector3 position)
    {
        transform.position = position;
        startingPosition = position;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}