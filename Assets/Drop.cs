using UnityEngine;

public class Drop : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DropIt() {
        Debug.Log("drop");
        rb.isKinematic = false;
        GetComponent<BoxCollider>().enabled = false;
    }
}
