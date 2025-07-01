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
    public MouseLook mouseLookScript;

    private bool isSettingsActive = false;
    private bool wasGrapplingBeforePause = false;

    void Start()
    {
        movementScript ??= FindObjectOfType<New_Movement>();
        grapplingScript ??= FindObjectOfType<Grappling>();
        grapplingController ??= FindObjectOfType<PlayerGrapplingController>();
        mouseLookScript ??= FindObjectOfType<MouseLook>();

        if (setting != null) setting.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);

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

        // Pause time
        Time.timeScale = 0f;

        // Disable movement & grappling scripts
        if (movementScript != null) movementScript.enabled = false;

        if (grapplingScript != null)
        {
            wasGrapplingBeforePause = grapplingScript.IsGrappling();
            grapplingScript.enabled = false;
        }

        // Freeze physics
        if (grapplingController != null)
        {
            Rigidbody rb = grapplingController.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }

        // Unlock cursor
        if (mouseLookScript != null)
            mouseLookScript.SetPaused(true);
    }

    public void Resume()
    {
        isSettingsActive = false;

        // Resume time
        Time.timeScale = 1f;

        if (setting != null) setting.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);

        if (movementScript != null) movementScript.enabled = true;
        if (grapplingScript != null) grapplingScript.enabled = true;

        if (grapplingController != null)
        {
            Rigidbody rb = grapplingController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;

                // Restore pending velocity if any
                if (grapplingController.pendingVelocity != Vector3.zero)
                {
                    rb.linearVelocity = grapplingController.pendingVelocity;
                    grapplingController.pendingVelocity = Vector3.zero;
                }
            }
        }

        if (mouseLookScript != null)
            mouseLookScript.SetPaused(false);

        // Restore grappling visual
        if (wasGrapplingBeforePause && grapplingScript != null && grapplingScript.IsGrappling())
        {
            LineRenderer lr = grapplingScript.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.enabled = true;
                lr.positionCount = 2;
                lr.SetPosition(0, grapplingScript.gunTip.position);
                lr.SetPosition(1, grapplingScript.GetGrapplePoint());
            }
        }

        wasGrapplingBeforePause = false;
    }
}
