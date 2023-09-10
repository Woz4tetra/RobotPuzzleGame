using UnityEngine;
public class InputManager : MonoBehaviour
{
    private bool prevButtonState = false;
    private Vector2 prevClickPosition = Vector2.zero;
    private Vector2 GetMovementVector()
    {
        bool buttonState = GetMouseDown();
        Vector2 mousePosition = Input.mousePosition;
        if (buttonState != prevButtonState)
        {
            prevButtonState = buttonState;
            if (buttonState)
            {
                prevClickPosition = mousePosition;
            }
        }

        if (buttonState)
        {
            Vector2 rawDirection = mousePosition - prevClickPosition;
            Vector2 scaledDirection = new Vector2(rawDirection.x / Screen.width, rawDirection.y / Screen.height);
            float x = Mathf.Max(-1.0f, Mathf.Min(scaledDirection.x, 1.0f));
            float y = Mathf.Max(-1.0f, Mathf.Min(scaledDirection.y, 1.0f));
            return new Vector2(x, y);
        }
        else
        {
            return Vector2.zero;
        }
    }

    private bool GetMouseDown()
    {
        return Input.GetButton("Fire1");
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
        return new InteractableObjectInput(
            GetMovementVector(),
            GetMouseDown()
        );
    }
}