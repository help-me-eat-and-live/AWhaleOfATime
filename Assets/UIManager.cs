// Enhanced UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private TextMeshProUGUI airTimeText;
    [SerializeField] private TextMeshProUGUI depthText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Combo Display")]
    [SerializeField] private GameObject comboPanel;
    [SerializeField] private TextMeshProUGUI comboMultiplierText;
    [SerializeField] private float comboDisplayTime = 2f;

    [Header("Trick Display")]
    [SerializeField] private GameObject trickNotificationPanel;
    [SerializeField] private TextMeshProUGUI trickNameText;
    [SerializeField] private TextMeshProUGUI trickScoreText;
    [SerializeField] private float trickDisplayTime = 1.5f;

    private WhaleTrickController whaleController;
    private float comboDisplayTimer = 0f;
    private float trickDisplayTimer = 0f;
    private int lastComboCount = 0;

    private void Start()
    {
        // Find whale controller
        whaleController = FindFirstObjectByType<WhaleTrickController>();
        if (!whaleController)
        {
            Debug.LogError("UIManager: No WhaleTrickController found in scene!");
            return;
        }

        // Initialize UI
        if (comboPanel) comboPanel.SetActive(false);
        if (trickNotificationPanel) trickNotificationPanel.SetActive(false);
        
        UpdateAllUI();
    }

    private void Update()
    {
        if (!whaleController) return;

        UpdateAllUI();
        UpdateComboDisplay();
        UpdateTrickDisplay();
    }

    private void UpdateAllUI()
    {
        UpdateScoreUI();
        UpdateComboUI();
        UpdateStaminaUI();
        UpdateStatusUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText)
        {
            scoreText.text = $"Score: {whaleController.GetScore():N0}";
        }
    }

    private void UpdateComboUI()
    {
        if (comboText)
        {
            int currentCombo = whaleController.GetComboCount();
            if (currentCombo > 0)
            {
                comboText.text = $"Combo: {currentCombo}x";
                comboText.color = GetComboColor(currentCombo);
            }
            else
            {
                comboText.text = "Combo: Ready";
                comboText.color = Color.white;
            }

            // Show combo panel when combo increases
            if (currentCombo > lastComboCount && currentCombo > 1)
            {
                ShowComboMultiplier(currentCombo);
            }
            lastComboCount = currentCombo;
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaBar)
        {
            staminaBar.value = whaleController.GetStaminaPercent();
            
            // Color coding for stamina
            Image fillImage = staminaBar.fillRect.GetComponent<Image>();
            if (fillImage)
            {
                float stamina = whaleController.GetStaminaPercent();
                if (stamina > 0.6f)
                    fillImage.color = Color.green;
                else if (stamina > 0.3f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = Color.red;
            }
        }
    }

    private void UpdateStatusUI()
    {
        // Air time display
        if (airTimeText)
        {
            if (!whaleController.IsUnderwater())
            {
                airTimeText.text = $"Air Time: {whaleController.GetAirTime():F1}s";
                airTimeText.gameObject.SetActive(true);
            }
            else
            {
                airTimeText.gameObject.SetActive(false);
            }
        }

        // Depth display
        if (depthText)
        {
            if (whaleController.IsUnderwater())
            {
                float depth = Mathf.Abs(whaleController.transform.position.y);
                depthText.text = $"Depth: {depth:F1}m";
                depthText.gameObject.SetActive(true);
            }
            else
            {
                depthText.gameObject.SetActive(false);
            }
        }

        // Status text
        if (statusText)
        {
            if (whaleController.IsUnderwater())
            {
                statusText.text = "Underwater";
                statusText.color = Color.cyan;
            }
            else
            {
                statusText.text = "Above Water";
                statusText.color = Color.white;
            }
        }
    }

    private void ShowComboMultiplier(int comboCount)
    {
        if (comboPanel && comboMultiplierText)
        {
            comboMultiplierText.text = $"{comboCount}x COMBO!";
            comboMultiplierText.color = GetComboColor(comboCount);
            comboPanel.SetActive(true);
            comboDisplayTimer = comboDisplayTime;
        }
    }

    private void UpdateComboDisplay()
    {
        if (comboDisplayTimer > 0f)
        {
            comboDisplayTimer -= Time.deltaTime;
            if (comboDisplayTimer <= 0f && comboPanel)
            {
                comboPanel.SetActive(false);
            }
        }
    }

    private void UpdateTrickDisplay()
    {
        if (trickDisplayTimer > 0f)
        {
            trickDisplayTimer -= Time.deltaTime;
            if (trickDisplayTimer <= 0f && trickNotificationPanel)
            {
                trickNotificationPanel.SetActive(false);
            }
        }
    }

    private Color GetComboColor(int comboCount)
    {
        if (comboCount >= 5)
            return Color.magenta;
        else if (comboCount >= 3)
            return Color.yellow;
        else if (comboCount >= 2)
            return Color.green;
        else
            return Color.white;
    }

    // Public method to show trick notifications
    public void ShowTrickNotification(string trickName, int score)
    {
        if (trickNotificationPanel && trickNameText && trickScoreText)
        {
            trickNameText.text = trickName;
            trickScoreText.text = $"+{score}";
            trickNotificationPanel.SetActive(true);
            trickDisplayTimer = trickDisplayTime;
        }
    }

    // Method to animate score increase
    public void AnimateScoreIncrease(int oldScore, int newScore)
    {
        StartCoroutine(AnimateScoreCoroutine(oldScore, newScore));
    }

    private System.Collections.IEnumerator AnimateScoreCoroutine(int startScore, int endScore)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            int currentScore = Mathf.RoundToInt(Mathf.Lerp(startScore, endScore, progress));
            
            if (scoreText)
            {
                scoreText.text = $"Score: {currentScore:N0}";
            }
            
            yield return null;
        }

        if (scoreText)
        {
            scoreText.text = $"Score: {endScore:N0}";
        }
    }
}
