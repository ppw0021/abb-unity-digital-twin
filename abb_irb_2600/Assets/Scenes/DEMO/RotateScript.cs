using UnityEngine;

public class RotateScript : MonoBehaviour
{
[SerializeField] private bool shouldRotate = false;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // auto rotation speed
    [SerializeField] private float dragSensitivity = 0.5f; // adjust sensitivity

    private float lastX;

    void Update()
    {
        if (shouldRotate)
        {
            // Auto rotate
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
        else
        {
            HandleManualRotation();
        }
    }

    private void HandleManualRotation()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // Mouse input for editor/PC
        if (Input.GetMouseButtonDown(0))
        {
            lastX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = Input.mousePosition.x - lastX;
            transform.Rotate(Vector3.up, deltaX * dragSensitivity, Space.World);
            lastX = Input.mousePosition.x;
        }
#elif UNITY_ANDROID || UNITY_IOS
        // Touch input for mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                float deltaX = touch.deltaPosition.x;
                transform.Rotate(Vector3.up, deltaX * dragSensitivity, Space.World);
            }
        }
#endif
    }
}
