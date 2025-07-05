using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAutoFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public string whaleTag = "Player";
    public Vector3 offset = new Vector3(0f, 5f, -12f);
    public float followSpeed = 8f;
    public float rotationSpeed = 4f;

    [Header("Zoom Settings")]
    public float baseFOV = 60f;
    public float maxFOV = 85f;
    public float zoomSmoothing = 2f;

    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.5f;

    private Transform whaleTarget;
    private Camera cam;
    private float currentFOV;
    private float shakeTime = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    void Start()
    {
        cam = GetComponent<Camera>();
        currentFOV = cam.fieldOfView;

        GameObject whaleGO = GameObject.FindGameObjectWithTag(whaleTag);
        if (whaleGO != null)
        {
            whaleTarget = whaleGO.transform;
            Debug.Log("🎯 CameraAutoFollow: Found whale target: " + whaleTarget.name);
        }
        else
        {
            Debug.LogWarning("❌ CameraAutoFollow: No object tagged '" + whaleTag + "' found!");
        }
    }

    void LateUpdate()
    {
        if (!whaleTarget) return;

        // Smooth position follow
        Vector3 desiredPos = whaleTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos + shakeOffset, followSpeed * Time.deltaTime);

        // Smooth rotation to look at whale
        Vector3 lookDir = whaleTarget.position - transform.position;
        if (lookDir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(lookDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        UpdateCameraShake();
        UpdateZoomFOV();
    }

    void UpdateCameraShake()
    {
        if (shakeTime > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    void UpdateZoomFOV()
    {
        if (!cam || !whaleTarget) return;

        Rigidbody rb = whaleTarget.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float speed = rb.linearVelocity.magnitude;
            float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speed / 20f);
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomSmoothing);
            cam.fieldOfView = currentFOV;
        }
    }

    public void TriggerShake()
    {
        shakeTime = shakeDuration;
    }
}
