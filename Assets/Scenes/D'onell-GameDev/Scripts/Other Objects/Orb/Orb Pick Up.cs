using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrbPickUp : MonoBehaviour
{
    public GameObject PickUpOrbText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PickUpOrbText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == ("Player"))
        {
            PickUpOrbText.SetActive(true);

            if(Input.GetKey(KeyCode.E))
            {
                SceneManager.LoadScene("End Credits");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == ("Player"))
        {
            PickUpOrbText.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
