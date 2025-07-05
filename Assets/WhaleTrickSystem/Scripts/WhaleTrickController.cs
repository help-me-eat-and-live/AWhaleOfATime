// Simplified WhaleTrickController.cs - No Animator
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WhaleTrickController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float underwaterSwimForce = 20f;
    [SerializeField] private float airMovementForce = 8f;
    // Removed unused turnSpeed and jumpForce - movement is handled directly via velocity
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float waterDrag = 5f;
    [SerializeField] private float airDrag = 0.5f;
    [SerializeField] private float waterSurfaceY = 0f;
    [SerializeField] private float buoyancyStrength = 10f;

    [Header("Trick System")]
    [SerializeField] private TrickRegistry trickRegistry;
    [SerializeField] private float comboResetTime = 3f;
    [SerializeField] private float minAirTimeForTricks = 0.3f;

    [Header("Effects")]
    [SerializeField] private GameObject splashEffect;
    [SerializeField] private GameObject trickEffect;
    [SerializeField] private AudioClip trickSound;
    [SerializeField] private AudioClip splashSound;

    [Header("Stamina System")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float jumpStaminaCost = 15f;

    // Private variables
    private Rigidbody rb;
    private bool isUnderwater = false;
    private bool wasUnderwater = false;
    private float airTime = 0f;
    private float currentStamina;
    private int score = 0;
    private int comboCount = 0;
    private float lastTrickTime = -Mathf.Infinity;
    private bool canPerformTricks = false;

    // Input
    private Vector3 inputDirection;
    private bool[] trickInputs = new bool[10];

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;
    }

    private void Update()
    {
        HandleInput();
        CheckUnderwaterStatus();
        UpdateStamina();
        UpdateAirTime();
        CheckTrickInputs();
        // Removed UpdateAnimator() call
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyBuoyancy();
        EnforceSpeedLimit();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float verticalMove = 0f;
        
        if (Input.GetKey(KeyCode.Space))
            verticalMove = 1f;
        else if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl))
            verticalMove = -1f;
            
        inputDirection = new Vector3(horizontal, verticalMove, vertical);
        
        for (int i = 0; i < trickInputs.Length; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;
            trickInputs[i] = Input.GetKeyDown(key);
        }
    }

    private void CheckUnderwaterStatus()
    {
        wasUnderwater = isUnderwater;
        isUnderwater = transform.position.y < waterSurfaceY;
        
        if (wasUnderwater != isUnderwater)
        {
            OnWaterStatusChanged();
        }
        
        UpdateDragSettings();
    }

    private void OnWaterStatusChanged()
    {
        if (isUnderwater)
        {
            CreateSplashEffect();
            ResetComboIfNeeded();
        }
        else
        {
            CreateSplashEffect();
            airTime = 0f;
        }
    }

    private void UpdateDragSettings()
    {
        rb.linearDamping = isUnderwater ? waterDrag : airDrag;
        rb.angularDamping = isUnderwater ? 2f : 1f;
    }

    private void ApplyMovement()
    {
        float moveSpeed = isUnderwater ? underwaterSwimForce : airMovementForce;
        
        Camera cam = Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>();
        
        Vector3 moveVector = Vector3.zero;
        
        if (cam != null && inputDirection.magnitude > 0.1f)
        {
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;
            
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            
            moveVector = (camForward * inputDirection.z) + (camRight * inputDirection.x) + (Vector3.up * inputDirection.y);
        }
        
        if (moveVector.magnitude > 0.1f)
        {
            Vector3 targetVelocity = moveVector * moveSpeed;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 8f * Time.fixedDeltaTime);
            
            Vector3 lookDirection = new Vector3(moveVector.x, 0f, moveVector.z);
            if (lookDirection.magnitude > 0.1f)
            {
                float targetYRotation = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, targetYRotation, 0f);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 2f * Time.fixedDeltaTime);
            }
        }
        
        if (inputDirection.y != 0f && HasStamina(jumpStaminaCost))
        {
            UseStamina(jumpStaminaCost * 0.1f * Time.fixedDeltaTime);
        }
    }

    private void ApplyBuoyancy()
    {
        if (isUnderwater)
        {
            float depth = waterSurfaceY - transform.position.y;
            float buoyancyForce = depth * buoyancyStrength;
            rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Force);
        }
    }

    private void EnforceSpeedLimit()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void UpdateAirTime()
    {
        if (!isUnderwater)
        {
            airTime += Time.deltaTime;
            canPerformTricks = airTime >= minAirTimeForTricks;
        }
        else
        {
            canPerformTricks = false;
        }
    }

    private void CheckTrickInputs()
    {
        if (!canPerformTricks || !trickRegistry || trickRegistry.tricks.Count == 0)
            return;

        for (int i = 0; i < trickInputs.Length && i < trickRegistry.tricks.Count; i++)
        {
            if (trickInputs[i])
            {
                AttemptTrick(trickRegistry.tricks[i]);
            }
        }
    }

    private void AttemptTrick(Trick trick)
    {
        if (Time.time - trick.lastUsedTime < trick.cooldown)
            return;

        if (!HasStamina(trick.staminaCost))
            return;

        PerformTrick(trick);
    }

    private void PerformTrick(Trick trick)
    {
        rb.AddTorque(trick.rotationAxis * trick.rotationAmount, ForceMode.VelocityChange);
        
        AddScore(trick.score);
        comboCount++;
        lastTrickTime = Time.time;
        
        UseStamina(trick.staminaCost);
        trick.lastUsedTime = Time.time;
        
        CreateTrickEffect();
        PlayTrickSound();
        
        WhaleFollowCamera camera = FindFirstObjectByType<WhaleFollowCamera>();
        if (camera)
        {
            camera.TriggerCameraShake();
        }
        
        Debug.Log($"Performed trick: {trick.trickName} (Score: {trick.score}, Combo: {comboCount}x)");
    }

    private void UpdateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
        
        if (Time.time - lastTrickTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    // Rest of the methods remain the same...
    private void CreateSplashEffect()
    {
        if (splashEffect)
        {
            Vector3 splashPos = new Vector3(transform.position.x, waterSurfaceY, transform.position.z);
            Instantiate(splashEffect, splashPos, Quaternion.identity);
        }
    }

    private void CreateTrickEffect()
    {
        if (trickEffect)
        {
            Instantiate(trickEffect, transform.position, transform.rotation);
        }
    }

    private void PlayTrickSound()
    {
        if (trickSound)
        {
            AudioSource.PlayClipAtPoint(trickSound, transform.position);
        }
    }

    private void AddScore(int points)
    {
        int comboMultiplier = Mathf.Max(1, comboCount);
        score += points * comboMultiplier;
    }

    private void ResetCombo()
    {
        if (comboCount > 0)
        {
            comboCount = 0;
            Debug.Log("Combo reset!");
        }
    }

    private void ResetComboIfNeeded()
    {
        if (Time.time - lastTrickTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    private bool HasStamina(float cost)
    {
        return currentStamina >= cost;
    }

    private void UseStamina(float cost)
    {
        currentStamina -= cost;
        currentStamina = Mathf.Max(0, currentStamina);
    }

    // Public getters
    public float GetStaminaPercent() => currentStamina / maxStamina;
    public int GetScore() => score;
    public int GetComboCount() => comboCount;
    public bool IsUnderwater() => isUnderwater;
    public float GetAirTime() => airTime;
}