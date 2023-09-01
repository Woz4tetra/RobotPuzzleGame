using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBallManager : MonoBehaviour
{
    private Rigidbody body;
    private Vector3 pausedVelocity = Vector3.zero;
    private Vector3 pausedAngularVelocity = Vector3.zero;
    private bool wasTimePassing = false;
    [SerializeField] private PassingTimeManager passingTimeManager;
    [SerializeField] private float forceMagnitude = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        bool isTimePassing = passingTimeManager.isTimePassing();

        if (wasTimePassing != isTimePassing)
        {
            if (!isTimePassing)
            {
                pausedVelocity = body.velocity;
                pausedAngularVelocity = body.angularVelocity;
                body.isKinematic = true;
                Debug.Log($"Paused velocity: {pausedVelocity}");
            }
            else
            {
                body.isKinematic = false;
                Debug.Log($"Unpaused velocity: {pausedVelocity}");
                body.velocity = pausedVelocity;
                body.angularVelocity = pausedAngularVelocity;
            }
        }
        wasTimePassing = isTimePassing;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            body.AddForce(Vector3.back * forceMagnitude, ForceMode.Impulse);
        }
    }
}
