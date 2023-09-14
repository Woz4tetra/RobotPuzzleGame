using UnityEngine;
abstract class InteractionManager : MonoBehaviour
{
    [SerializeField] protected InteractableObjectManager interactableObjectManager;
    private bool wasInteracting = false;
    private InteractableObjectInput lastObjectInput;
    public enum InteractionState
    {
        Enter,
        Interacting,
        Exit,
        Idle
    }
    InteractionState state = InteractionState.Idle;

    public void UpdateInteraction(InteractableObjectInput objectInput, bool isInteracting)
    {
        state = InteractionState.Idle;
        if (isInteracting != wasInteracting)
        {
            if (isInteracting)
            {
                state = InteractionState.Enter;
            }
            else
            {
                state = InteractionState.Exit;
            }
        }
        wasInteracting = isInteracting;
        if (isInteracting && state == InteractionState.Idle)
        {
            state = InteractionState.Interacting;
            lastObjectInput = objectInput;
        }
        switch (state)
        {
            case InteractionState.Enter:
                OnEnterInteracting(objectInput);
                break;
            case InteractionState.Interacting:
                OnInteracting(objectInput);
                break;
            case InteractionState.Exit:
                OnExitInteracting(lastObjectInput);
                break;
            default:
                break;
        }
    }

    abstract public bool IsInteracting();

    protected InteractionState GetState()
    {
        return state;
    }

    abstract protected void OnEnterInteracting(InteractableObjectInput objectInput);
    abstract protected void OnInteracting(InteractableObjectInput objectInput);
    abstract protected void OnExitInteracting(InteractableObjectInput objectInput);
}