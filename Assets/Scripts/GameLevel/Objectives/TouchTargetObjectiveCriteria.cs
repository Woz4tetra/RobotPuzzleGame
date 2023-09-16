using UnityEngine;
public class TouchTargetObjectiveCriteria : ObjectiveCriteria
{
    [SerializeField] GameObject VisualsObject;
    private float clearTime = -1.0f;

    void Update()
    {
        bool isMet = IsCriteriaMet();
        if (!isMet && timePassingManager.IsAtFrontier())
        {
            ResetCriteria();
        }
        VisualsObject.gameObject.SetActive(!isMet);
    }

    override public bool IsCriteriaMet()
    {
        return 0.0f <= clearTime && clearTime <= timePassingManager.GetLevelDuration();
    }

    void ResetCriteria()
    {
        clearTime = -1.0f;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (Helpers.InTagInTree(collision.gameObject, Tags.Robot.Value))
        {
            clearTime = timePassingManager.GetLevelDuration();
        }
    }
}