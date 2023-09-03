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
    void Update()
    {
        Vector3 screenPos = playerCam.WorldToScreenPoint(followObject.transform.position);
        screenPos.x /= playerCam.pixelWidth;
        screenPos.y /= playerCam.pixelHeight;

        if (!IsWithinMargin(screenPos.x) || !IsWithinMargin(screenPos.y))
        {
            Matrix4x4 playerMat = Matrix4x4.TRS(followObject.transform.position, followObject.transform.rotation, Vector3.one);
            Matrix4x4 cameraMat = playerToCameraOffset * playerMat;

            float followDelta = cameraFollowSpeed * Time.deltaTime;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, cameraMat.GetT(), followDelta);
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, cameraMat.GetR(), followDelta);

            transform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
        }
    }

    bool IsWithinMargin(float value)
    {
        return value > followMargin && value < 1.0f - followMargin;
    }
}
