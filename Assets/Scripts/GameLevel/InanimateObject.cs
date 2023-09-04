public class InanimateObject : InteractableObject
{
    override public void Interact(InteractableObjectInput objectInput) { }
    override public void SetActivelyControlled(bool active) { }
    override public bool IsActivelyControlled() { return false; }
    override protected void InteractableObjectUpdate() { }
    override public void Coast() { }
}
