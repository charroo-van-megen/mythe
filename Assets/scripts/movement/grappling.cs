using UnityEngine;

[RequireComponent(typeof(PlayerGrapplingController))]
public class Grappling : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public LineRenderer lineRenderer;
    public Camera playerCamera;

    [Header("Settings")]
    public float maxGrappleDistance = 50f;
    public float arcHeight = 8f;
    public float cooldown = 2f;
    public KeyCode grappleKey = KeyCode.Mouse1;
    public LayerMask grappleLayer;

    private float cooldownTimer = 0f;
    private bool isGrappling = false;
    private Vector3 grapplePoint;

    private PlayerGrapplingController grapplingController;

    private void Awake()
    {
        grapplingController = GetComponent<PlayerGrapplingController>();
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (playerCamera == null) playerCamera = Camera.main;
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(grappleKey) && cooldownTimer <= 0f && !isGrappling)
        {
            TryStartGrapple();
        }

        if (isGrappling && lineRenderer != null)
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    private void TryStartGrapple()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * maxGrappleDistance, Color.green, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, grappleLayer))
        {
            grapplePoint = hit.point;
            isGrappling = true;
            cooldownTimer = cooldown;

            grapplingController.JumpToPosition(grapplePoint, arcHeight);

            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, grapplePoint);
            }

            Invoke(nameof(StopGrapple), 3f);
        }
        else
        {
            Debug.Log("Nothing hit by grappling ray.");
        }
    }

    public void StopGrapple()
    {
        isGrappling = false;
        if (lineRenderer != null) lineRenderer.enabled = false;
    }
}
