using UnityEngine;
public class Robot : InteractableObject
{
    [SerializeField] private float forceMagnitude = 1.0f;
    [SerializeField] private float fastForceMagnitude = 2.0f;
    [SerializeField] private float maximumSpeed = 3.0f;

    override protected void InteractableObjectUpdate()
    {

    }

    override public void Interact(InteractableObjectInput objectInput)
    {
        float speed = Vector3.Magnitude(body.velocity);  // test current object speed

        if (speed > maximumSpeed)
        {
            float brakeSpeed = speed - maximumSpeed;  // calculate the speed decrease

            Vector3 normalizedVelocity = body.velocity.normalized;
            Vector3 brakeVelocity = normalizedVelocity * brakeSpeed;  // make the brake Vector3 value

            body.AddForce(-brakeVelocity);  // apply opposing brake force
        }
        else
        {
            Vector3 force = new Vector3(objectInput.moveDirection.x, objectInput.moveDirection.y, 0.0f);
            force *= objectInput.fastToggle ? fastForceMagnitude : forceMagnitude;
            body.AddForce(force, ForceMode.VelocityChange);
        }
        historyManager.NewMotionCallback();
    }

    override public void SetActive(bool active)
    {
        historyManager.SetActiveControl(active);
    }
    override public bool IsActive()
    {
        return historyManager.IsControlActive();
    }
}