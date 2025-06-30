using UnityEngine;

public class Bringupsettings : MonoBehaviour
{
    [Header("UI References")]
    public GameObject setting;
    public GameObject crosshair;

    [Header("Script References")]
    public Movement movementScript;
    public Grappling grapplingScript;
    public PlayerGrapplingController grapplingController;

    private bool issettingactive = false;

    void Start()
    {
        // Auto-assign missing references
        movementScript ??= FindObjectOfType<Movement>();
        grapplingScript ??= FindObjectOfType<Grappling>();
        grapplingController ??= FindObjectOfType<PlayerGrapplingController>();

        if (setting != null)
            setting.SetActive(false);
        if (crosshair != null)
            crosshair.SetActive(true);
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
            grapplingScript.StopGrapple();      // Safely stop any ongoing grapple
            grapplingScript.enabled = false;    // Disable grappling input
        }

        if (grapplingController != null)
        {
            var rb = grapplingController.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;          // Freeze physics
        }
    }

    public void Resume()
    {
        setting?.SetActive(false);
        crosshair?.SetActive(true);

        issettingactive = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (movementScript != null)
            movementScript.enabled = true;

        if (grapplingScript != null)
            grapplingScript.enabled = true;

        if (grapplingController != null)
        {
            var rb = grapplingController.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = false;         // Resume physics
        }
    }
}
