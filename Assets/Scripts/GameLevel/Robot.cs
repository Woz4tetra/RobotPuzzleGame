using UnityEngine;
public class Robot : InteractableObject
{
    [SerializeField] private float forceMagnitude = 10.0f;

    override public void Interact(InteractableObjectInput objectInput)
    {
        Vector3 force = new Vector3(objectInput.moveDirection.x, objectInput.moveDirection.y, 0.0f);
        force *= forceMagnitude;
        body.velocity = force;
        historyManager.ResumeFromSave();
    }

    override public void SetActive(bool active)
    {
        historyManager.SetActiveControl(active);
    }
}