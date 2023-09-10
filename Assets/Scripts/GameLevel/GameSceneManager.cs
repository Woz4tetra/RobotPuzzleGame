using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    /**
        Scene setup instructions
        Create empty game object at the origin:
            - Managers
            - Robots
            - InanimateObjects
            - StaticObjects
            - Objectives
        Add child objects to Managers:
            - SceneManager
            - TimePassingManager
            - InputManager
            - PauseMenuManager
            - ObjectiveManager
        Add child objects to Robots:
            - Any robots in the scene (game objects with Robot script attached)
        Add child objects to InanimateObjects:
            - Any inanimate objects in the scene (game objects with InanimateObject script attached)
        Add child objects to StaticObjects:
            - Any static objects in the scene
        Add child objects to Objectives:
            - Any objective criteria in the scene (game objects with an ObjectiveCriteria type script attached)

        Edit Main Camera:
            - Add component CameraObjectFollower Script
            - Set Follow Object to initial Robot
        Edit SceneManager:
            - Add as many controllable objects as there are robots in the scene
                - Point each entry to a unique robot
            - Point TimePassingManager to TimePassingManager object
            - Point InputManager to InputManager object
            - Point PauseMenuManager to PauseMenuManager object
            - Point ObjectiveManager to ObjectiveManager object
            - Point Active Robot to initial Robot
            - Point CameraFollower to CameraFollower object
        Edit ObjectiveManager:
            - Add all objective criteria in the scene to "Criteria"
     */
    public enum LevelState
    {
        Start,
        Paused,
        Frozen,
        Moving,
        GameOver,
        GameWin
    };

    [SerializeField] private InteractableObject[] interactableObjects;
    [SerializeField] private TimePassingManager timePassingManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PauseMenuManager pauseMenuManager;
    [SerializeField] private ObjectiveManager objectiveManager;
    [SerializeField] private Robot activeRobot;
    [SerializeField] private CameraObjectFollower cameraFollower;
    private List<Robot> robots = new List<Robot>();

    private int labelBorderSize = 5;

    private LevelState levelState = LevelState.Start;
    private LevelState beforePauseState = LevelState.Frozen;

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

    void Update()
    {
        LevelState newState = GetActiveState(levelState);
        StateUpdate(newState, levelState);
        levelState = newState;
    }

    LevelState GetActiveState(LevelState prevState)
    {
        LevelState state = prevState;
        if (prevState == LevelState.Start)
        {
            state = LevelState.Frozen;
        }
        else if (inputManager.PauseToggled())
        {
            if (prevState == LevelState.Paused)
            {
                state = beforePauseState;
            }
            else
            {
                beforePauseState = prevState;
                state = LevelState.Paused;
            }
        }
        else if (prevState != LevelState.Paused)
        {
            if (objectiveManager.IsObjectiveComplete())
            {
                state = LevelState.GameWin;
            }
            else if (objectiveManager.IsObjectiveFailed())
            {
                state = LevelState.GameOver;
            }
            else if (GetActiveRobot().ShouldInteract())
            {
                state = LevelState.Frozen;
            }
            else
            {
                state = LevelState.Moving;
            }
        }
        return state;
    }

    void StateUpdate(LevelState newState, LevelState prevState)
    {
        if (newState != prevState)
        {
            OnStateExit(prevState);
            OnStateEnter(newState);
        }
        float deltaTime = 0.0f;
        switch (newState)
        {
            case LevelState.Moving:
                deltaTime = Time.deltaTime;
                GetActiveRobot().Interact(inputManager.GetInteractionStruct());
                break;
            case LevelState.Paused:
                break;
            case LevelState.Frozen:
                int seekDirection = inputManager.GetSeekDirection();
                Robot robot = GetActiveRobot();
                InteractableObject switchedRobot;
                if (seekDirection != 0)
                {
                    switchedRobot = JumpToEvent(seekDirection);
                }
                else
                {
                    switchedRobot = robot.Interact(inputManager.GetInteractionStruct());
                }

                if (switchedRobot != robot && IsObjectRobot(switchedRobot))
                {
                    SetActiveRobot(switchedRobot as Robot);
                    cameraFollower.SetFollowObject(switchedRobot.gameObject);
                }
                break;
            case LevelState.GameOver:
                // TODO show game over screen
                break;
            case LevelState.GameWin:
                // TODO show win screen
                break;
            default:
                break;
        }

        timePassingManager.SeekTime(deltaTime);
    }

    bool IsObjectRobot(InteractableObject obj)
    {
        return typeof(Robot).IsInstanceOfType(obj);
    }

    void OnStateEnter(LevelState state)
    {
        Debug.Log($"Entering state {state}");
        switch (state)
        {
            case LevelState.Moving:
                UnfreezeObjects();
                break;
            case LevelState.Frozen:
                FreezeObjects(GetActiveRobot());
                break;
            case LevelState.Paused:
                pauseMenuManager.OnActiveChange(true);
                break;
            default:
                break;
        }
    }

    void OnStateExit(LevelState state)
    {
        Debug.Log($"Exiting state {state}");
        switch (state)
        {
            case LevelState.Paused:
                pauseMenuManager.OnActiveChange(false);
                break;
            default:
                break;
        }

    }

    void FreezeObjects(Robot activeRobot)
    {
        Debug.Log("Freezing objects");
        timePassingManager.RecordEvent(new SceneInstant
        {
            activeRobot = activeRobot,
            levelDuration = timePassingManager.GetLevelDuration()
        });
        foreach (InteractableObject obj in interactableObjects)
        {
            obj.FreezeObject(timePassingManager.GetLevelDuration());
        }
    }

    void UnfreezeObjects()
    {
        Debug.Log("Unfreezing objects");
        foreach (InteractableObject obj in interactableObjects)
        {
            obj.UnfreezeObject(timePassingManager.GetLevelDuration());
        }
    }

    Robot JumpToEvent(int seekDirection)
    {
        Debug.Log($"Jumping in {seekDirection} direction");
        SceneInstant instant = timePassingManager.JumpToEvent(seekDirection);
        if (instant == null)
        {
            return GetActiveRobot();
        }
        Debug.Log($"Jumping to {timePassingManager.GetLevelDuration()}");
        foreach (InteractableObject obj in interactableObjects)
        {
            obj.JumpToInstant(timePassingManager.GetLevelDuration());
        }
        return instant.activeRobot;
    }

    Robot GetActiveRobot()
    {
        return activeRobot;
    }

    void SetActiveRobot(Robot robot)
    {
        activeRobot = robot;
    }

    void OnGUI()
    {
        string text = "";
        text += $"Duration {timePassingManager.GetLevelDuration():0.00} s\n";
        text += $"State {levelState}\n";
        GUIStyle labelStyle = new GUIStyle
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        Vector2 size = labelStyle.CalcSize(new GUIContent(text));

        Rect boxRect = new Rect(Screen.width - size.x - 2 * labelBorderSize, 0, size.x + 2 * labelBorderSize, size.y + 2 * labelBorderSize);
        Rect labelRect = new Rect(boxRect.x + labelBorderSize, boxRect.y + labelBorderSize, size.x, size.y);
        GUI.Box(boxRect, GUIContent.none);
        GUI.Label(labelRect, text, labelStyle);
    }
}