using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public enum LevelState
    {
        Paused,
        Frozen,
        Moving,
        Seeking,
        Interacting
    };

    [SerializeField] private InteractableObject[] interactableObjects;
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
        interactionBroadcaster.SetInteractableObjects(interactableObjects);
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
            default:
                break;
        }

        timePassingManager.SeekTime(deltaTime);
        if (objectiveManager.IsObjectiveComplete())
        {
            // TODO show win screen
        }
    }

    Robot GetActiveRobot()
    {
        return activeRobot;
    }

    void SetActiveRobot(Robot robot)
    {
        foreach (InteractableObject obj in interactableObjects)
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