using UnityEngine;

public class CompassController : MonoBehaviour
{
    [Header("References")]
    public Transform whale;                    // Drag your whale here
    public Transform compassContainer;         // Drag the main Compass UI element
    
    [Header("Compass Settings")]
    public float smoothRotation = 5f;          // How smooth the compass rotation is
    public bool lockToNorth = true;            // If true, N always points north
    
    private float targetRotation = 0f;

    void Start()
    {
        // Try to find whale automatically if not assigned
        if (!whale)
        {
            WhaleTrickController whaleController = FindFirstObjectByType<WhaleTrickController>();
            if (whaleController)
                whale = whaleController.transform;
        }
        
        // Verify we have the compass
        if (!compassContainer)
        {
            Debug.LogWarning("CompassController: No compass container assigned!");
        }
    }

    void Update()
    {
        if (!whale || !compassContainer) return;
        
        UpdateCompassRotation();
    }

    void UpdateCompassRotation()
    {
        if (lockToNorth)
        {
            // Make compass rotate OPPOSITE to whale's rotation
            // This keeps N pointing north while whale turns
            targetRotation = -whale.eulerAngles.y;
        }
        else
        {
            // Alternative: Make compass show whale's heading
            targetRotation = whale.eulerAngles.y;
        }
        
        // Smooth rotation
        float currentZ = compassContainer.eulerAngles.z;
        float newZ = Mathf.LerpAngle(currentZ, targetRotation, smoothRotation * Time.deltaTime);
        
        compassContainer.rotation = Quaternion.Euler(0, 0, newZ);
    }
    
    // Public method to toggle compass behavior
    public void ToggleCompassMode()
    {
        lockToNorth = !lockToNorth;
        Debug.Log("Compass mode: " + (lockToNorth ? "True North" : "Whale Heading"));
    }
}
