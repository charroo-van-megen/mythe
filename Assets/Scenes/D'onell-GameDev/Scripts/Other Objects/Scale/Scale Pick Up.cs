using UnityEngine;

public class ScalePickUp : MonoBehaviour
{
    public GameObject Scale;
    public GameObject TwoScale;
    public GameObject ThreeScale;
    public GameObject PickUpScaleText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Scale.SetActive(false);
        TwoScale.SetActive(false);
        ThreeScale.SetActive(false);

        PickUpScaleText.SetActive(false);

        Inventory.Keys = 0;
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
// COLLISION DETECTION
        if(other.gameObject.tag == "Player")
        {
            PickUpScaleText.SetActive(true);

// PICK UP SCALE
            if(Input.GetKeyDown(KeyCode.E))
            {
                this.gameObject.SetActive(false);

                PickUpScaleText.SetActive(false);

                Inventory.Keys += 1;

                if(Inventory.Keys == 1)
                {
                    Scale.gameObject.SetActive(true);
                }

                if (Inventory.Keys == 2)
                {
                    TwoScale.gameObject.SetActive(true);
                }

                if (Inventory.Keys == 3)
                {
                    ThreeScale.gameObject.SetActive(true);
                }

                Debug.Log("You got a scale!" + Inventory.Keys);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PickUpScaleText.SetActive(false);
    }
}
