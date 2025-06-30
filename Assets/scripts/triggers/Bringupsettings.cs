using UnityEngine;

public class Bringupsettings : MonoBehaviour
{
    [Header("UI References")]
    public GameObject setting;
    public GameObject crosshair;

    [Header("Script References")]
    public New_Movement movementScript; // Only keep New_Movement
    public Grappling grapplingScript;
    public PlayerGrapplingController grapplingController;
    public MouseLook mouseLookScript;

    private bool issettingactive = false;

    void Start()
    {
        // Auto-assign missing references
        movementScript ??= FindObjectOfType<New_Movement>();
        grapplingScript ??= FindObjectOfType<Grappling>();
        grapplingController ??= FindObjectOfType<PlayerGrapplingController>();
        mouseLookScript ??= FindObjectOfType<MouseLook>();

        if (setting != null) setting.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);
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
        if (setting != null) setting.SetActive(true);
        if (crosshair != null) crosshair.SetActive(false);

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
        if (setting != null) setting.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);

        issettingactive = false;
        Time.timeScale = 1f;

        // MouseLook handles cursor locking
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
            mouseLookScript.SetPaused(false);
    }
}
