using UnityEngine;
public class InteractionBroadcaster : MonoBehaviour
{
    void Start()
    {

    }

    public InteractableObject GetInteractionObject()
    {
        return null;
    }

    public bool IsInteractionObjectRobot()
    {
        return GetInteractionObject() != null && typeof(Robot).IsInstanceOfType(GetInteractionObject());
    }

    public void SetActive(bool active)
    {

    }
}