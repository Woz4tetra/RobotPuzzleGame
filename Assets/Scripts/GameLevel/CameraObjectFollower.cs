using Unity.VisualScripting;
using UnityEngine;

public class CameraObjectFollower : MonoBehaviour
{
    private Matrix4x4 playerToCameraOffset;
    private Camera playerCam;
    [SerializeField] private GameObject followObject;
    [SerializeField] private float followMargin = 0.2f;
    [SerializeField] private float cameraFollowSpeed = 3.0f;


    // Start is called before the first frame update
    void Start()
    {
        playerCam = GetComponent<Camera>();
        Matrix4x4 playerMat = Matrix4x4.TRS(followObject.transform.position, followObject.transform.rotation, Vector3.one);
        Matrix4x4 cameraMat = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        playerToCameraOffset = playerMat.inverse * cameraMat;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 screenPos = playerCam.WorldToScreenPoint(followObject.transform.position);
        screenPos.x /= playerCam.pixelWidth;
        screenPos.y /= playerCam.pixelHeight;

        if (!IsWithinMargin(screenPos.x) || !IsWithinMargin(screenPos.y))
        {
            (Vector3, Quaternion) cameraPose = ComputeCameraPose();
            float followDelta = cameraFollowSpeed * Time.fixedDeltaTime;
            Vector3 smoothedPosition = Vector3.Slerp(transform.position, cameraPose.Item1, followDelta);
            Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, cameraPose.Item2, followDelta);
            transform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
        }
    }

    bool IsWithinMargin(float value)
    {
        return value > followMargin && value < 1.0f - followMargin;
    }

    (Vector3, Quaternion) ComputeCameraPose()
    {
        Matrix4x4 playerMat = Matrix4x4.TRS(followObject.transform.position, followObject.transform.rotation, Vector3.one);
        Matrix4x4 cameraMat = playerToCameraOffset * playerMat;
        return (cameraMat.GetT(), cameraMat.GetR());
    }

    public void Recenter()
    {
        (Vector3, Quaternion) cameraPose = ComputeCameraPose();
        transform.SetPositionAndRotation(cameraPose.Item1, cameraPose.Item2);
    }

    public void SetFollowObject(GameObject followObject)
    {
        this.followObject = followObject;
    }
}
