using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private Camera cam;
    private float defaultFov;
    private Coroutine fovCoroutine;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        if (cam != null)
            defaultFov = cam.fieldOfView;
    }

    public void DoFov(float targetFov)
    {
        if (fovCoroutine != null)
            StopCoroutine(fovCoroutine);
        fovCoroutine = StartCoroutine(LerpFov(targetFov));
    }

    private System.Collections.IEnumerator LerpFov(float targetFov)
    {
        float duration = 0.3f;
        float elapsed = 0f;

        float startFov = cam.fieldOfView;

        while (elapsed < duration)
        {
            cam.fieldOfView = Mathf.Lerp(startFov, targetFov, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.fieldOfView = targetFov;
    }
}
