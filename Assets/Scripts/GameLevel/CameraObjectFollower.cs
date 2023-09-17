using UnityEngine;

public class CameraObjectFollower : MonoBehaviour
{
    private Camera playerCam;
    [SerializeField] private GameObject followObject;
    [SerializeField] private float followMargin = 0.2f;
    [SerializeField] private float cameraFollowSpeed = 3.0f;
    [SerializeField] private float slowDownTime = 1.0f;
    private float slowDownTimer = 0.0f;
    private Vector3 cameraFollowOffset;


    // Start is called before the first frame update
    void Start()
    {
        playerCam = GetComponent<Camera>();
        cameraFollowOffset = transform.position - followObject.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 goalPosition = GetCameraGoal();
        Vector3 screenPos = playerCam.WorldToScreenPoint(goalPosition);
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
                goalPosition = transform.position;
            }
            slowDownFactor = 1.0f - (slowDownTimer / slowDownTime);
        }
        else
        {
            slowDownTimer = 0.0f;
        }

        float followDelta = slowDownFactor * cameraFollowSpeed * Time.fixedDeltaTime;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, goalPosition, followDelta);
        transform.position = smoothedPosition;
    }
    bool IsWithinMargin(float value, float maxValue)
    {
        value /= maxValue;
        return value > followMargin && value < 1.0f - followMargin;
    }

    private Vector3 GetCameraGoal()
    {
        return followObject.transform.position + cameraFollowOffset;
    }


    public void Recenter()
    {
        transform.position = GetCameraGoal();
    }

    public void SetFollowObject(GameObject followObject)
    {
        this.followObject = followObject;
    }
}
