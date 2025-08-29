using UnityEngine;

public class TouchOrbitCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;          // The point the camera orbits around
    public float distance = 5.0f;     // Initial distance from target
    public float minDistance = 2.0f;  // Minimum zoom distance
    public float maxDistance = 10.0f; // Maximum zoom distance

    [Header("Orbit Settings")]
    public float xSpeed = 120f;       // Speed of horizontal orbit
    public float ySpeed = 80f;        // Speed of vertical orbit
    public float yMinLimit = -20f;    // Minimum vertical angle
    public float yMaxLimit = 80f;     // Maximum vertical angle

    private float x = 0.0f;           // Current x rotation
    private float y = 0.0f;           // Current y rotation

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("No target assigned for TouchOrbitCamera.");
            enabled = false;
            return;
        }

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (Input.touchCount == 1) // Single touch to rotate
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                x += touch.deltaPosition.x * xSpeed * 0.02f * Time.deltaTime;
                y -= touch.deltaPosition.y * ySpeed * 0.02f * Time.deltaTime;

                y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
            }
        }
        else if (Input.touchCount == 2) // Two-finger pinch to zoom
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prevT0 = t0.position - t0.deltaPosition;
            Vector2 prevT1 = t1.position - t1.deltaPosition;

            float prevMag = (prevT0 - prevT1).magnitude;
            float currMag = (t0.position - t1.position).magnitude;

            float diff = prevMag - currMag;

            distance += diff * 0.01f; // Zoom sensitivity
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        // Mouse fallback for testing in editor
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
        }
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 2f, minDistance, maxDistance);
#endif

        // Apply rotation and position
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}
