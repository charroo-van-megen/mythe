using UnityEngine;
using UnityEngine.UI;

public class CameraSensitivity : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public Slider sensitivitySlider;  // Assign in Inspector
    public float defaultSensitivity = 100f;  // Increased default value
    private const string sensitivityKey = "currentSensitivity";

    public float CurrentSensitivity { get; private set; }

    public event System.Action<float> OnSensitivityChanged;

    void Start()
    {
        // Load saved sensitivity or use default
        CurrentSensitivity = PlayerPrefs.GetFloat(sensitivityKey, defaultSensitivity);
        CurrentSensitivity = Mathf.Max(0.1f, CurrentSensitivity); // Prevent zero or negative

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 50f;    // Increased min
            sensitivitySlider.maxValue = 500f;   // Increased max
            sensitivitySlider.wholeNumbers = false; // Allows fine-tuning
            sensitivitySlider.value = CurrentSensitivity;

            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        }

        // Notify any listeners (in case they initialize late)
        OnSensitivityChanged?.Invoke(CurrentSensitivity);
    }

    private void UpdateSensitivity(float value)
    {
        CurrentSensitivity = Mathf.Max(0.1f, value);
        PlayerPrefs.SetFloat(sensitivityKey, CurrentSensitivity);
        PlayerPrefs.Save();

        OnSensitivityChanged?.Invoke(CurrentSensitivity);

        Debug.Log("CameraSensitivity updated to: " + CurrentSensitivity);
    }
}
