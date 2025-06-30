using UnityEngine;

public class PlayerGrapplingController : MonoBehaviour
{
    public New_Movement movementScript;  // Reference to your movement script
    public PlayerCam cam;

    public float grappleFov = 95f;

    private void Awake()
    {
        if (movementScript == null)
            movementScript = GetComponent<New_Movement>();
    }

    public void JumpToPosition(Vector3 target, float arcHeight)
    {
        Vector3 launchVelocity = CalculateJumpVelocity(transform.position, target, arcHeight);

        if (IsVelocityValid(launchVelocity))
        {
            movementScript.AddLaunchVelocity(launchVelocity);

            cam?.DoFov(grappleFov);
            Invoke(nameof(ResetFov), 3f);
        }
        else
        {
            Debug.LogWarning("Invalid grapple velocity calculated (NaN). Check arcHeight and target position.");
        }
    }

    public void ResetFov()
    {
        cam?.DoFov(85f);
    }

    public Vector3 CalculateJumpVelocity(Vector3 start, Vector3 end, float arcHeight)
    {
        Vector3 toTarget = end - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float yOffset = toTarget.y;
        float gravity = Mathf.Abs(Physics.gravity.y);

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
