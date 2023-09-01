using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAgent : MonoBehaviour
{
    private Rigidbody body;
    private Matrix4x4 playerToCameraOffset;
    private GUIStyle labelStyle;
    private int labelBorderSize = 5;
    [SerializeField] private PassingTimeManager passingTimeManager;
    [SerializeField] private Camera playerCam;
    [SerializeField] private float forceMagnitude = 10.0f;
    [SerializeField] private float followMargin = 0.2f;
    [SerializeField] private float cameraFollowSpeed = 3.0f;
    [SerializeField] private float movementThreshold = 0.01f;


    // Start is called before the first frame update
    void Start()
    {
        labelStyle = new GUIStyle
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        body = GetComponent<Rigidbody>();
        Matrix4x4 playerMat = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Matrix4x4 cameraMat = Matrix4x4.TRS(playerCam.transform.position, playerCam.transform.rotation, Vector3.one);
        playerToCameraOffset = playerMat.inverse * cameraMat;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Vertical") * forceMagnitude;
        float vertical = Input.GetAxis("Horizontal") * forceMagnitude;
        body.velocity = new Vector3(vertical, horizontal, 0.0f);
        Vector3 screenPos = playerCam.WorldToScreenPoint(transform.position);
        screenPos.x /= playerCam.pixelWidth;
        screenPos.y /= playerCam.pixelHeight;

        if (!IsWithinMargin(screenPos.x) || !IsWithinMargin(screenPos.y))
        {
            Matrix4x4 playerMat = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Matrix4x4 cameraMat = playerToCameraOffset * playerMat;

            float followDelta = cameraFollowSpeed * Time.deltaTime;
            Vector3 smoothedPosition = Vector3.Lerp(playerCam.transform.position, cameraMat.GetT(), followDelta);
            Quaternion smoothedRotation = Quaternion.Lerp(playerCam.transform.rotation, cameraMat.GetR(), followDelta);

            playerCam.transform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
        }

        passingTimeManager.setTimePassing(body.velocity.magnitude > movementThreshold);
    }
    void OnGUI()
    {
        string text = $"Duration {passingTimeManager.getDuration():0.00}";
        Vector2 size = labelStyle.CalcSize(new GUIContent(text));

        Rect boxRect = new Rect(Screen.width - size.x - 2 * labelBorderSize, 0, size.x + 2 * labelBorderSize, size.y + 2 * labelBorderSize);
        Rect labelRect = new Rect(boxRect.x + labelBorderSize, boxRect.y + labelBorderSize, size.x, size.y);
        GUI.Box(boxRect, GUIContent.none);
        GUI.Label(labelRect, text, labelStyle);
    }
    bool IsWithinMargin(float value)
    {
        return value > followMargin && value < 1.0f - followMargin;
    }
}
