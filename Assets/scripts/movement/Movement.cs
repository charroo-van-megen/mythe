using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 10f; // Movement speed
    public float crouchSpeed = 4f; // Crouch movement speed
    public float turnSpeed = 8f; // Speed of turning
    public Camera playerCamera; // Camera to determine direction
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public Rigidbody rb;

    private float cameraPitch = 0f; // Camera pitch angle
    [SerializeField] private bool isCrouching = false;
    private float originalCameraY;

    void Start()
    {
        originalCameraY = playerCamera.transform.localPosition.y;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleCrouch();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * turnSpeed;

        // Rotate character left/right (yaw)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down (pitch)
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ) * moveSpeed;

        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        rb.AddForce(movement, ForceMode.Force);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            if (isCrouching)
            {
                // Attempt to stand up
                float headClearance = (standingHeight - crouchingHeight) * 0.5f;
                Vector3 rayOrigin = transform.position + Vector3.up * (crouchingHeight * 0.5f);
                bool isBlocked = Physics.Raycast(rayOrigin, Vector3.up, headClearance);

                if (!isBlocked)
                {
                    // Safe to stand
                    isCrouching = false;

                    // Reset camera height
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
                    // Still crouching due to obstruction
                    Debug.Log("Cannot stand up, something is above!");
                }
            }
            else
            {
                // Start crouching
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