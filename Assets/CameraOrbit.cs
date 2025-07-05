using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 10f;
    public float zoomSpeed = 2f;
    public float rotationSpeed = 100f;
    public Vector2 pitchLimits = new Vector2(-30f, 80f);

    private float yaw = 0f;
    private float pitch = 20f;

    void LateUpdate()
    {
        if (target == null) return;

        // Orbit with arrow keys or mouse
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        }
        else
        {
            yaw += Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            pitch -= Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
        }

        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        // Zoom
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, 2f, 50f);

        // Calculate position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
