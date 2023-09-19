using UnityEngine;

public class LockSpriteToCamera : MonoBehaviour
{
    [SerializeField] private Camera cameraObject;
    void Update()
    {
        transform.rotation = cameraObject.transform.rotation;
    }
}