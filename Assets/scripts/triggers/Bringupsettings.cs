using UnityEngine;

public class Bringupsettings : MonoBehaviour
{
    [Header("UI References")]
    public GameObject setting;
    public GameObject crosshair;

    [Header("Script References")]
    public New_Movement movementScript;
    public Grappling grapplingScript;
    public PlayerGrapplingController grapplingController;
    public MouseLook mouseLookScript; // NEW

    private bool issettingactive = false;

    void Start()
    {
        movementScript ??= FindObjectOfType<New_Movement>();
        grapplingScript ??= FindObjectOfType<Grappling>();
        grapplingController ??= FindObjectOfType<PlayerGrapplingController>();
        mouseLookScript ??= FindObjectOfType<MouseLook>(); // NEW

        setting?.SetActive(false);
        crosshair?.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!issettingactive)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        setting?.SetActive(true);
        crosshair?.SetActive(false);

        issettingactive = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (movementScript != null)
            movementScript.enabled = false;

        if (grapplingScript != null)
        {
            grapplingScript.StopGrapple();
            grapplingScript.enabled = false;
        }

        if (grapplingController != null)
        {
            var rb = grapplingController.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }

        if (mouseLookScript != null)
            mouseLookScript.SetPaused(true);
    }

    public void Resume()
    {
        setting?.SetActive(false);
        crosshair?.SetActive(true);

        issettingactive = false;
        Time.timeScale = 1f;
        // Remove these cursor calls here because MouseLook.SetPaused(false) will handle locking
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        if (movementScript != null)
            movementScript.enabled = true;

        if (grapplingScript != null)
            grapplingScript.enabled = true;

        if (grapplingController != null)
        {
            var rb = grapplingController.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }

        if (mouseLookScript != null)
            mouseLookScript.SetPaused(false);  // This will call LockCursor internally
    }
}
