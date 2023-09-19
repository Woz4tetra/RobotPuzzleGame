using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DestroyWhenAnimationComplete : MonoBehaviour
{
    Animator animator;
    AnimationClip clip;

    void Awake()
    {
        animator = GetComponent<Animator>();
        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            clip = animator.runtimeAnimatorController.animationClips[i];

            AnimationEvent animationEndEvent = new AnimationEvent
            {
                time = clip.length,
                functionName = "AnimationCompleteHandler",
                stringParameter = clip.name
            };

            clip.AddEvent(animationEndEvent);
        }
    }

    public void AnimationCompleteHandler(string name)
    {
        GameObject parent = Helpers.GetParent(gameObject);
        if (parent == null)
        {
            return;
        }
        Debug.Log($"{name} animation complete. Removing {parent.name}");
        Destroy(parent);
        clip.events = new AnimationEvent[] { };
    }
}