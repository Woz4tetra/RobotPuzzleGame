using System;
using UnityEngine;
public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] TimePassingManager timePassingManager;
    [SerializeField] int historyRecordInterval = 10;
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
        InteractableObjectUpdate();
    }
    protected void InteractableObjectUpdate()
    {
        if (historyRecordInterval <= 0 || Time.frameCount % historyRecordInterval == 0)
        {
            historyManager.Update();
        }
    }

    abstract public void Interact(InteractableObjectInput objectInput);

    abstract public void SetActive(bool active);

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void OnExternalCollisionEnter(GameObject collidingObject)
    {
        historyManager.NewMotionCallback();
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Interactive"))
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
        ReadOnlySpan<Vector3> positions = historyManager.getPositions().ToArray();
        Gizmos.DrawLineStrip(positions, false);
    }
}
