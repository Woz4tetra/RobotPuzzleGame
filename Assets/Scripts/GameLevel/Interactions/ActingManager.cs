using System.Collections.Generic;
using UnityEngine;
class ActingManager : InteractionManager
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject powerUpPrefab;
    [SerializeField] GameObject trajectoryLinePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float maxTrajectoryProjectionDistance = 30.0f;
    [SerializeField] float inputProjectMulitplier = 50.0f;
    [SerializeField] int maxBouncePredictions = 3;
    private GameObject activeArrow;
    private GameObject activePowerUp;
    private RocketPowerUpAnimation powerUpAnimator;
    private GameObject activeTrajectoryLine;
    private LineRenderer trajectoryLineRenderer;
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

        SpawnPowerUp(robot.GetPosition(), objectInput.GetMoveDirection(), robot.GetCollisionRadius());
        ScalePowerUp(robot.GetPosition(), objectInput.GetMoveDirection(), robot.GetCollisionRadius());

        SpawnTrajectoryLine();
        robot.OnEnterInteracting(objectInput);
        shouldRobotBeMoving = false;
    }

    override protected void OnInteracting(InteractableObjectInput objectInput)
    {
        Robot robot = interactableObjectManager.GetActiveRobot();
        ScaleArrow(robot.GetPosition(), objectInput.GetMoveDirection());
        ScalePowerUp(robot.GetPosition(), objectInput.GetMoveDirection(), robot.GetCollisionRadius());
        float projectionDistance = Mathf.Min(
            maxTrajectoryProjectionDistance,
            inputProjectMulitplier * objectInput.GetMoveDirection().magnitude
        );
        SetTrajectoryLinePoints(
            ComputeTrajectoryRaycasts(robot.GetPosition(), objectInput.GetMoveDirection(), robot.GetCollisionRadius(), projectionDistance)
        );
        robot.OnInteracting(objectInput);
    }

    override protected void OnIdle(InteractableObjectInput objectInput)
    {

    }

    override protected void OnExitInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} exit interacting");
        Robot robot = interactableObjectManager.GetActiveRobot();
        DespawnArrow();
        DespawnPowerUp();
        DespawnTrajectoryLine();
        SpawnExplosion(robot.GetPosition(), robot.GetCollisionRadius(), objectInput.GetMoveDirection());
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

    private void SpawnPowerUp(Vector3 robotPosition, Vector2 direction, float robotRadius)
    {
        Vector3 spawnLocation = GetPowerUpPosition(robotPosition, direction, robotRadius);
        activePowerUp = Instantiate(powerUpPrefab, spawnLocation, Quaternion.identity);
        powerUpAnimator = activePowerUp.GetComponent<RocketPowerUpAnimation>();
    }

    private void ScalePowerUp(Vector3 robotPosition, Vector2 direction, float robotRadius)
    {
        activePowerUp.transform.position = GetPowerUpPosition(robotPosition, direction, robotRadius);
        float magnitude = Mathf.Max(minArrowMagnitude, direction.magnitude) * 4.0f;
        powerUpAnimator.SetIntensity(magnitude);
    }

    private Vector3 GetPowerUpPosition(Vector3 robotPosition, Vector2 direction, float robotRadius)
    {
        Vector3 direction3 = new Vector3(direction.x, direction.y, 0.0f);
        Vector3 result = robotPosition - direction3.normalized * robotRadius;
        result.z -= 0.25f;
        return result;
    }

    private void DespawnPowerUp()
    {
        Destroy(activePowerUp);
    }

    private void SpawnTrajectoryLine()
    {
        activeTrajectoryLine = Instantiate(trajectoryLinePrefab, Vector3.zero, Quaternion.identity);
        trajectoryLineRenderer = activeTrajectoryLine.GetComponent<LineRenderer>();
        trajectoryLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(1.0f, 0.0f, 0.0f), 0.0f), new GradientColorKey(new Color(1.0f, 0.1f, 0.1f), 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        trajectoryLineRenderer.colorGradient = gradient;
        Debug.Log($"trajectoryLineRenderer: {trajectoryLineRenderer}");
    }

    private void SetTrajectoryLinePoints(List<Vector3> points)
    {
        if (points.Count == 0)
        {
            return;
        }
        trajectoryLineRenderer.positionCount = points.Count;
        trajectoryLineRenderer.SetPositions(points.ToArray());
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
        if (remainingDistance <= 0.0f)
        {
            return;
        }
        if (Physics.SphereCast(ray, collisionRadius, out hit, remainingDistance, layerMask))
        {
            Vector3 nextOrigin = hit.point;
            nextOrigin.z = origin.z;
            Vector3 nextDirection = Vector3.Reflect(nextOrigin - origin, hit.normal);
            results.Add(nextOrigin);
            if (results.Count < maxBouncePredictions)
            {
                RaycastToNextCollision(ref results, nextOrigin, nextDirection, collisionRadius, remainingDistance - hit.distance, layerMask);
            }
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

    private void SpawnExplosion(Vector3 robotPosition, float robotRadius, Vector2 inputDirection)
    {
        // Explosions despawn on animation complete
        if (inputDirection.magnitude < 0.01f)
        {
            return;
        }
        float multiplier = Mathf.Min(1.0f, Mathf.Max(0.0f, inputDirection.magnitude));
        Vector3 spawnPosition = new Vector3(robotPosition.x, robotPosition.y, -robotRadius * multiplier * 1.5f);
        Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);
    }
}
