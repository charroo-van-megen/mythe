using UnityEngine;
using UnityEngine.UI;
using System;

public class CameraSensitivity : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public Slider sensitivitySlider;  // Assign in Inspector
    public float defaultSensitivity = 100f;  // Single declaration with intended value

    private const string sensitivityKey = "currentSensitivity";
    public float CurrentSensitivity { get; private set; }

    // Optional: Other scripts can subscribe to this
    public event Action<float> OnSensitivityChanged;

    void Start()
    {
        // Load saved sensitivity or use default
        CurrentSensitivity = PlayerPrefs.GetFloat(sensitivityKey, defaultSensitivity);
        CurrentSensitivity = Mathf.Max(0.1f, CurrentSensitivity); // Prevent zero or negative

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 50f;      // Adjusted min
            sensitivitySlider.maxValue = 500f;     // Adjusted max
            sensitivitySlider.wholeNumbers = false; // Allow fine adjustment
            sensitivitySlider.value = CurrentSensitivity;

            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        }

        // Notify listeners (like MouseLook) immediately
        OnSensitivityChanged?.Invoke(CurrentSensitivity);
    }

    private void UpdateSensitivity(float value)
    {
        CurrentSensitivity = Mathf.Max(0.1f, value);
        PlayerPrefs.SetFloat(sensitivityKey, CurrentSensitivity);
        PlayerPrefs.Save();

        OnSensitivityChanged?.Invoke(CurrentSensitivity);
    }
}
