using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class New_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float crouchSpeed = 4f;
    public float turnSpeed = 6f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    public AudioSource WalkingSFX;

    public Camera playerCamera;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public KeyCode crouchKey = KeyCode.LeftControl;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private float cameraPitch = 0f;
    private float originalCameraY;
    private bool isCrouching = false;
    private bool isGrounded;

    private PlayerGrapplingController grapplingController;

    public Vector3 velocity = Vector3.zero; // movement velocity including grapple launch

    [HideInInspector]
    public bool freezeMovement = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (!playerCamera) Debug.LogError("Player Camera not assigned!");
        originalCameraY = playerCamera.transform.localPosition.y;

        grapplingController = GetComponent<PlayerGrapplingController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();

        if (freezeMovement)
            return;

        CheckGround();
        HandleJump();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * turnSpeed;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;
        float speed = isCrouching ? crouchSpeed : moveSpeed;

        Vector3 move = transform.TransformDirection(inputDir) * speed;

        // Apply grapple launch velocity if any
        if (grapplingController != null && grapplingController.pendingVelocity != Vector3.zero)
        {
            velocity = grapplingController.pendingVelocity;
            grapplingController.pendingVelocity = Vector3.zero;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        if (isGrounded)
        {
            if (inputDir.magnitude < 0.1f)
            {
                // Smoothly reduce horizontal velocity to zero when no input
                velocity.x = Mathf.Lerp(velocity.x, 0f, 10f * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, 0f, 10f * Time.deltaTime);
            }
            else
            {
                // Player input overrides grapple horizontal velocity
                velocity.x = 0f;
                velocity.z = 0f;
            }

            if (velocity.y < 0)
                velocity.y = -2f;  // Keep player grounded
        }

        // Move horizontally by input + grapple horizontal velocity
        controller.Move((move + new Vector3(velocity.x, 0f, velocity.z)) * Time.deltaTime);

        // Move vertically by vertical velocity (gravity, jump, grapple)
        controller.Move(Vector3.up * velocity.y * Time.deltaTime);

        // Walking SFX control
        if (isGrounded && inputDir.magnitude > 0.1f)
        {
            if (!WalkingSFX.isPlaying)
                WalkingSFX.Play();
        }
        else
        {
            if (WalkingSFX.isPlaying)
                WalkingSFX.Pause();
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
}
