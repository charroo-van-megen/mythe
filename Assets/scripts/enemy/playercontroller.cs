using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;

    // Projectile prefab without any custom script attached,
    // just a Rigidbody and Collider.
    public GameObject projectilePrefab;

    // Last horizontal input (-1 left, 1 right)
    private float lastInput = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (groundCheck == null)
            Debug.LogError("GroundCheck is not assigned in Inspector!");
        if (projectilePrefab == null)
            Debug.LogError("Projectile prefab is not assigned in Inspector!");
    }

    void Update()
    {
        // Movement Input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveX != 0)
            lastInput = moveX;

        // Move player - preserve Y velocity
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        // Ground check
        if (groundCheck != null)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump
        if (isGrounded && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        // Shoot projectile on left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null)
            return;

        Vector3 shootDirection = new Vector3(lastInput, 0f, 0f).normalized;
        if (shootDirection == Vector3.zero)
            shootDirection = transform.forward;

        Vector3 spawnPos = transform.position + shootDirection;

        // Instantiate the projectile prefab (no custom script needed)
        GameObject projInstance = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(shootDirection));

        Rigidbody projRb = projInstance.GetComponent<Rigidbody>();
        if (projRb != null)
        {
            projRb.AddForce(shootDirection * 10f, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Projectile prefab missing Rigidbody component.");
        }

        // Optionally destroy the projectile after some time so it doesn't persist forever
        Destroy(projInstance, 3f);
    }
}
