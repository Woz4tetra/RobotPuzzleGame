using UnityEngine;
using UnityEngine.SceneManagement;

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
            - InteractionBroadcaster
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
            - Point InteractionBroadcaster to InteractionBroadcaster object
            - Point ObjectiveManager to ObjectiveManager object
            - Point Active Robot to initial Robot
            - Point CameraFollower to CameraFollower object
        Edit ObjectiveManager:
            - Add all objective criteria in the scene to "Criteria"
     */
    public enum LevelState
    {
        Paused,
        Frozen,
        Moving,
        Seeking,
        Interacting,
        GameOver,
        GameWin,
        Reset
    };

    [SerializeField] private InteractableObject[] controllableObjects;
    [SerializeField] private TimePassingManager timePassingManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PauseMenuManager pauseMenuManager;
    [SerializeField] private InteractionBroadcaster interactionBroadcaster;
    [SerializeField] private ObjectiveManager objectiveManager;
    [SerializeField] private Robot activeRobot;
    [SerializeField] private CameraObjectFollower cameraFollower;
    [SerializeField] private float resetSpeedMultiplier = 10.0f;

    private int labelBorderSize = 5;

    private LevelState levelState = LevelState.Frozen;
    private LevelState beforePauseState = LevelState.Frozen;
    private float realTimeStart = 0.0f;
    private Robot initialActiveRobot;

    void Start()
    {
        initialActiveRobot = activeRobot;
        activeRobot.SetActive(true);
        interactionBroadcaster.SetInteractableObjects(controllableObjects);
        realTimeStart = Time.realtimeSinceStartup;
    }

    void Update()
    {
        StateMachineUpdate();
    }

    void StateMachineUpdate()
    {
        LevelState newState = GetActiveState(levelState);
        OnStateUpdate(newState, levelState);
        levelState = newState;
    }

    LevelState GetActiveState(LevelState prevState)
    {
        LevelState state = prevState;
        if (inputManager.PauseToggled())
        {
            if (state == LevelState.Paused)
            {
                state = beforePauseState;
            }
            else
            {
                beforePauseState = state;
                state = LevelState.Paused;
            }
        }
        else if (state != LevelState.Paused)
        {
            if (inputManager.IsReset())
            {
                state = LevelState.Reset;
            }
            if (state == LevelState.Reset && timePassingManager.GetDuration() > 0.0f)
            {
                return state;
            }
            state = GetActiveStateGameActive(state);
            state = GetActiveStateObjectiveCriteria(state);
        }
        return state;
    }

    LevelState GetActiveStateGameActive(LevelState prevState)
    {
        LevelState state = prevState;
        if (inputManager.InteractToggled() && state != LevelState.Moving)
        {
            if (state == LevelState.Interacting &&
                interactionBroadcaster.IsInteractionObjectRobotOrNull())
            {
                state = LevelState.Frozen;
            }
            else
            {
                state = LevelState.Interacting;
            }
        }
        else if (state != LevelState.Interacting)
        {
            if (inputManager.SeekToggled())
            {
                state = LevelState.Seeking;
            }
            else if (inputManager.MoveToggled())
            {
                state = LevelState.Moving;
            }
            else
            {
                state = LevelState.Frozen;
            }
        }
        return state;
    }

    LevelState GetActiveStateObjectiveCriteria(LevelState prevState)
    {
        LevelState state = prevState;
        if (objectiveManager.IsObjectiveComplete())
        {
            state = LevelState.GameWin;
        }
        else if (objectiveManager.IsObjectiveFailed())
        {
            if (state == LevelState.Seeking && inputManager.GetSeekDirection() < 0)
            {
                return state;
            }
            state = LevelState.GameOver;
        }
        return state;
    }

    void OnStateUpdate(LevelState newState, LevelState prevState)
    {
        float deltaTime = 0.0f;
        if (newState != prevState)
        {
            switch (newState)
            {
                case LevelState.Seeking: goto case LevelState.Frozen;
                case LevelState.Moving: goto case LevelState.Frozen;
                case LevelState.Paused: goto case LevelState.Frozen;
                case LevelState.Frozen:
                    cameraFollower.SetFollowObject(GetActiveRobot().gameObject);
                    break;
                case LevelState.Interacting:
                    cameraFollower.SetFollowObject(interactionBroadcaster.gameObject);
                    break;
                default:
                    break;
            }
            interactionBroadcaster.OnActiveChange(newState == LevelState.Interacting, GetActiveRobot().GetPosition());
            pauseMenuManager.OnActiveChange(newState == LevelState.Paused);
        }
        switch (newState)
        {
            case LevelState.Seeking:
                deltaTime = inputManager.GetSeekDirection() * Time.deltaTime;
                break;
            case LevelState.Moving:
                GetActiveRobot().Interact(inputManager.GetInteractionStruct());
                deltaTime = Time.deltaTime;
                break;
            case LevelState.Interacting:
                interactionBroadcaster.MovePointer(inputManager.GetInteractionStruct());
                if (interactionBroadcaster.IsInteractionObjectRobot())
                {
                    SetActiveRobot((Robot)interactionBroadcaster.GetInteractionObject());
                }
                break;
            case LevelState.Paused:
                break;
            case LevelState.Frozen:
                deltaTime = 0.0f;
                break;
            case LevelState.GameOver:
                // TODO show game over screen
                break;
            case LevelState.GameWin:
                // TODO show win screen
                break;
            case LevelState.Reset:
                deltaTime = -Time.deltaTime * resetSpeedMultiplier;
                SetActiveRobot(initialActiveRobot);
                break;
            default:
                break;
        }

        timePassingManager.SeekTime(deltaTime);
    }

    Robot GetActiveRobot()
    {
        return activeRobot;
    }

    void SetActiveRobot(Robot robot)
    {
        foreach (InteractableObject obj in controllableObjects)
        {
            if (!typeof(Robot).IsInstanceOfType(obj))
            {
                continue;
            }
            obj.SetActive(obj == robot);
            if (obj == robot)
            {
                activeRobot = robot;
            }
        }
    }

    public float GetEnergySpent()
    {
        // "Energy" is really just the time spent in the level
        return Time.realtimeSinceStartup - realTimeStart;
    }

    void OnGUI()
    {
        string text = "";
        text += $"Duration {timePassingManager.GetDuration():0.00} s\n";
        text += $"Energy {GetEnergySpent():0.00} mJ\n";
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