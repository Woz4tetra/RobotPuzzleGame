public class InanimateObject : InteractableObject
{
    override public void Interact(InteractableObjectInput objectInput) { }
    override public void SetActive(bool active) { }
    override public bool IsActive() { return false; }
}
