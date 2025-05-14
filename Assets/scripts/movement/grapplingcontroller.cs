using UnityEngine;

public class PlayerGrapplingController : MonoBehaviour
{
    private Rigidbody rb;
    private bool enableMovementOnNextTouch;
    private Vector3 _pendingVelocity;

    public PlayerCam cam;
    public float grappleFov = 95f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void JumpToPosition(Vector3 target, float height)
    {
        _pendingVelocity = CalculateJumpVelocity(transform.position, target, height);
        Invoke(nameof(SetVelocity), 0.1f);
        Invoke(nameof(ResetFov), 3f);
    }

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.linearVelocity = _pendingVelocity;
        cam?.DoFov(grappleFov);
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

    public Vector3 CalculateJumpVelocity(Vector3 start, Vector3 end, float height)
    {
        float gravity = Physics.gravity.y;
        float displacementY = end.y - start.y;
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0f, end.z - start.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity));

        return velocityXZ + velocityY;
    }
}
