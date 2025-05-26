using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float speed = 20f; // (Optional: You can use this for jump force if you want)
    public Rigidbody rb;
    private bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Set upward velocity to make the player jump
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 5f, rb.linearVelocity.z);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only reset isGrounded if we hit an object tagged as "Ground"
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
