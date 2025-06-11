using UnityEngine;
using UnityEngine.UI;

public class CameraSensitivity : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public Slider sensitivitySlider;  // Assign in Inspector
    public float defaultSensitivity = 10f;
    private const string sensitivityKey = "currentSensitivity";

    public float CurrentSensitivity { get; private set; }

    public event System.Action<float> OnSensitivityChanged;

    void Start()
    {
        // Load saved sensitivity or default
        CurrentSensitivity = PlayerPrefs.GetFloat(sensitivityKey, defaultSensitivity);
        CurrentSensitivity = Mathf.Max(0.1f, CurrentSensitivity); // Prevent zero

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 1f;
            sensitivitySlider.maxValue = 20f;
            sensitivitySlider.wholeNumbers = true;
            sensitivitySlider.value = CurrentSensitivity;

            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        }
    }

    private void UpdateSensitivity(float value)
    {
        CurrentSensitivity = Mathf.Max(0.1f, value);  // Clamp to prevent zero
        PlayerPrefs.SetFloat(sensitivityKey, CurrentSensitivity);
        PlayerPrefs.Save();

        OnSensitivityChanged?.Invoke(CurrentSensitivity);

        Debug.Log("CameraSensitivity updated to: " + CurrentSensitivity);
    }
}
