using UnityEngine;

public class Bringupsettings : MonoBehaviour
{
    [Header("UI References")]
    public GameObject setting;     // Assign in Inspector
    public GameObject crosshair;   // Assign in Inspector

    [Header("Script References")]
    public New_Movement movementScript;
    public Grappling grapplingScript;
    public PlayerGrapplingController grapplingController;
    public MouseLook mouseLookScript;

    private bool isSettingsActive = false;

    void Start()
    {
        // Auto-assign references if missing
        movementScript ??= FindObjectOfType<New_Movement>();
        grapplingScript ??= FindObjectOfType<Grappling>();
        grapplingController ??= FindObjectOfType<PlayerGrapplingController>();
        mouseLookScript ??= FindObjectOfType<MouseLook>();

        // Ensure settings UI is hidden and crosshair shown at start
        if (setting != null)
            setting.SetActive(false);
        if (crosshair != null)
            crosshair.SetActive(true);

        isSettingsActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isSettingsActive)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        if (setting != null) setting.SetActive(true);
        if (crosshair != null) crosshair.SetActive(false);

        isSettingsActive = true;
        Time.timeScale = 0f;

        if (movementScript != null) movementScript.enabled = false;

        if (grapplingScript != null)
        {
            grapplingScript.StopGrapple();
            grapplingScript.enabled = false;
        }

        if (grapplingController != null)
        {
            Rigidbody rb = grapplingController.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }

        if (mouseLookScript != null)
            mouseLookScript.SetPaused(true);  // This manages cursor lock & visibility now
    }

    public void Resume()
    {
        if (setting != null) setting.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);

        isSettingsActive = false;
        Time.timeScale = 1f;

        if (movementScript != null) movementScript.enabled = true;
        if (grapplingScript != null) grapplingScript.enabled = true;

        if (grapplingController != null)
        {
            Rigidbody rb = grapplingController.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }

        if (mouseLookScript != null)
            mouseLookScript.SetPaused(false);  // This manages cursor lock & visibility now
    }
}
