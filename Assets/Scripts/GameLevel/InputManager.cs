using UnityEngine;
public class InputManager : MonoBehaviour
{
    [SerializeField] float inputSmoothing = 0.5f;
    private Vector2 prevMovementVector = Vector2.zero;

    private Vector2 GetMovementVector()
    {
        prevMovementVector = Vector2.Lerp(GetMovementVectorRaw(), prevMovementVector, inputSmoothing);
        return prevMovementVector;
    }
    private Vector2 GetMovementVectorRaw()
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

    public bool IsReset()
    {
        return Input.GetKeyDown(KeyCode.R);  // TODO reference key mapping
    }

    public int GetSeekDirection()
    {
        bool isForwarding = Input.GetKeyDown(KeyCode.E);  // TODO reference key mapping
        bool isReversing = Input.GetKeyDown(KeyCode.Q);
        if (isReversing)
        {
            return -1;
        }
        else if (isForwarding)
        {
            return 1;
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