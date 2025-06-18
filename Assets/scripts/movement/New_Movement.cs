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
    public Vector3 velocity;
    private bool isGrounded;

    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        

        if (!playerCamera) Debug.LogError("Player Camera not assigned!");
        originalCameraY = playerCamera.transform.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        
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

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move((move + velocity) * Time.deltaTime);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 🎵 Control walking SFX
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
