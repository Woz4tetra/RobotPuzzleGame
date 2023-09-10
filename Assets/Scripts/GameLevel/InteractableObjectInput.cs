using UnityEngine;
public class InteractableObjectInput
{
    private Vector2 moveDirection;
    private bool isInteracting;

    public InteractableObjectInput(Vector2 moveDirection, bool isInteracting)
    {
        this.moveDirection = moveDirection;
        this.isInteracting = isInteracting;
    }

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }
}
