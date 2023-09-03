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

    void OnDrawGizmos()
    {
        ReadOnlySpan<Vector3> positions = historyManager.getPositions().ToArray();
        Gizmos.DrawLineStrip(positions, false);
    }
}
