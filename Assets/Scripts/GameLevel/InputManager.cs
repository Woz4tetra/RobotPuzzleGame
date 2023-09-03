using UnityEngine;
public class InputManager : MonoBehaviour
{
    [SerializeField] float movementDeadzone = 0.1f;
    void Start()
    {

    }

    private Vector2 GetMovementVector()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        return new Vector2(horizontal, vertical);
    }

    public bool PauseToggled()
    {
        return Input.GetKeyDown(KeyCode.Escape);  // TODO reference key mapping
    }

    public bool InteractToggled()
    {
        return Input.GetKeyDown(KeyCode.F);  // TODO reference key mapping
    }

    public bool SeekToggled()
    {
        return GetSeekDirection() != 0;
    }

    public bool MoveToggled()
    {
        return GetMovementVector().magnitude > movementDeadzone;
    }

    public int GetSeekDirection()
    {
        bool isForwarding = Input.GetKey(KeyCode.E);  // TODO reference key mapping
        bool isReversing = Input.GetKey(KeyCode.Q);
        if (isForwarding)
        {
            return 1;
        }
        else if (isReversing)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public InteractableObjectInput GetInteractionStruct()
    {
        return new InteractableObjectInput
        {
            moveDirection = GetMovementVector()
        };
    }
}