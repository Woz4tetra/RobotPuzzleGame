using UnityEngine;
public abstract class ObjectiveCriteria : MonoBehaviour
{
    protected TimePassingManager timePassingManager;

    public void SetTimePassingManager(TimePassingManager timePassingManager)
    {
        this.timePassingManager = timePassingManager;
    }

    public abstract bool IsCriteriaMet();
}