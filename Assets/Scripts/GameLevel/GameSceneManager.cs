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
        Seeking,
        Reset,
        GameOver,
        GameWin
    };

    [SerializeField] private TimePassingManager timePassingManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PauseMenuManager pauseMenuManager;
    [SerializeField] private ObjectiveManager objectiveManager;
    [SerializeField] private CameraObjectFollower cameraFollower;
    [SerializeField] private InteractableObjectManager interactableObjectManager;
    [SerializeField] private ActingManager actingManager;
    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private SwitchRobotManager switchRobotManager;
    [SerializeField] private float seekAnimationMultiplier = 2.0f;

    private int labelBorderSize = 5;

    private LevelState levelState = LevelState.Start;
    private LevelState nextState = LevelState.Start;
    private LevelState beforePauseState = LevelState.Frozen;
    private SeekDestination seekDestination = null;
    private float seekStartDuration = 0.0f;

    // ---
    // Game object methods
    // ---

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        nextState = GetActiveState(levelState);
    }

    void FixedUpdate()
    {
        StateUpdate(nextState, levelState);
        levelState = nextState;
    }

    // ---
    // State machine methods
    // ---

    LevelState GetActiveState(LevelState prevState)
    {
        LevelState state = prevState;
        if (prevState == LevelState.Start)
        {
            state = LevelState.Frozen;
        }
        else if (inputManager.IsReset())
        {
            state = LevelState.Reset;
        }
        else if (inputManager.PauseToggled())
        {
            if (prevState == LevelState.Paused)
            {
                Debug.Log($"Unpausing from {beforePauseState}");
                Time.timeScale = 1.0f;
                state = beforePauseState;
            }
            else
            {
                Debug.Log($"Pausing. Previous state is {prevState}");
                beforePauseState = prevState;
                state = LevelState.Paused;
            }
        }
        else if (prevState != LevelState.Paused)
        {
            if (seekDestination != null)
            {
                state = LevelState.Seeking;
            }
            else if (objectiveManager.IsObjectiveComplete())
            {
                state = LevelState.GameWin;
            }
            else if (objectiveManager.IsObjectiveFailed())
            {
                state = LevelState.GameOver;
            }
            else if (AnyInteracting())
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
            if ((prevState == LevelState.Start || prevState == LevelState.Moving) && newState != LevelState.Paused)
            {
                RecordSceneEvent(interactableObjectManager.GetActiveRobot());
            }
            OnStateExit(prevState);
            OnStateEnter(newState);
        }
        float deltaTime = 0.0f;
        Robot robot = interactableObjectManager.GetActiveRobot();
        Robot switchedRobot = robot;
        float levelDuration = timePassingManager.GetLevelDuration();
        int seekDirection = 0;
        SceneInstant seekGoal = null;
        Time.timeScale = 1.0f;
        InteractableObjectInput input = inputManager.GetInteractionStruct();
        switch (newState)
        {
            case LevelState.Moving:
                deltaTime = Time.deltaTime;
                seekDirection = inputManager.GetSeekDirection();
                UpdateMovingInteractions(input);
                interactableObjectManager.RecordObjectEvent();
                break;
            case LevelState.Paused:
                Time.timeScale = 0.0f;
                break;
            case LevelState.Frozen:
                seekDirection = inputManager.GetSeekDirection();
                UpdateFrozenInteractions(input);
                Robot nextRobot = switchRobotManager.GetNextActiveRobot();
                if (nextRobot != null)
                {
                    switchedRobot = nextRobot;
                }
                break;
            case LevelState.Reset:
                seekDirection = -1;
                seekGoal = timePassingManager.GetFirstInstant();
                break;
            case LevelState.Seeking:
                if (seekDestination != null)
                {
                    if (IsSeekDestinationReached())
                    {
                        switchedRobot = OnSeekExit();
                        Debug.Log($"Seeking complete. Switched to {switchedRobot.gameObject.name}");
                    }
                    else
                    {
                        float exponentialFactor = Mathf.Abs(seekStartDuration - levelDuration);
                        float baseFactor = seekAnimationMultiplier * Time.deltaTime;
                        deltaTime = seekDestination.direction * Mathf.Max(baseFactor, baseFactor * exponentialFactor);
                        interactableObjectManager.JumpToObjectInstant(levelDuration + deltaTime);
                    }
                }
                break;

            case LevelState.GameOver:
                // TODO show game over screen
                seekDirection = inputManager.GetSeekDirection();
                break;
            case LevelState.GameWin:
                // TODO show win screen
                seekDirection = inputManager.GetSeekDirection();
                break;
            default:
                break;
        }
        if (seekDirection != 0)
        {
            OnSeekEnter(seekDirection, seekGoal);
        }
        if (switchedRobot != robot)
        {
            FollowRobot(switchedRobot);
        }

        timePassingManager.MoveByDelta(deltaTime);
    }

    void OnStateEnter(LevelState state)
    {
        Debug.Log($"Entering state {state}");
        switch (state)
        {
            case LevelState.Moving:
                interactableObjectManager.UnfreezeObjects();
                break;
            case LevelState.Frozen:
                interactableObjectManager.FreezeObjects();
                break;
            case LevelState.GameOver:
                interactableObjectManager.FreezeObjects();
                CancelInteractions();
                break;
            case LevelState.GameWin:
                interactableObjectManager.FreezeObjects();
                CancelInteractions();
                break;
            case LevelState.Paused:
                interactableObjectManager.FreezeObjects();
                pauseMenuManager.OnActiveChange(true);
                break;
            case LevelState.Reset:
                CancelInteractions();
                break;
            case LevelState.Seeking:
                CancelInteractions();
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
            case LevelState.Start:
                QueueDialog();
                break;
            default:
                break;
        }

    }

    // ---
    // Seeking events
    // ---

    void OnSeekEnter(int seekDirection, SceneInstant goalInstant = null)
    {
        SceneInstant instant;
        if (goalInstant == null)
        {
            instant = timePassingManager.GetInstant(seekDirection);
        }
        else
        {
            instant = goalInstant;
        }
        if (instant == null)
        {
            if (seekDirection < 0)
            {
                instant = timePassingManager.GetFirstInstant();
            }
            else
            {
                instant = timePassingManager.GetLastInstant();
            }
        }
        if (instant == null)
        {
            Debug.Log("No scene instant to seek to");
            return;
        }

        Debug.Log($"Seeking {seekDirection} to {instant.levelDuration}");
        SetSeekDestination(new SeekDestination
        {
            goal = instant,
            direction = seekDirection
        });
    }

    Robot OnSeekExit()
    {
        Robot switchedRobot = JumpToSceneInstant(seekDestination);
        if (seekDestination.goal.levelDuration == 0.0f)
        {
            CenterCameraOnRobot();
        }
        interactableObjectManager.FreezeObjects();
        switchRobotManager.ResetNearbyRobot();
        seekDestination = null;
        return switchedRobot;
    }

    // ---
    // Seeking destination
    // ---

    void SetSeekDestination(SeekDestination destination)
    {
        seekDestination = destination;
        seekStartDuration = timePassingManager.GetLevelDuration();
        Debug.Log($"Seek started at {seekStartDuration}");
    }

    bool IsSeekDestinationReached()
    {
        float levelDuration = timePassingManager.GetLevelDuration();
        bool destinationReached;
        if (levelDuration == 0.0f && seekDestination.goal.levelDuration == 0.0f)
        {
            Debug.Log("Level duration is 0. Forcing destination reached");
            destinationReached = true;
        }
        else
        {
            bool isBelowDestination = levelDuration < seekDestination.goal.levelDuration;
            destinationReached = isBelowDestination;
            if (seekDestination.direction > 0)
            {
                destinationReached = !destinationReached;
            }
        }
        return destinationReached;
    }

    // ---
    // Camera follower
    // ---

    void FollowRobot(Robot robot)
    {
        interactableObjectManager.SetActiveRobot(robot);
        cameraFollower.SetFollowObject(robot.gameObject);
        CenterCameraOnRobot();
    }

    void CenterCameraOnRobot()
    {
        cameraFollower.Recenter();
    }


    // ---
    // Record history event
    // ---

    void RecordSceneEvent(Robot activeRobot)
    {
        Debug.Log("Recording event");
        timePassingManager.RecordEvent(new SceneInstant
        {
            activeRobot = activeRobot,
            levelDuration = timePassingManager.GetLevelDuration()
        });
        interactableObjectManager.RecordObjectEvent();
    }

    // ---
    // Jump to a scene instant
    // ---

    Robot JumpToSceneInstant(SeekDestination destination)
    {
        float levelDuration = destination.goal.levelDuration;
        Debug.Log($"Jumping to {levelDuration} in the {destination.direction} direction");
        timePassingManager.JumpToInstant(destination.goal);
        interactableObjectManager.JumpToObjectInstant(levelDuration);
        return destination.goal.activeRobot;
    }

    // ---
    // Interactions
    // ---

    bool AnyInteracting()
    {
        return actingManager.IsInteracting() || dialogManager.IsInteracting();
    }

    void UpdateFrozenInteractions(InteractableObjectInput input)
    {
        dialogManager.UpdateInteraction(input, input.ShouldDialog());
        if (dialogManager.IsInteracting())
        {
            return;
        }
        switchRobotManager.UpdateInteraction(input, input.ShouldSwitch());
        if (switchRobotManager.IsInteracting())
        {
            return;
        }
        actingManager.UpdateInteraction(input, input.ShouldAct());
    }
    void UpdateMovingInteractions(InteractableObjectInput input)
    {
        dialogManager.UpdateInteraction(input, input.ShouldDialog());
        if (dialogManager.IsInteracting())
        {
            return;
        }
        actingManager.UpdateInteraction(input, input.ShouldAct());
    }

    void QueueDialog()
    {
        dialogManager.SetDialog(new Conversation(new string[] { "Hello", "World" }));
    }

    void CancelInteractions()
    {
        InteractableObjectInput input = new InteractableObjectInput();
        dialogManager.UpdateInteraction(input, input.ShouldDialog());
        switchRobotManager.UpdateInteraction(input, input.ShouldSwitch());
        actingManager.UpdateInteraction(input, input.ShouldAct());
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