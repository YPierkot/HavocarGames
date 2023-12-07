using UnityEngine;
using TMPro;
using System.Threading;
using System.Threading.Tasks;

[System.Serializable]
public enum DisplayMode
{
    FPS,
    FPSAndMS
}

public class FPSCounter : MonoBehaviour
{
    [Header("Settings")]
    public DisplayMode currentDisplayMode = DisplayMode.FPS; // Default display mode
    public float updateRate = 0.5f;

    [Header("UI Elements")]
    public TextMeshProUGUI fpsText;

    private float frameRate = 0f;
    private float frameTimeMs = 0f;
    private CancellationTokenSource cancellationTokenSource;


    private void Start()
    {
        if (fpsText == null)
        {
            Debug.LogError("Please assign a TextMeshProUGUI object to fpsText in the inspector.");
            enabled = false;
            return;
        }

        cancellationTokenSource = new CancellationTokenSource();
        UpdateFPSAsync(cancellationTokenSource.Token);
    }
    
    private void Update()
    {
        UpdateTargetFrameRate();
    }

    private void OnDestroy()
    { 
        cancellationTokenSource.Cancel();
    }

    private async void UpdateFPSAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay((int)(updateRate * 1000), cancellationToken);

            // Check for cancellation before proceeding
            if (cancellationToken.IsCancellationRequested)
                return;

            // Calculate FPS and frame time on the main thread
            CalculateFPS();

            // Update the UI on the main thread
            UpdateUIText();
        }
    }

    private void UpdateTargetFrameRate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetTargetFrameRate(KeyCode.F1, 10);
            SetTargetFrameRate(KeyCode.F2, 20);
            SetTargetFrameRate(KeyCode.F3, 30);
            SetTargetFrameRate(KeyCode.F4, 60);
            SetTargetFrameRate(KeyCode.F5, 120);
        }
    }

    private void SetTargetFrameRate(KeyCode key, int frameRate)
    {
        if (Input.GetKeyUp(key))
        {
            Application.targetFrameRate = frameRate;
        }
    }

    private void CalculateFPS()
    {
        float fpsAccumulator = 0f;
        float frameTimeAccumulator = 0f;
        int frameCounter = 0;

        for (int i = 0; i < 60; i++) // Assuming 60 frames to calculate average FPS
        {
            float deltaTime = Time.deltaTime;
            fpsAccumulator += deltaTime;
            frameTimeAccumulator += deltaTime * 1000; // Convert to milliseconds
            frameCounter++;
        }

        frameRate = frameCounter / fpsAccumulator;
        frameTimeMs = frameTimeAccumulator / frameCounter;
    }

    private void UpdateUIText()
    {
        // Update the UI text on the main thread based on the current display mode
        switch (currentDisplayMode)
        {
            case DisplayMode.FPS:
                UpdateUIText($"FPS: {Mathf.Round(frameRate)}");
                break;
            case DisplayMode.FPSAndMS:
                UpdateUIText($"FPS: {Mathf.Round(frameRate)}, Frame Time: {frameTimeMs:F2} ms");
                break;
        }
    }

    private void UpdateUIText(string text, bool enableDebug = false)
    {
        fpsText.text = text;
        if (enableDebug)
        {
            Debug.Log(text);
        }
    }
}
