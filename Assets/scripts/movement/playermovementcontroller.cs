using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Speed Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;
    public float airMultiplier = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;

    [Header("Crouching")]
    public float crouchYScale = 0.5f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f;

    [Header("References")]
    public Transform orientation;

    private Rigidbody rb;
    private float startYScale;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;

    private bool grounded;
    private bool readyToJump = true;
    private bool exitingSlope;
    private RaycastHit slopeHit;

    public bool freeze;

    public float CurrentMoveSpeed { get; private set; }
    public Vector3 CurrentVelocity => rb.linearVelocity; // Fixed incorrect use of 'linearVelocity'

    // Called when the script is initialized
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYScale = transform.localScale.y;
    }

    // Called once per frame
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        GetInput();
        HandleCrouch();
        StateHandler();
        SpeedControl();
    }

    // Called once per physics frame
    private void FixedUpdate()
    {
        if (!freeze)
            MovePlayer();
    }

    // Handles input detection
    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    // Handles crouch behavior
    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    // Determines current movement state (walk/sprint/crouch/air)
    private void StateHandler()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            CurrentMoveSpeed = crouchSpeed;
        else if (grounded && Input.GetKey(KeyCode.LeftShift))
            CurrentMoveSpeed = sprintSpeed;
        else if (grounded)
            CurrentMoveSpeed = walkSpeed;
        else
            CurrentMoveSpeed = walkSpeed;
    }

    // Moves the player based on input and slope handling
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeDirection() * CurrentMoveSpeed * 20f, ForceMode.Force);
            if (rb.linearVelocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * CurrentMoveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * CurrentMoveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    // Limits speed to prevent exceeding maximum allowed
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > CurrentMoveSpeed)
        {
            Vector3 limited = flatVel.normalized * CurrentMoveSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }

    // Executes the jump
    private void Jump()
    {
        exitingSlope = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // Resets jump availability
    private void ResetJump() => readyToJump = true;

    // Checks whether the player is on a slope
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    // Returns a vector representing the slope movement direction
    private Vector3 GetSlopeDirection() =>
        Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
}
