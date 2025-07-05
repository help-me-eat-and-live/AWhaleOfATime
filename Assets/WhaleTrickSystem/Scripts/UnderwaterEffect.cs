using UnityEngine;
using UnityEngine.UI;

public class UnderwaterEffect : MonoBehaviour
{
    [Header("References")]
    public Transform whale;                    // Drag your whale here
    public Image underwaterOverlay;           // Drag UnderwaterOverlay here
    public float waterSurfaceY = 0f;          // Water level height
    
    [Header("Effect Settings")]
    public float fadeSpeed = 3f;              // How fast the effect fades in/out
    public float maxOpacity = 0.3f;           // Maximum blue tint strength
    
    private Color originalColor;
    private Color transparentColor;
    private bool isUnderwater = false;
    private float targetAlpha = 0f;

    void Start()
    {
        if (underwaterOverlay)
        {
            originalColor = underwaterOverlay.color;
            transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            
            // Start with no overlay
            underwaterOverlay.color = transparentColor;
        }
        
        if (!whale)
        {
            // Try to find whale automatically
WhaleTrickController whaleController = FindFirstObjectByType<WhaleTrickController>();            if (whaleController)
                whale = whaleController.transform;
        }
    }

    void Update()
    {
        if (!whale || !underwaterOverlay) return;
        
        CheckUnderwaterStatus();
        UpdateOverlayEffect();
    }

    void CheckUnderwaterStatus()
    {
        bool wasUnderwater = isUnderwater;
        isUnderwater = whale.position.y < waterSurfaceY;
        
        // Set target opacity based on underwater status
        targetAlpha = isUnderwater ? maxOpacity : 0f;
        
        // Optional: Log status changes for debugging
        if (wasUnderwater != isUnderwater)
        {
            Debug.Log(isUnderwater ? "Entered underwater" : "Surfaced");
        }
    }

    void UpdateOverlayEffect()
    {
        // Smoothly transition the overlay opacity
        Color currentColor = underwaterOverlay.color;
        float newAlpha = Mathf.Lerp(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        
        underwaterOverlay.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
        
        // Enable/disable the overlay for performance
        if (newAlpha <= 0.01f && underwaterOverlay.gameObject.activeSelf)
        {
            underwaterOverlay.gameObject.SetActive(false);
        }
        else if (newAlpha > 0.01f && !underwaterOverlay.gameObject.activeSelf)
        {
            underwaterOverlay.gameObject.SetActive(true);
        }
    }

    // Public method to manually set water level
    public void SetWaterLevel(float newWaterLevel)
    {
        waterSurfaceY = newWaterLevel;
    }
    
    // Public method to adjust effect intensity
    public void SetMaxOpacity(float opacity)
    {
        maxOpacity = Mathf.Clamp01(opacity);
    }
}
