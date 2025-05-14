using UnityEngine;

[RequireComponent(typeof(PlayerGrapplingController))]
public class Grappling : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Settings")]
    public float maxGrappleDistance = 40f;
    public float grappleDelayTime = 0.2f;
    public float overshootYAxis = 5f;
    public float cooldown = 2f;
    public KeyCode grappleKey = KeyCode.Mouse1;

    private Rigidbody rb;
    private PlayerGrapplingController grapplingController;

    private Vector3 grapplePoint;
    private float cooldownTimer;
    private bool isGrappling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();  // Get Rigidbody for movement control
        grapplingController = GetComponent<PlayerGrapplingController>();

        if (lr != null)
        {
            lr.enabled = false;
        }
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(grappleKey) && cooldownTimer <= 0f && !isGrappling)
        {
            StartGrapple();
        }
    }

    private void LateUpdate()
    {
        if (isGrappling && lr != null && lr.enabled)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }
    }

    private void StartGrapple()
    {
        if (isGrappling || lr == null || grapplingController == null)
            return;

        isGrappling = true;
        FreezePlayerMovement(true);  // Freeze movement when grappling

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            lr.startColor = Color.green;
        }
        else
        {
            StopGrapple(); // No valid target, cancel grapple
            return;
        }

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
        lr.enabled = true;

        cooldownTimer = cooldown;

        Invoke(nameof(ExecuteGrapple), grappleDelayTime);
    }

    private void ExecuteGrapple()
    {
        FreezePlayerMovement(false);  // Unfreeze movement after grapple execution
        float relativeY = grapplePoint.y - (transform.position.y - 1f);
        float arcHeight = relativeY > 0 ? relativeY + overshootYAxis : overshootYAxis;

        grapplingController.JumpToPosition(grapplePoint, arcHeight);
    }

    public void StopGrapple()
    {
        isGrappling = false;
        FreezePlayerMovement(false);  // Unfreeze player movement
        grapplingController.ResetFov();

        if (lr != null)
        {
            lr.enabled = false;
        }
    }

    public bool IsGrappling() => isGrappling;
    public Vector3 GetGrapplePoint() => grapplePoint;

    private void FreezePlayerMovement(bool freeze)
    {
        if (rb != null)
        {
            rb.isKinematic = freeze;  // Disable Rigidbody physics when freezing the player
        }
    }
}
