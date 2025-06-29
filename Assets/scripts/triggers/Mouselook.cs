using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;
    public CameraSensitivity cameraSensitivity;

    private float mouseSensitivity;
    private float xRotation = 0f;
    private bool isPaused = false;

    void Start()
    {
        if (cameraSensitivity != null)
        {
            mouseSensitivity = cameraSensitivity.CurrentSensitivity;
            cameraSensitivity.OnSensitivityChanged += UpdateSensitivity;
        }
        else
        {
            Debug.LogWarning("CameraSensitivity reference missing in MouseLook!");
        }

        LockCursor();
    }

    void Update()
    {
        if (isPaused) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void UpdateSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
        Debug.Log("MouseLook received new sensitivity: " + mouseSensitivity);
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDestroy()
    {
        if (cameraSensitivity != null)
        {
            cameraSensitivity.OnSensitivityChanged -= UpdateSensitivity;
        }
    }
}
