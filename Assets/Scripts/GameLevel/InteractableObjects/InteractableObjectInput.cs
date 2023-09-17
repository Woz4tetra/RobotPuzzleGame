using UnityEngine;
public class InteractableObjectInput
{
    private Vector2 moveDirection;
    private bool shouldAct;
    private bool shouldDialog;
    private bool shouldSwitch;

    public InteractableObjectInput()
    {
        moveDirection = Vector2.zero;
        shouldAct = false;
        shouldDialog = false;
        shouldSwitch = false;
    }

    public InteractableObjectInput(Vector2 moveDirection, bool shouldAct, bool shouldDialog, bool shouldSwitch)
    {
        this.moveDirection = moveDirection;
        this.shouldAct = shouldAct;
        this.shouldDialog = shouldDialog;
        this.shouldSwitch = shouldSwitch;
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

    public bool ShouldSwitch()
    {
        return shouldSwitch;
    }
}
