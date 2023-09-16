using Unity.VisualScripting;
using UnityEngine;

public class CameraObjectFollower : MonoBehaviour
{
    private Matrix4x4 playerToCameraOffset;
    private Camera playerCam;
    [SerializeField] private GameObject followObject;
    [SerializeField] private float followMargin = 0.2f;
    [SerializeField] private float cameraFollowSpeed = 3.0f;
    [SerializeField] private float slowDownTime = 1.0f;
    private float slowDownTimer = 0.0f;


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
        (Vector3, Quaternion) cameraPose = ComputeCameraPose();

        Vector3 screenPos = playerCam.WorldToScreenPoint(cameraPose.Item1);
        Vector3 cameraPositionGoal = cameraPose.Item1;
        float slowDownFactor = 1.0f;
        if (IsWithinMargin(screenPos.x, playerCam.pixelWidth) && IsWithinMargin(screenPos.y, playerCam.pixelHeight))
        {
            if (slowDownTimer == 0.0f)
            {
                slowDownTimer = Time.fixedDeltaTime;
            }
            else
            {
                slowDownTimer = Mathf.Min(slowDownTimer + Time.fixedDeltaTime, slowDownTime);
            }
            if (slowDownTimer >= slowDownTime)
            {
                cameraPositionGoal = transform.position;
            }
            slowDownFactor = 1.0f - (slowDownTimer / slowDownTime);
        }
        else
        {
            slowDownTimer = 0.0f;
        }

        Quaternion cameraRotationGoal = cameraPose.Item2;

        float followDelta = slowDownFactor * cameraFollowSpeed * Time.fixedDeltaTime;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, cameraPositionGoal, followDelta);
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, cameraRotationGoal, followDelta);
        transform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
    }
    bool IsWithinMargin(float value, float maxValue)
    {
        value /= maxValue;
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
