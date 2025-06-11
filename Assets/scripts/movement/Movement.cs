using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float crouchSpeed = 4f;
    public float jumpForce = 6f;

    [Header("Mouse Look Settings")]
    public float turnSpeed = 8f;  // Sensitivity multiplier
    public Slider sensitivitySlider;  // Assign in Inspector

    [Header("Camera & Crouch")]
    public Camera playerCamera;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Tooltip("Assign a Transform placed slightly below the player")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private float cameraPitch = 0f;
    private float originalCameraY;
    private bool isCrouching = false;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Auto-assign MainCamera if not assigned
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                Debug.LogError("No camera assigned and no MainCamera found in scene!");
        }

        if (!groundCheck)
            Debug.LogError("GroundCheck transform not assigned!");

        // Load saved sensitivity (default 8)
        turnSpeed = PlayerPrefs.GetFloat("currentSensitivity", 8f);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 1f;
            sensitivitySlider.maxValue = 20f;
            sensitivitySlider.wholeNumbers = false;
            sensitivitySlider.value = turnSpeed;
            sensitivitySlider.onValueChanged.AddListener(AdjustSensitivity);
        }

        originalCameraY = playerCamera.transform.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleCrouch();
        CheckGround();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * turnSpeed;

        // Rotate player body left-right
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up-down
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

        Vector3 move = transform.TransformDirection(inputDir) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if (isCrouching)
            {
                if (CanStandUp())
                    Uncrouch();
                else
                    return;
            }

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            if (isCrouching)
            {
                if (CanStandUp())
                    Uncrouch();
            }
            else
            {
                Crouch();
            }
        }
    }

    void CheckGround()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
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

        Vector3 scale = transform.localScale;
        scale.y = crouchingHeight / standingHeight;
        transform.localScale = scale;

        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = originalCameraY - 0.5f;
        playerCamera.transform.localPosition = camPos;
    }

    void Uncrouch()
    {
        isCrouching = false;

        Vector3 scale = transform.localScale;
        scale.y = 1f;
        transform.localScale = scale;

        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = originalCameraY;
        playerCamera.transform.localPosition = camPos;
    }

    public void AdjustSensitivity(float newSensitivity)
    {
        turnSpeed = newSensitivity;
        PlayerPrefs.SetFloat("currentSensitivity", turnSpeed);
        PlayerPrefs.Save();
        Debug.Log("Sensitivity updated to: " + turnSpeed);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
