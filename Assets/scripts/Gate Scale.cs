using UnityEngine;

public class GateDebugLog : MonoBehaviour
{
    public bool scale = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            scale = true;

            Debug.Log("You got a Scale!");
        }

        if (scale == true && Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            Debug.Log("You opened the gate with your scale!");
        }

        if (scale == false && Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            Debug.Log("You don't have a scale yet!");
        }
    }
}