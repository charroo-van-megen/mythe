using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;

    public GameObject projectilePrefab;

    private float lastInput = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError("Rigidbody component missing!");

        if (groundCheck == null)
            Debug.LogError("GroundCheck is not assigned in Inspector!");

        if (projectilePrefab == null)
            Debug.LogError("Projectile prefab is not assigned in Inspector!");
    }

    void Update()
    {
        // Optional: Ground check (can be removed if not used anywhere else)
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // Track direction for shooting
        float moveX = Input.GetAxis("Horizontal");
        if (moveX != 0)
            lastInput = moveX;

        // Shoot projectile
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

        Destroy(projInstance, 3f);
    }
}
