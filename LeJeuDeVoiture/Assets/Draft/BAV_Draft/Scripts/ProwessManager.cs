using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using Random = UnityEngine.Random;

enum ProwessDescription
{ 
    None,
    EnemyKilled,
    EnviroDestroyed,
}

public class ProwessManager : MonoBehaviour
{
    [Header("Combo Parameters")]
    [SerializeField] AnimationCurve prowessTimeCurve;
    
    // Min and Max Combo Value
    [Range(1.0f, 10.0f)] [Tooltip("xMin of the Curve")] public float minComboValue = 2.0f;
    [Range(1.0f, 10.0f)] [Tooltip("xMax of the Curve")]public float maxComboValue = 6.0f;
    
    // Min and Max Time Value
    [Range(1.0f, 10.0f)] [Tooltip("yMin of the Curve")] public float minTime = 2.0f;
    [Range(1.0f, 10.0f)] [Tooltip("yMax of the Curve")]public float maxTime = 6.0f;
    
    [Header("Prowess Value")]
    public float defaultProwessValue = 1.0f;
    public float currentProwessMultiplier = 1.0f;
    
    [Header("UI Elements")]
    public TextMeshProUGUI prowessMultiplierText;
    public Color testColor;
    private Vector3 originalTextPosition;
    //Shake Text Parameters
    public float minShakeForce = 0.02f;
    public float maxShakeForce = 0.1f;
    //Growing Text Parameters
    public float stretchDuration = 0.5f;
    public float sizeVariation = 1.5f;
    
    //Private Value
    private float currentTimeValue = 0.0f;
    
    //TODO : Remove this part, just for debug
    [Header("Debug Values")]
    public float increaseProwessMultiplier = 0.1f;
    
    public bool enableDebugLog = true;
    private const KeyCode IncreaseMultiplierKey = KeyCode.Space;

    /// <summary>
    /// Return the time based on the currentProwessMultiplier.
    /// The time is based on the currentProwessMultiplier.
    /// </summary>
    private float timeBeforeLessCombo
    {
        get
        {
            // Check if currentProwessMultiplier is less or more than minComboValue or maxComboValue and set to avoid calculation
            if (currentProwessMultiplier > maxComboValue) return minTime;
            if (currentProwessMultiplier < minComboValue) return maxTime;

            // Normalize the currentProwessMultiplier between minComboValue and maxComboValue
            float normalizedComboValue = Mathf.InverseLerp(minComboValue, maxComboValue, currentProwessMultiplier);
            float timeValue = prowessTimeCurve.Evaluate(normalizedComboValue);
            return Mathf.Lerp(minTime, maxTime, timeValue);
        }
    }

    private void Start()
    {
        currentTimeValue = timeBeforeLessCombo;
        prowessMultiplierText.gameObject.SetActive(false);
        originalTextPosition = prowessMultiplierText.transform.position;
    }

    void Update()
    {
        if (currentProwessMultiplier > defaultProwessValue)
        {
            DisplayProwessMultiplier();
            DecreaseTime();
        }
        //TODO : Remove this part, just for debug
        if(Input.GetKeyDown(IncreaseMultiplierKey))
        {
            IncreaseProwessMultiplier(increaseProwessMultiplier);
            if (enableDebugLog)
            {
                Debug.Log($"currentProwessMultiplier : {currentProwessMultiplier}, currentTimeValue: {currentTimeValue}");
            }

            if (Mathf.Approximately(currentProwessMultiplier, Mathf.Round(currentProwessMultiplier)))
            {
                _ = GrowingTextAsync(stretchDuration, sizeVariation);
            }
            else
            {
                _ = ShakeTextAsync(timeBeforeLessCombo);
            }
        }
    }

    /// <summary>
    /// Increase the currentProwessMultiplier by the amount.
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseProwessMultiplier(float amount)
    {
        currentProwessMultiplier += amount;
        currentTimeValue = timeBeforeLessCombo; //Reset the timer
    }

    /// <summary>
    /// Decrease the currentTimeValue and reset if the timer is 0.
    /// </summary>
    private void DecreaseTime()
    {
        if (currentProwessMultiplier <= 1.0f) return;
        currentTimeValue -= Time.deltaTime;
        if (currentTimeValue <= 0)
        {
            ResetProwess();
        }
    }
    
    private void DisplayProwessMultiplier()
    {
        if (prowessMultiplierText == null) return;
        prowessMultiplierText.gameObject.SetActive(true);
        prowessMultiplierText.text = $"X. <{testColor.ToHex()}> {currentProwessMultiplier:F1} </color> <size=50%> ({currentTimeValue:F1}) </size>";
    }

    

    /// <summary>
    ///  Reset the currentProwessMultiplier to defaultProwessValue and the currentTimeValue to TimeBeforeLessCombo.
    ///  This function is called when the player don't hit an enemy after a time.
    /// </summary>
    private void ResetProwess()
    {
        currentProwessMultiplier = defaultProwessValue;
        currentTimeValue = timeBeforeLessCombo;
        prowessMultiplierText.gameObject.SetActive(false);
    }
    
    async Task ShakeTextAsync(float shakeDuration)
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float normalizedMultiplier = Mathf.InverseLerp(minComboValue, maxComboValue, currentProwessMultiplier);
            float shakeForce = Mathf.Lerp(minShakeForce, maxShakeForce, normalizedMultiplier);

            float x = originalTextPosition.x + Random.Range(-1f, 1f) * shakeForce;
            float y = originalTextPosition.y + Random.Range(-1f, 1f) * shakeForce;

            prowessMultiplierText.transform.position = new Vector3(x, y, originalTextPosition.z);

            elapsed += Time.deltaTime;

            await Task.Yield();
        }

        prowessMultiplierText.transform.position = originalTextPosition;
    }

    async Task GrowingTextAsync(float duration, float sizeVariation)
    {
        Vector3 originalScale = prowessMultiplierText.rectTransform.localScale;
        Vector3 targetScale = originalScale * sizeVariation; 
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            prowessMultiplierText.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);

            elapsed += Time.deltaTime;

            await Task.Yield();
        }

        prowessMultiplierText.rectTransform.localScale = originalScale;
    }
}
