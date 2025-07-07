using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(PlayerGrapplingController), typeof(Rigidbody))]
public class Grappling : MonoBehaviour
{
    [Header("Grappling Settings")]
    public float maxGrappleDistance = 100f;
    public float grappleDelay = 0.1f;
    public float launchArcHeight = 5f;
    public float grappleCooldown = 1f;
    public LayerMask grappleLayer;
    public KeyCode grappleKey = KeyCode.Mouse1;

    [Header("References")]
    public Transform gunTip;
    public Camera playerCamera;
    public LineRenderer lineRenderer;

    private Vector3 grapplePoint;
    private bool isGrappling = false;
    private float cooldownTimer = 0f;

    private PlayerGrapplingController grapplingController;
    private Rigidbody rb;

    private void Awake()
    {
        grapplingController = GetComponent<PlayerGrapplingController>();
        rb = GetComponent<Rigidbody>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 0;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(grappleKey) && cooldownTimer <= 0f)
        {
            StartGrapple();
        }

        if (isGrappling && lineRenderer.enabled)
        {
            Vector3 lineStart = gunTip != null ? gunTip.position : transform.position;
            lineRenderer.SetPosition(0, lineStart);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    private void StartGrapple()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, grappleLayer))
        {
            grapplePoint = hit.point;
            isGrappling = true;
            cooldownTimer = grappleCooldown;

            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
                lineRenderer.positionCount = 2;
                Vector3 start = gunTip != null ? gunTip.position : transform.position;
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, grapplePoint);
            }

            Invoke(nameof(ExecuteGrapple), grappleDelay);
        }
        else
        {
            Debug.Log("No valid grapple point found.");
        }
    }

    private void ExecuteGrapple()
    {
        // Use Rigidbody's current position for accuracy
        Vector3 startPoint = rb.position;

        Vector3 velocity = grapplingController.CalculateJumpVelocity(startPoint, grapplePoint, launchArcHeight);
        grapplingController.ApplyPendingVelocity(velocity);

        Invoke(nameof(StopGrapple), 1.5f);
    }

    public void StopGrapple()
    {
        isGrappling = false;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 0;
        }

        grapplingController.ResetFov();
    }

    public bool IsGrappling() => isGrappling;

    public Vector3 GetGrapplePoint() => grapplePoint;
}
