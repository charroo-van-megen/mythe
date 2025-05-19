using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    public float moveSpeed = 10f;             // Movement speed when standing
    public float crouchSpeed = 4f;            // Movement speed when crouching
    public float turnSpeed = 8f;              // Mouse look speed
    public Camera playerCamera;               // Camera for looking and positioning
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public KeyCode crouchKey = KeyCode.LeftControl;

    private Rigidbody rb;
    private float cameraPitch = 0f;
    private float originalCameraY;
    private bool isCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent rotation by physics

        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not assigned!");
        }

        originalCameraY = playerCamera.transform.localPosition.y;
    }

    void Update()
    {
        HandleMouseLook();
        HandleCrouch();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * turnSpeed;

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        Vector3 moveDir = transform.TransformDirection(inputDir) * currentSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + moveDir);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            if (isCrouching)
            {
                float headClearance = (standingHeight - crouchingHeight) * 0.5f;
                Vector3 rayOrigin = transform.position + Vector3.up * (crouchingHeight * 0.5f);
                bool isBlocked = Physics.Raycast(rayOrigin, Vector3.up, headClearance);

                if (!isBlocked)
                {
                    isCrouching = false;

                    // Reset camera position
                    Vector3 camPos = playerCamera.transform.localPosition;
                    camPos.y = originalCameraY;
                    playerCamera.transform.localPosition = camPos;

                    // Reset scale
                    Vector3 scale = transform.localScale;
                    scale.y = 1f;
                    transform.localScale = scale;
                }
                else
                {
                    Debug.Log("Cannot stand up — something is blocking above.");
                }
            }
            else
            {
                isCrouching = true;

                // Lower camera
                Vector3 camPos = playerCamera.transform.localPosition;
                camPos.y = originalCameraY - 0.5f;
                playerCamera.transform.localPosition = camPos;

                // Scale down
                Vector3 scale = transform.localScale;
                scale.y = crouchingHeight / standingHeight;
                transform.localScale = scale;
            }
        }
    }
}
