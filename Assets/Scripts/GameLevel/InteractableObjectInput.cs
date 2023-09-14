using UnityEngine;
public class InteractableObjectInput
{
    private Vector2 moveDirection;
    private bool shouldAct;
    private bool shouldDialog;

    public InteractableObjectInput()
    {
        moveDirection = Vector2.zero;
        shouldAct = false;
        shouldDialog = false;
    }

    public InteractableObjectInput(Vector2 moveDirection, bool shouldAct, bool shouldDialog)
    {
        this.moveDirection = moveDirection;
        this.shouldAct = shouldAct;
        this.shouldDialog = shouldDialog;
    }

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool ShouldAct()
    {
        return shouldAct;
    }

    public bool ShouldDialog()
    {
        return shouldDialog;
    }
}
