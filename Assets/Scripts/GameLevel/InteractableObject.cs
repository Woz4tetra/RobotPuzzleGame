using System;
using UnityEngine;
public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] protected TimePassingManager timePassingManager;
    [SerializeField] private int historyRecordInterval = 10;
    protected Rigidbody body;
    protected HistoryManager historyManager;
    void Start()
    {
        InteractableObjectStart();
    }

    protected void InteractableObjectStart()
    {
        body = GetComponent<Rigidbody>();
        historyManager = new HistoryManager(body, transform, timePassingManager, false);
    }

    void Update()
    {
        if (historyRecordInterval <= 0 || Time.frameCount % historyRecordInterval == 0)
        {
            historyManager.Update();
        }
        InteractableObjectUpdate();
    }
    abstract protected void InteractableObjectUpdate();

    abstract public void Interact(InteractableObjectInput objectInput);
    abstract public void Coast();

    abstract public void SetActive(bool active);
    abstract public bool IsActive();

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void OnExternalCollisionEnter(GameObject collidingObject)
    {
        historyManager.NewMotionCallback();
    }

    public void ZeroVelocities()
    {
        if (!body.isKinematic)
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tags.Robot.Value) || collision.gameObject.CompareTag(Tags.Interactive.Value))
        {
            InteractableObject interactableObject = collision.gameObject.GetComponent<InteractableObject>();
            if (interactableObject != null)
            {
                interactableObject.OnExternalCollisionEnter(gameObject);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (historyManager != null)
        {
            ReadOnlySpan<Vector3> positions = historyManager.getPositions().ToArray();
            Gizmos.DrawLineStrip(positions, false);
        }
    }
}
