using UnityEngine;
class ActingManager : InteractionManager
{
    [SerializeField] GameObject arrowPrefab;
    private GameObject activeArrow;
    private float minArrowMagnitude = 0.25f;
    private bool shouldRobotBeMoving = false;
    private float lastInteractExitTime = 0.0f;
    private float interactionEndTimeout = 0.25f;

    override protected void OnEnterInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} enter interacting");
        Robot robot = interactableObjectManager.GetActiveRobot();
        activeArrow = Instantiate(arrowPrefab, robot.GetPosition(), Quaternion.identity);
        ScaleArrow(robot.GetPosition(), objectInput.GetMoveDirection());
        robot.OnEnterInteracting();
        shouldRobotBeMoving = false;
    }

    override protected void OnInteracting(InteractableObjectInput objectInput)
    {
        Robot robot = interactableObjectManager.GetActiveRobot();
        ScaleArrow(robot.GetPosition(), objectInput.GetMoveDirection());
    }

    override protected void OnExitInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} exit interacting");
        Destroy(activeArrow);
        Robot robot = interactableObjectManager.GetActiveRobot();
        robot.OnExitInteracting(objectInput);
        shouldRobotBeMoving = true;
        lastInteractExitTime = Time.realtimeSinceStartup;
    }

    override public bool IsInteracting()
    {
        Robot robot = interactableObjectManager.GetActiveRobot();
        if (Time.realtimeSinceStartup - lastInteractExitTime > interactionEndTimeout || robot.IsMoving())
        {
            shouldRobotBeMoving = false;
        }
        return !(robot.IsMoving() || shouldRobotBeMoving) || GetState() != InteractionState.Idle;
    }

    private void ScaleArrow(Vector3 robotPosition, Vector2 direction)
    {
        float magnitude = Mathf.Max(minArrowMagnitude, direction.magnitude);
        direction = direction.normalized * magnitude;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
        activeArrow.transform.position = robotPosition;
        activeArrow.transform.localScale = new Vector3(magnitude, 1f, 1f);
        activeArrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}