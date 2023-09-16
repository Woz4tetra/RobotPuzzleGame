using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectManager : MonoBehaviour
{
    [SerializeField] private InteractableObject[] interactableObjects;
    [SerializeField] private TimePassingManager timePassingManager;
    [SerializeField] private Robot activeRobot;
    private List<Robot> robots = new List<Robot>();

    void Start()
    {
        foreach (InteractableObject obj in interactableObjects)
        {
            if (IsObjectRobot(obj))
            {
                robots.Add(obj as Robot);
            }
        }
    }

    public Robot GetActiveRobot()
    {
        return activeRobot;
    }

    public static bool IsObjectRobot(InteractableObject obj)
    {
        return typeof(Robot).IsInstanceOfType(obj);
    }

    public void SetActiveRobot(Robot robot)
    {
        activeRobot = robot;
    }

    public void FreezeObjects()
    {
        Debug.Log("Freezing objects");
        foreach (InteractableObject obj in interactableObjects)
        {
            obj.FreezeObject();
        }
    }

    public void UnfreezeObjects()
    {
        Debug.Log("Unfreezing objects");
        timePassingManager.Unfreeze();
        foreach (InteractableObject obj in interactableObjects)
        {
            obj.UnfreezeObject(timePassingManager.GetLevelDuration());
        }
    }

    public void RecordObjectEvent()
    {
        foreach (InteractableObject obj in interactableObjects)
        {
            obj.RecordEvent(timePassingManager.GetLevelDuration());
        }
    }

    public void JumpToObjectInstant(float levelDuration)
    {
        foreach (InteractableObject obj in interactableObjects)
        {
            obj.JumpToInstant(levelDuration);
        }
    }
}