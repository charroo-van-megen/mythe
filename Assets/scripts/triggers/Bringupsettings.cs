using UnityEngine;

public class Bringupsettings : MonoBehaviour
{
    public GameObject setting;               // The settings UI panel
    public bool issettingactive = false;     // Is settings UI open?
    public Movement movementScript;          // Reference to Movement script

    void Start()
    {
        if (movementScript == null)
        {
            movementScript = FindObjectOfType<Movement>();

            if (movementScript == null)
                Debug.LogError("Movement script reference not assigned and not found in scene!");
        }

        if (setting != null)
            setting.SetActive(false);
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
        if (setting != null)
            setting.SetActive(true);

        issettingactive = true;

        // Freeze time
        Time.timeScale = 0f;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable movement & camera
        if (movementScript != null)
            movementScript.enabled = false;
    }

    public void Resume()
    {
        if (setting != null)
            setting.SetActive(false);

        issettingactive = false;

        // Resume time
        Time.timeScale = 1f;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Re-enable movement & camera
        if (movementScript != null)
            movementScript.enabled = true;
    }
}
