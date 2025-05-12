using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerMovementGrappling : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float swingSpeed = 12f;
    public float groundDrag = 5f;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.4f;
    private bool readyToJump = true;

    [Header("Crouching")]
    public float crouchSpeed = 2f;
    public float crouchYScale = 0.5f;
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    public Transform orientation;
    public PlayerCam cam;
    public float grappleFov = 95f;

    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_mode;

    private Rigidbody rb;
    private float moveSpeed;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;

    public enum MovementState { freeze, grappling, swinging, walking, sprinting, crouching, air }
    public MovementState state;

    public bool freeze;
    public bool activeGrapple;
    public bool swinging;

    private Vector3 _pendingVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();
        rb.linearDamping = (grounded && !activeGrapple) ? groundDrag : 0f;
        TextStuff();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0f;
            rb.linearVelocity = Vector3.zero;
        }
        else if (activeGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = sprintSpeed;
        }
        else if (swinging)
        {
            state = MovementState.swinging;
            moveSpeed = swingSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        if (activeGrapple || swinging) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            if (rb.linearVelocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (activeGrapple) return;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() => readyToJump = true;

    public void JumpToPosition(Vector3 target, float height)
    {
        activeGrapple = true;
        _pendingVelocity = CalculateJumpVelocity(transform.position, target, height);
        Invoke(nameof(SetVelocityInternal), 0.1f); // This correctly calls the SetVelocityInternal method after 0.1s
        Invoke(nameof(ResetRestrictions), 3f);
    }

    private void SetVelocityInternal()
    {
        enableMovementOnNextTouch = true;
        rb.linearVelocity = _pendingVelocity;
        if (cam != null) cam.DoFov(grappleFov);
    }

    private void ResetRestrictions()
    {
        activeGrapple = false;
        if (cam != null) cam.DoFov(85f);
    }

    private bool enableMovementOnNextTouch;

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<Grappling>()?.StopGrapple();
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection() =>
        Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;

    public Vector3 CalculateJumpVelocity(Vector3 start, Vector3 end, float height)
    {
        float gravity = Physics.gravity.y;
        float displacementY = end.y - start.y;
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity));

        return velocityXZ + velocityY;
    }

    private void TextStuff()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        text_speed?.SetText("Speed: " + flatVel.magnitude.ToString("F1") + " / " + moveSpeed.ToString("F1"));
        text_mode?.SetText(state.ToString());
    }
}
