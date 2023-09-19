using UnityEngine;

public class DestroyWhenAnimationComplete : MonoBehaviour
{
    public void AnimationComplete(string key)
    {
        Destroy(gameObject);
    }
}
