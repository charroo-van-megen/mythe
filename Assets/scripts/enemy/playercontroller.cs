using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    public bool isGrounded;

    public Projectile projectile;

    // Last horizontal input direction (-1 left, 1 right)
    public float lastInput = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck transform is not assigned in the Inspector!", this);
        }
    }

    void Update()
    {
        // Get movement input on X (left/right) and Z (forward/back)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveX != 0 || moveZ != 0)
        {
            lastInput = moveX != 0 ? moveX : lastInput;
        }

        // Move player - keep current Y velocity (vertical)
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        // Ground check using sphere overlap
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // Jump on ground and press W or Space
        if (isGrounded && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        // Shoot projectile when pressing left mouse button or spacebar
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Vector3 shootDirection = new Vector3(lastInput, 0, 0).normalized;

            if (shootDirection == Vector3.zero)
                shootDirection = Vector3.forward; // default direction

            Projectile projectileInstance = Instantiate(projectile, transform.position + shootDirection, Quaternion.LookRotation(shootDirection));
            projectileInstance.rb.AddForce(shootDirection * 10f, ForceMode.Impulse);
        }
    }
}
