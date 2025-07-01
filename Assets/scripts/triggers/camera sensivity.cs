using UnityEngine;
using UnityEngine.UI;
using System;

public class CameraSensitivity : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public Slider sensitivitySlider;  // Assign in Inspector
    public float defaultSensitivity = 10f;

    private const string sensitivityKey = "currentSensitivity";
    public float CurrentSensitivity { get; private set; }

    public event Action<float> OnSensitivityChanged;

    void Start()
    {
        // Load saved sensitivity or use default
        CurrentSensitivity = PlayerPrefs.GetFloat(sensitivityKey, defaultSensitivity);
        CurrentSensitivity = Mathf.Max(0.1f, CurrentSensitivity);

        if (sensitivitySlider != null)
        {
            // Set slider range and value
            sensitivitySlider.minValue = 0.1f;
            sensitivitySlider.maxValue = 10f;
            sensitivitySlider.wholeNumbers = false;
            sensitivitySlider.value = CurrentSensitivity;

            // Subscribe to slider change event
            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        }
        else
        {
            Debug.LogWarning("Sensitivity slider not assigned in CameraSensitivity.");
        }

        // Notify listeners of initial value
        OnSensitivityChanged?.Invoke(CurrentSensitivity);
        Debug.Log($"CameraSensitivity initialized with {CurrentSensitivity}");
    }

    private void UpdateSensitivity(float value)
    {
        CurrentSensitivity = Mathf.Max(0.1f, value);
        PlayerPrefs.SetFloat(sensitivityKey, CurrentSensitivity);
        PlayerPrefs.Save();

        Debug.Log($"CameraSensitivity changed to {CurrentSensitivity}");
        OnSensitivityChanged?.Invoke(CurrentSensitivity);
    }
}
