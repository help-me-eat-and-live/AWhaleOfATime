using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [Header("References")]
    public Transform whale;           // Drag whale here
    
    [Header("Follow Settings")]
    public float followHeight = 100f; // How high above whale
    public float followSpeed = 5f;    // How smoothly it follows
    
    void Start()
    {
        // Try to find whale automatically
        if (!whale)
        {
            WhaleTrickController whaleController = FindFirstObjectByType<WhaleTrickController>();
            if (whaleController)
                whale = whaleController.transform;
        }
        
        if (!whale)
        {
            Debug.LogWarning("MinimapFollow: No whale found!");
        }
    }
    
    void LateUpdate()
    {
        if (!whale) return;
        
        // Follow whale position but stay at fixed height
        Vector3 targetPosition = new Vector3(whale.position.x, followHeight, whale.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
