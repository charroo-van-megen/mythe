using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerGrapplingController : MonoBehaviour
{
    private Rigidbody rb;
    public PlayerCam cam;

    public float grappleFov = 95f;
    private bool enableMovementOnNextTouch = false;
    private Vector3 _pendingVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void JumpToPosition(Vector3 target, float arcHeight)
    {
        _pendingVelocity = CalculateJumpVelocity(transform.position, target, arcHeight);

        if (IsVelocityValid(_pendingVelocity))
        {
            Invoke(nameof(SetVelocity), 0.1f);
            Invoke(nameof(ResetFov), 3f);
        }
        else
        {
            Debug.LogWarning("Invalid grapple velocity calculated (NaN). Check arcHeight and target position.");
        }
    }

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;

        if (IsVelocityValid(_pendingVelocity))
        {
            rb.linearVelocity = _pendingVelocity;
            cam?.DoFov(grappleFov);
        }
    }

    public void ResetFov()
    {
        cam?.DoFov(85f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            GetComponent<Grappling>()?.StopGrapple();
        }
    }

    public Vector3 CalculateJumpVelocity(Vector3 start, Vector3 end, float arcHeight)
    {
        Vector3 toTarget = end - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float xzDistance = toTargetXZ.magnitude;
        float yOffset = toTarget.y;
        float gravity = Mathf.Abs(Physics.gravity.y);

        // Clamp arc height and vertical offset
        arcHeight = Mathf.Max(arcHeight, 0.1f);
        float heightDifference = Mathf.Max(yOffset - arcHeight, 0.1f);

        float timeUp = Mathf.Sqrt(2 * arcHeight / gravity);
        float timeDown = Mathf.Sqrt(2 * heightDifference / gravity);
        float totalTime = timeUp + timeDown;

        if (totalTime < 0.1f) totalTime = 0.1f;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * arcHeight);
        Vector3 velocityXZ = toTargetXZ / totalTime;

        return velocityXZ + velocityY;
    }

    private bool IsVelocityValid(Vector3 velocity)
    {
        return !(float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z));
    }
}
