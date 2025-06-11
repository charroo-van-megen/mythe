using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float crouchSpeed = 4f;
    public float turnSpeed = 8f; // This is the value we will control with the slider
    public float jumpForce = 6f;

    public Camera playerCamera;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Tooltip("Assign a Transform placed slightly below the player")]
    public Transform groundCheck;

    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Slider sensitivitySlider; // Assign this in the Inspector

    private Rigidbody rb;
    private float cameraPitch = 0f;
    private float originalCameraY;
    private bool isCrouching = false;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (!playerCamera)
            Debug.LogError("Player Camera not assigned!");

        if (!groundCheck)
            Debug.LogError("GroundCheck transform not assigned!");

        // Load saved sensitivity
        turnSpeed = PlayerPrefs.GetFloat("currentSensitivity", 8f);

        // Update slider if assigned
        if (sensitivitySlider != null)
            sensitivitySlider.value = turnSpeed / 10f;

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
                {
                    Debug.Log("Blocked above, cannot uncrouch to jump.");
                    return;
                }
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
                else
                    Debug.Log("Blocked above, cannot stand up.");
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
        float checkDistance = (standingHeight - crouchingHeight);
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

    // 📌 Called by the Slider OnValueChanged
    public void AdjustSpeed(float newSpeed)
    {
        turnSpeed = newSpeed * 10f;
        PlayerPrefs.SetFloat("currentSensitivity", turnSpeed);
        PlayerPrefs.Save();
        Debug.Log("Updated turnSpeed to: " + turnSpeed);
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
