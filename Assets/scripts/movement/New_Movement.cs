using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class New_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float crouchSpeed = 4f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    public Camera playerCamera;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public KeyCode crouchKey = KeyCode.LeftControl;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Slider sensitivitySlider;  // Optional UI sensitivity slider

    private CharacterController controller;
    private bool isCrouching = false;

    // velocity.y holds vertical velocity including jump and gravity
    public Vector3 velocity;

    private bool isGrounded;
    private float originalCameraY;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!playerCamera) Debug.LogError("Player Camera not assigned!");
        originalCameraY = playerCamera.transform.localPosition.y;

        if (sensitivitySlider != null)
            sensitivitySlider.value = PlayerPrefs.GetFloat("currentSensitivity", 6f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckGround();

        HandleJump();

        HandleMovement();

        HandleCrouch();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;
        float speed = isCrouching ? crouchSpeed : moveSpeed;
        Vector3 move = transform.TransformDirection(inputDir) * speed;

        // Apply gravity to vertical velocity
        velocity.y += gravity * Time.deltaTime;

        // Reset vertical velocity if grounded and falling
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small negative to keep grounded
        }

        // Combine horizontal and vertical velocities
        Vector3 finalVelocity = move + Vector3.up * velocity.y;

        controller.Move(finalVelocity * Time.deltaTime);
    }

    void HandleJump()
    {
        // Normal jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            if (isCrouching && CanStandUp()) Uncrouch();
            else if (!isCrouching) Crouch();
        }
    }

    bool CanStandUp()
    {
        float checkDistance = standingHeight - crouchingHeight;
        Vector3 rayOrigin = transform.position + Vector3.up * (crouchingHeight * 0.5f);
        return !Physics.Raycast(rayOrigin, Vector3.up, checkDistance);
    }

    void Crouch()
    {
        isCrouching = true;
        controller.height = crouchingHeight;

        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = originalCameraY - 0.5f;
        playerCamera.transform.localPosition = camPos;
    }

    void Uncrouch()
    {
        isCrouching = false;
        controller.height = standingHeight;

        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = originalCameraY;
        playerCamera.transform.localPosition = camPos;
    }

    // Called by grappling script to apply vertical launch velocity
    public void AddLaunchVelocity(Vector3 launchVelocity)
    {
        // Add launch velocity directly to vertical velocity, allowing it to override current falling velocity if stronger
        velocity.y = Mathf.Max(velocity.y, launchVelocity.y);
    }
}
