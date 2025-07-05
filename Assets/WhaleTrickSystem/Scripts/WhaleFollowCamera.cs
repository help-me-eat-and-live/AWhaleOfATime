// Fixed WhaleFollowCamera.cs - Stationary behind whale
using UnityEngine;

public class WhaleFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform whaleTarget;
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Camera whaleCam;

    [Header("Zoom Settings")]
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float maxFOV = 85f;
    [SerializeField] private float zoomSmoothing = 2f;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 0.5f;

    private float currentFOV;
    private float shakeTime = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    private void Start()
    {
        if (!whaleCam)
        {
            whaleCam = GetComponent<Camera>();
            if (!whaleCam)
            {
                whaleCam = Camera.main;
                if (!whaleCam) Debug.LogWarning("WhaleFollowCamera: No Camera found on this GameObject. Assign a Camera in the Inspector.");
            }
        }

        if (whaleCam)
        {
            currentFOV = whaleCam.fieldOfView;
        }
    }

    private void LateUpdate()
    {
        if (!whaleTarget) return;

        // Follow whale position with fixed offset (no rotation following)
        Vector3 desiredPosition = whaleTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Always look at the whale (camera stays behind, looking forward)
        Vector3 lookDirection = whaleTarget.position - transform.position;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Apply camera shake if active
        UpdateCameraShake();
        
        UpdateFieldOfView();
    }

    private void UpdateCameraShake()
    {
        if (shakeTime > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.position += shakeOffset;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    private void UpdateFieldOfView()
    {
        if (!whaleCam || !whaleTarget) return;

        WhaleTrickController controller = whaleTarget.GetComponent<WhaleTrickController>();
        if (controller != null)
        {
            float speed = controller.GetComponent<Rigidbody>().linearVelocity.magnitude;
            float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speed / 20f);
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomSmoothing);
            whaleCam.fieldOfView = currentFOV;
        }
    }

    public void TriggerCameraShake()
    {
        shakeTime = shakeDuration;
    }
}
