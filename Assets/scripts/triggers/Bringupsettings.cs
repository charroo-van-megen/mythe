using UnityEngine;

public class Bringupsettings : MonoBehaviour
{
    public GameObject setting;
    public bool issettingactive;

    private MouseLook mouseLook;

    void Start()
    {
        mouseLook = GetComponent<MouseLook>();
        if (mouseLook == null)
        {
            Debug.LogError("MouseLook component not found on this GameObject.");
        }
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
        setting.SetActive(true);
        issettingactive = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mouseLook != null)
            mouseLook.SetPaused(true);
    }

    public void Resume()
    {
        setting.SetActive(false);
        issettingactive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (mouseLook != null)
            mouseLook.SetPaused(false);
    }
}
