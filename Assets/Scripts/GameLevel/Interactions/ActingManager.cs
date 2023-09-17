using System.Collections.Generic;
using UnityEngine;
class ActingManager : InteractionManager
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject trajectoryLinePrefab;
    [SerializeField] float maxTrajectoryProjectionDistance = 20.0f;
    private GameObject activeArrow;
    private GameObject activeTrajectoryLine;
    private float minArrowMagnitude = 0.25f;
    private bool shouldRobotBeMoving = false;
    private float lastInteractExitTime = 0.0f;
    private float interactionEndTimeout = 0.25f;

    override protected void OnEnterInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} enter interacting");
        Robot robot = interactableObjectManager.GetActiveRobot();
        SpawnArrow(robot.GetPosition());
        ScaleArrow(robot.GetPosition(), objectInput.GetMoveDirection());
        SpawnTrajectoryLine(robot.GetPosition());
        robot.OnEnterInteracting();
        shouldRobotBeMoving = false;
    }

    override protected void OnInteracting(InteractableObjectInput objectInput)
    {
        Robot robot = interactableObjectManager.GetActiveRobot();
        ScaleArrow(robot.GetPosition(), objectInput.GetMoveDirection());
        float projectionDistance = maxTrajectoryProjectionDistance * objectInput.GetMoveDirection().magnitude;
        SetTrajectoryLinePoints(
            ComputeTrajectoryRaycasts(robot.GetPosition(), objectInput.GetMoveDirection(), robot.GetCollisionRadius(), projectionDistance)
        );
    }

    override protected void OnIdle(InteractableObjectInput objectInput)
    {

    }

    override protected void OnExitInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} exit interacting");
        DespawnArrow();
        DespawnTrajectoryLine();
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

    private void SpawnArrow(Vector3 robotPosition)
    {
        activeArrow = Instantiate(arrowPrefab, robotPosition, Quaternion.identity);
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

    private void DespawnArrow()
    {
        Destroy(activeArrow);
    }

    private void SpawnTrajectoryLine(Vector3 robotPosition)
    {
        activeTrajectoryLine = Instantiate(trajectoryLinePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = activeTrajectoryLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(1.0f, 0.0f, 0.0f), 0.0f), new GradientColorKey(new Color(1.0f, 0.1f, 0.1f), 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        lr.colorGradient = gradient;
    }

    private void SetTrajectoryLinePoints(List<Vector3> points)
    {
        if (points.Count == 0)
        {
            return;
        }
        LineRenderer lr = activeTrajectoryLine.GetComponent<LineRenderer>();
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }

    private List<Vector3> ComputeTrajectoryRaycasts(Vector3 robotPosition, Vector2 direction, float collisionRadius, float projectionDistance)
    {
        int layerMask = 1 << LayerMask.NameToLayer(Layers.Interactable.Value);
        // layerMask |= 1 << LayerMask.NameToLayer(Layers.Robot.Value);
        Vector3 castDirection = new Vector3(direction.x, direction.y, 0.0f);
        List<Vector3> results = new List<Vector3> { robotPosition };
        RaycastToNextCollision(ref results, robotPosition, castDirection, collisionRadius, projectionDistance, layerMask);
        for (int index = 0; index < results.Count - 1; index++)
        {
            Debug.DrawLine(results[index], results[index + 1], Color.green);
        }
        return results;
    }

    private void RaycastToNextCollision(ref List<Vector3> results, Vector3 origin, Vector3 direction, float collisionRadius, float remainingDistance, int layerMask)
    {
        RaycastHit hit;
        Ray ray = new Ray(origin, direction.normalized);
        if (remainingDistance > 0.0f && Physics.SphereCast(ray, collisionRadius, out hit, remainingDistance, layerMask))
        {
            Vector3 nextOrigin = hit.point;
            nextOrigin.z = origin.z;
            Vector3 nextDirection = Vector3.Reflect(nextOrigin - origin, hit.normal);
            results.Add(nextOrigin);
            RaycastToNextCollision(ref results, nextOrigin, nextDirection, collisionRadius, remainingDistance - hit.distance, layerMask);
        }
        else
        {
            results.Add(origin + direction.normalized * remainingDistance);
        }
    }

    private void DespawnTrajectoryLine()
    {
        Destroy(activeTrajectoryLine);
    }
}
