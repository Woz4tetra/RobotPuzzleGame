using UnityEngine;
using UnityEngine.Assertions;

public class RobotSpriteAnimator : MonoBehaviour
{
    [SerializeField] private TimePassingManager timePassingManager;
    [SerializeField] private Sprite[] spriteArray;
    private SpriteRenderer spriteRenderer;
    private Robot robot;
    private float prevDuration = 0.0f;
    private int length = 0;
    private float epsilon = 1e-4f;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        robot = Helpers.GetComponentInTree<Robot>(gameObject);
        Assert.IsTrue(spriteArray.Length == 8);
        length = spriteArray.Length;
    }

    void Update()
    {
        float levelDuration = timePassingManager.GetLevelDuration();
        int index;

        Vector3 direction = GetDirection();
        if (direction.magnitude < epsilon)
        {
            return;
        }

        float deltaDuration = levelDuration - prevDuration;
        prevDuration = levelDuration;

        float angle = Mathf.Atan2(direction.y, direction.x);
        index = Mathf.RoundToInt(angle / (Mathf.PI / (length / 2)));
        if (deltaDuration < 0.0f)
        {
            index = index + length / 2;
        }
        index = (index + length) % length;
        spriteRenderer.sprite = spriteArray[index];
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = robot.GetDirection();
        if (direction.magnitude < epsilon)
        {
            direction = robot.GetNextForce();
        }

        if (direction.magnitude < epsilon)
        {
            float levelDuration = timePassingManager.GetLevelDuration();
            if (levelDuration == 0.0f && robot.GetForce().magnitude < epsilon)
            {
                direction = new Vector3(1.0f, -1.0f, 0.0f);
            }
        }
        return direction;
    }
}
