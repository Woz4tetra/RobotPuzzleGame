using UnityEngine;
public class InputManager : MonoBehaviour
{
    [SerializeField] float movementDeadzone = 0.1f;

    void Start()
    {

    }

    private Vector2 GetMovementVector()
    {
        float vertical = 0.0f;
        float horizontal = 0.0f;
        if (Input.GetKey(KeyCode.W))
        {
            vertical += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vertical -= 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontal -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontal += 1.0f;
        }

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

    public bool FastMovementToggled()
    {
        return Input.GetKey(KeyCode.LeftControl);  // TODO reference key mapping
    }

    public bool IsReset()
    {
        return Input.GetKeyDown(KeyCode.R);  // TODO reference key mapping
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
            moveDirection = GetMovementVector(),
            fastToggle = FastMovementToggled()
        };
    }
}