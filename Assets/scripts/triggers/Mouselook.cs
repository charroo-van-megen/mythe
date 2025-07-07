using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;

    public CameraSensitivity cameraSensitivity;

    private float mouseSensitivity = 10f; // fallback default
    private float xRotation = 0f;
    private bool isPaused = false;

    void Start()
    {
        if (cameraSensitivity == null)
        {
            cameraSensitivity = FindObjectOfType<CameraSensitivity>();
            if (cameraSensitivity == null)
                Debug.LogWarning("CameraSensitivity not found in scene for MouseLook.");
        }

        if (cameraSensitivity != null)
        {
            mouseSensitivity = cameraSensitivity.CurrentSensitivity;
            cameraSensitivity.OnSensitivityChanged += UpdateSensitivity;
            Debug.Log($"MouseLook subscribed to sensitivity: {mouseSensitivity}");
        }

        LockCursor();
    }

    void Update()
    {
        if (isPaused) return;

        // Debug log to see sensitivity live
        Debug.Log($"MouseLook current sensitivity: {mouseSensitivity}");

        // *** Removed Time.unscaledDeltaTime here ***
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
        else
            Debug.LogWarning("MouseLook: playerBody not assigned!");
    }

    public void UpdateSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
        Debug.Log($"MouseLook received new sensitivity: {mouseSensitivity}");
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;

        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            LockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDestroy()
    {
        if (cameraSensitivity != null)
            cameraSensitivity.OnSensitivityChanged -= UpdateSensitivity;
    }
}
