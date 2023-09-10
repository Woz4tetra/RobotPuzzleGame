using UnityEngine;
public class TouchTargetObjectiveCriteria : ObjectiveCriteria
{
    [SerializeField] GameObject VisualsObject;
    private float clearTime = -1.0f;

    void Update()
    {
        bool isMet = IsCriteriaMet();
        if (!isMet)
        {
            clearTime = -1.0f;
        }
        VisualsObject.gameObject.SetActive(!isMet);
    }

    override public bool IsCriteriaMet()
    {
        return 0.0f <= clearTime && clearTime <= timePassingManager.GetLevelDuration();
    }

    private bool InTagInTree(GameObject obj, string tag)
    {
        if (obj.tag == tag)
        {
            return true;
        }
        Transform tf = obj.transform;
        while (true)
        {
            if (tf == null)
            {
                return false;
            }
            if (tf.gameObject.tag == tag)
            {
                return true;
            }
            tf = tf.parent;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (InTagInTree(collision.gameObject, Tags.Robot.Value))
        {
            clearTime = timePassingManager.GetLevelDuration();
        }
    }
}