using UnityEngine;
using System.Collections;

public class PlayerCam : MonoBehaviour
{
    private Camera cam;  // Reference to the Camera component
    private Coroutine fovCoroutine;  // To handle the coroutine for smooth FOV transitions
    private float defaultFov;  // The default FOV when not in special modes
    public float fovLerpDuration = 0.3f;  // The duration for the FOV transition

    private void Awake()
    {
        // Try to get the camera component from the children of this GameObject
        cam = GetComponentInChildren<Camera>();

        if (cam != null)
            defaultFov = cam.fieldOfView;  // Store the default field of view (FOV)
        else
            Debug.LogWarning("No Camera found in children of PlayerCam.");
    }

    // Public method to change the FOV to a target value
    public void DoFov(float targetFov)
    {
        if (fovCoroutine != null)  // If there's already an ongoing FOV transition, stop it
            StopCoroutine(fovCoroutine);

        // Start a new coroutine to smoothly transition to the target FOV
        fovCoroutine = StartCoroutine(LerpFov(targetFov));
    }

    // Public method to reset FOV to the default value
    public void ResetFov()
    {
        if (cam != null)
        {
            DoFov(defaultFov);  // Reset the FOV to the default value
        }
    }

    // Coroutine to smoothly change the FOV between the current and target values
    private IEnumerator LerpFov(float targetFov)
    {
        if (cam == null) yield break;  // If no camera is found, exit the coroutine

        float elapsed = 0f;
        float startFov = cam.fieldOfView;  // Start value of the FOV

        while (elapsed < fovLerpDuration)  // While the transition isn't complete
        {
            // Lerp (smoothly interpolate) between the start FOV and target FOV
            cam.fieldOfView = Mathf.Lerp(startFov, targetFov, elapsed / fovLerpDuration);
            elapsed += Time.deltaTime;  // Increment elapsed time
            yield return null;  // Wait until the next frame
        }

        cam.fieldOfView = targetFov;  // Ensure the target FOV is set after the transition
    }
}
