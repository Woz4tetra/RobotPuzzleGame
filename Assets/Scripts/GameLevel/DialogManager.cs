using UnityEngine;
class DialogManager : InteractionManager
{
    private bool isDialogActive = false;
    override protected void OnEnterInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} enter interacting");
    }

    override protected void OnInteracting(InteractableObjectInput objectInput)
    {

    }

    override protected void OnExitInteracting(InteractableObjectInput objectInput)
    {
        Debug.Log($"{gameObject.name} exit interacting");
    }

    override public bool IsInteracting()
    {
        return isDialogActive;
    }
}