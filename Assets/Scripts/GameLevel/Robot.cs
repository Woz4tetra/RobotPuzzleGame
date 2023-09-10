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
    private float forceDecay = 0.9f;
    private float minArrowMagnitude = 0.25f;
    private GameObject activeArrow;
    private bool wasInteracting = false;
    private bool shouldInteract = true;
    private Vector2 prevDirection = Vector2.zero;
    private Vector3 force = Vector3.zero;

    void Update()
    {
        if (body.isKinematic)
        {
            return;
        }
        bool isMoving = body.velocity.magnitude > frozenSpeed;
        if (force.magnitude > epsilon)
        {
            if (isMoving)
            {
                force = forceDecay * force;
            }
            body.AddForce(force, ForceMode.VelocityChange);
        }
        else if (!shouldInteract)
        {
            if (!isMoving)
            {
                body.drag = lowDrag;
                shouldInteract = true;
            }
            else if (body.velocity.magnitude < rapidDecelSpeedThreshold)
            {
                body.drag = highDrag;
            }
        }
    }

    override public InteractableObject Interact(InteractableObjectInput objectInput)
    {
        UpdateInteractingState(objectInput);
        if (objectInput.IsInteracting())
        {
            Vector2 direction = objectInput.GetMoveDirection();
            float magnitude = Mathf.Max(minArrowMagnitude, direction.magnitude);
            direction = direction.normalized * magnitude;
            ScaleArrow(direction);
            prevDirection = direction;
        }
        return this;
    }

    private void OnEnterInteracting()
    {
        activeArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        shouldInteract = true;
    }

    private void OnExitInteracting(Vector2 moveDirection)
    {
        force = new Vector3
        {
            x = moveDirection.x,
            y = moveDirection.y,
            z = 0.0f
        };
        force *= forceMagnitude;
        Destroy(activeArrow);
        shouldInteract = false;
    }

    private void UpdateInteractingState(InteractableObjectInput objectInput)
    {
        bool isInteracting = objectInput.IsInteracting();
        if (isInteracting != wasInteracting)
        {
            if (isInteracting)
            {
                OnEnterInteracting();
            }
            else
            {
                OnExitInteracting(prevDirection);
            }
        }
        wasInteracting = isInteracting;
    }

    private void ScaleArrow(Vector2 direction)
    {
        float magnitude = direction.magnitude;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
        activeArrow.transform.localScale = new Vector3(magnitude, 1f, 1f);
        activeArrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public bool ShouldInteract()
    {
        return shouldInteract;
    }
}