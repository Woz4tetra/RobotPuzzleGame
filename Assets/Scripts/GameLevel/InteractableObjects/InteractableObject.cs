using System;
using Unity.VisualScripting;
using UnityEngine;
public abstract class InteractableObject : MonoBehaviour
{
    protected Rigidbody body;
    protected HistoryManager historyManager = new HistoryManager();

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    abstract public void OnEnterInteracting();
    abstract public void OnExitInteracting(InteractableObjectInput objectInput);

    void OnDrawGizmos()
    {
        if (historyManager != null)
        {
            ReadOnlySpan<Vector3> positions = historyManager.getPositions().ToArray();
            Gizmos.DrawLineStrip(positions, false);
        }
    }

    public void FreezeObject()
    {
        Debug.Log($"Freezing object {gameObject.name} at {transform.position}");
        body.isKinematic = true;
    }

    public void RecordEvent(float levelDuration)
    {
        historyManager.RecordEvent(new ObjectInstant(
            transform.position, transform.rotation, body.velocity, body.angularVelocity
        ), levelDuration);
    }

    public void UnfreezeObject(float levelDuration)
    {
        body.isKinematic = false;
        ObjectInstant instant = historyManager.UnfreezeObject(levelDuration);
        if (instant == null)
        {
            return;
        }
        Debug.Log($"Unfreezing object {gameObject.name}");
        body.velocity = instant.velocity;
        body.angularVelocity = instant.angularVelocity;
    }

    public void JumpToInstant(float levelDuration)
    {
        ObjectInstant instant = historyManager.JumpToInstant(levelDuration);
        if (instant == null)
        {
            return;
        }
        Debug.Log($"Jumping object {gameObject.name} to {instant.pose.GetT()}");
        transform.position = instant.pose.GetT();
        transform.rotation = instant.pose.GetR();
    }
}
