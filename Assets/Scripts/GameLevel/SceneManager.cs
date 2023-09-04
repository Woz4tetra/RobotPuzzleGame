using UnityEngine;

public class SceneManager : MonoBehaviour
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
     */
    public enum LevelState
    {
        Paused,
        Frozen,
        Moving,
        Seeking,
        Interacting,
        GameOver,
        GameWin
    };

    [SerializeField] private InteractableObject[] controllableObjects;
    [SerializeField] private TimePassingManager timePassingManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PauseMenuManager pauseMenuManager;
    [SerializeField] private InteractionBroadcaster interactionBroadcaster;
    [SerializeField] private ObjectiveManager objectiveManager;
    [SerializeField] private Robot activeRobot;
    [SerializeField] private CameraObjectFollower cameraFollower;

    private int labelBorderSize = 5;

    private LevelState levelState = LevelState.Frozen;

    void Start()
    {
        activeRobot.SetActive(true);
        interactionBroadcaster.SetInteractableObjects(controllableObjects);
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
                state = LevelState.Frozen;
            }
            else
            {
                state = LevelState.Paused;
            }
        }
        else if (state != LevelState.Paused)
        {
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

    void OnGUI()
    {
        string text = "";
        text += $"Duration {timePassingManager.GetDuration():0.00}\n";
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