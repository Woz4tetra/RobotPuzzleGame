using System;
using UnityEngine;
public class InanimateObject : MonoBehaviour
{
    [SerializeField] TimePassingManager timePassingManager;
    [SerializeField] int historyRecordInterval = 10;
    protected Rigidbody body;
    protected HistoryManager historyManager;
    void Start()
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
    }

    void OnDrawGizmos()
    {
        ReadOnlySpan<Vector3> positions = historyManager.getPositions().ToArray();
        Gizmos.DrawLineStrip(positions, false);
    }
}
