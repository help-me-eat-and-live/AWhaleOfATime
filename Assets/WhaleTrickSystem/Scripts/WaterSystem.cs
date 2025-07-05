using UnityEngine;
using System.Collections.Generic;

public class WaterSystem : MonoBehaviour
{
    [Header("Water Surface")]
    public Transform waterSurface; // Your water plane/mesh
    public float waterLevel = 0f; // Y position of water surface
    public LayerMask waterLayer = 1 << 4; // Water layer
    
    [Header("Splash Effects")]
    public GameObject splashPrefab;
    public ParticleSystem bubbleEffect;
    public float splashThreshold = 5f; // Minimum velocity for splash
    public float splashCooldown = 0.5f;
    
    [Header("Audio")]
    public AudioClip[] splashSounds;
    public AudioClip[] underwaterSounds;
    public AudioSource audioSource;
    
    [Header("Physics")]
    public float buoyancyForce = 10f;
    public float waterDensity = 1f;
    public AnimationCurve buoyancyCurve = AnimationCurve.Linear(0, 1, 1, 0);
    
    private Dictionary<Rigidbody, float> lastSplashTime = new Dictionary<Rigidbody, float>();
    private Dictionary<Rigidbody, bool> underwaterStatus = new Dictionary<Rigidbody, bool>();
    
    public static WaterSystem Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        
        // Set water level from surface transform if available
        if (waterSurface)
        {
            waterLevel = waterSurface.position.y;
        }
    }

    void Start()
    {
        SetupWaterCollider();
    }

    void SetupWaterCollider()
    {
        // Create a large trigger collider for water detection if none exists
        GameObject waterTrigger = GameObject.Find("WaterTrigger");
        if (!waterTrigger)
        {
            waterTrigger = new GameObject("WaterTrigger");
            waterTrigger.transform.position = new Vector3(0, waterLevel - 10f, 0);
            waterTrigger.layer = LayerMask.NameToLayer("Water");
            
            BoxCollider trigger = waterTrigger.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(200, 20, 200); // Large water volume
            
            WaterTrigger triggerScript = waterTrigger.AddComponent<WaterTrigger>();
            triggerScript.waterSystem = this;
            
            waterTrigger.tag = "Water";
        }
    }

    public bool IsUnderwater(Vector3 position)
    {
        return position.y < waterLevel;
    }

    public bool IsUnderwater(Transform transform)
    {
        return IsUnderwater(transform.position);
    }

    public float GetWaterDepth(Vector3 position)
    {
        return Mathf.Max(0, waterLevel - position.y);
    }

    public float GetDistanceToSurface(Vector3 position)
    {
        return waterLevel - position.y;
    }

    public void OnObjectEnterWater(Rigidbody rb, Vector3 entryPoint)
    {
        if (!rb) return;
        
        bool wasUnderwater = underwaterStatus.ContainsKey(rb) && underwaterStatus[rb];
        underwaterStatus[rb] = true;
        
        if (!wasUnderwater)
        {
            CreateSplashEffect(entryPoint, rb.linearVelocity);
            PlaySplashSound();
            
            // Notify whale controller if this is the whale
            WhaleTrickController whale = rb.GetComponent<WhaleTrickController>();
            if (whale)
            {
                // The whale controller will handle its own water entry logic
                Debug.Log("Whale entered water via trigger");
            }
        }
    }

    public void OnObjectExitWater(Rigidbody rb, Vector3 exitPoint)
    {
        if (!rb) return;
        
        bool wasUnderwater = underwaterStatus.ContainsKey(rb) && underwaterStatus[rb];
        underwaterStatus[rb] = false;
        
        if (wasUnderwater)
        {
            CreateSplashEffect(exitPoint, rb.linearVelocity);
            PlaySplashSound();
            
            // Notify whale controller if this is the whale
            WhaleTrickController whale = rb.GetComponent<WhaleTrickController>();
            if (whale)
            {
                Debug.Log("Whale exited water via trigger");
            }
        }
    }

    void CreateSplashEffect(Vector3 position, Vector3 velocity)
    {
        // Only create splash if velocity is high enough
        if (velocity.magnitude < splashThreshold) return;
        
        Vector3 splashPosition = new Vector3(position.x, waterLevel, position.z);
        
        if (splashPrefab)
        {
            GameObject splash = Instantiate(splashPrefab, splashPosition, Quaternion.identity);
            
            // Scale splash based on velocity
            float scale = Mathf.Clamp(velocity.magnitude / 10f, 0.5f, 3f);
            splash.transform.localScale = Vector3.one * scale;
            
            // Auto-destroy splash effect after 3 seconds
            Destroy(splash, 3f);
        }
        
        // Create bubble effect if available
        if (bubbleEffect)
        {
            bubbleEffect.transform.position = splashPosition;
            bubbleEffect.Play();
        }
    }

    void PlaySplashSound()
    {
        if (splashSounds.Length > 0 && audioSource)
        {
            AudioClip randomSplash = splashSounds[Random.Range(0, splashSounds.Length)];
            audioSource.PlayOneShot(randomSplash);
        }
    }

    // Apply buoyancy force to objects in water
    public void ApplyBuoyancy(Rigidbody rb)
    {
        if (!rb) return;
        
        float depth = GetWaterDepth(rb.position);
        if (depth <= 0) return;
        
        // Calculate buoyancy force based on depth
        float normalizedDepth = Mathf.Clamp01(depth / 5f); // Normalize over 5 units depth
        float buoyancyMultiplier = buoyancyCurve.Evaluate(normalizedDepth);
        
        Vector3 buoyancy = Vector3.up * buoyancyForce * buoyancyMultiplier * waterDensity;
        rb.AddForce(buoyancy, ForceMode.Acceleration);
    }

    // Method for other scripts to check water status
    public bool IsObjectUnderwater(Rigidbody rb)
    {
        return underwaterStatus.ContainsKey(rb) && underwaterStatus[rb];
    }

    // Visual debug
    void OnDrawGizmos()
    {
        // Draw water level
        Gizmos.color = new Color(0, 0.5f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position + Vector3.up * waterLevel, new Vector3(50, 0.1f, 50));
        
        // Draw water label
        Gizmos.color = Color.cyan;
        Vector3 labelPos = transform.position + Vector3.up * waterLevel + Vector3.forward * 10;
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(labelPos, $"Water Level: {waterLevel}");
        #endif
    }
}

// Separate script for the water trigger
public class WaterTrigger : MonoBehaviour
{
    [HideInInspector]
    public WaterSystem waterSystem;
    
    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb && waterSystem)
        {
            waterSystem.OnObjectEnterWater(rb, other.bounds.center);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb && waterSystem)
        {
            waterSystem.OnObjectExitWater(rb, other.bounds.center);
        }
    }
}
