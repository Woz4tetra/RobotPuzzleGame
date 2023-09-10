using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private TimePassingManager timePassingManager;
    [SerializeField] private float timeLimit = 30.0f;
    [SerializeField] private ObjectiveCriteria[] criteria;
    void Start()
    {
        foreach (ObjectiveCriteria criterion in criteria)
        {
            criterion.SetTimePassingManager(timePassingManager);
        }

    }

    public bool IsObjectiveComplete()
    {
        if (criteria.Length == 0)
        {
            return false;
        }
        foreach (ObjectiveCriteria criterion in criteria)
        {
            if (!criterion.IsCriteriaMet())
            {
                return false;
            }
        }
        return true;
    }

    public bool IsObjectiveFailed()
    {
        return timePassingManager.GetLevelDuration() > timeLimit;
    }
}