using UnityEngine;
public class Robot : InteractableObject
{

    void Update()
    {

    }

    override public InteractableObject Interact(InteractableObjectInput objectInput)
    {
        return this;
    }

    Vector3 LastCommand()
    {
        return Vector3.zero;
    }

    public bool IsInteractionDone()
    {
        return true;
    }
}