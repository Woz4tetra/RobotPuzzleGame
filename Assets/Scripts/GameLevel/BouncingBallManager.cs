using UnityEngine;

namespace GameLevel
{
    public class BouncingBallManager : MonoBehaviour
    {
        private Rigidbody body;
        [SerializeField] private float forceMagnitude = 10.0f;
        // Start is called before the first frame update
        void Start()
        {
            body = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Floor"))
            {
                body.AddForce(Vector3.back * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}