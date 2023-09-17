using UnityEngine;
class SwitchRobotManager : InteractionManager
{
    private Robot nextActiveRobot = null;
    override protected void OnEnterInteracting(InteractableObjectInput objectInput)
    {

    }

    override protected void OnInteracting(InteractableObjectInput objectInput)
    {

    }

    override protected void OnIdle(InteractableObjectInput objectInput)
    {

    }

    override protected void OnExitInteracting(InteractableObjectInput objectInput)
    {
        Robot robot = interactableObjectManager.GetActiveRobot();
        nextActiveRobot = robot.GetNearbyRobot();
        if (nextActiveRobot == null)
        {
            Debug.Log("No nearby robot");
        }
        else
        {
            Debug.Log($"Switching to robot {nextActiveRobot.gameObject.name}");
        }
    }

    public void ResetNearbyRobot()
    {
        nextActiveRobot = null;
    }

    public Robot GetNextActiveRobot()
    {
        return nextActiveRobot;
    }

    override public bool IsInteracting()
    {
        return false;
    }
}