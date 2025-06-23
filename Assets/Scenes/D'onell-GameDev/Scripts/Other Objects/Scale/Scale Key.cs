using UnityEngine;
using UnityEngine.UIElements;

public class ScaleKey : MonoBehaviour
{
    public GameObject Lock;
    public GameObject PlaceScaleText;
    public GameObject Scale;
    public GameObject TwoScale;
    public GameObject ThreeScale;
    public GameObject CannotPlaceScaleText;

    //scales on lock
    public GameObject  ScaleKeyShowOne;
    public GameObject ScaleKeyShowTwo;
    public GameObject ScaleKeyShowThree;

    //for handling the keys
    public GameObject[] scalesInHand;
    public GameObject[] scalesOnPedestal;

    public int GivenAKey;

    void Start()
    {
        Locked.locked = true;

        GivenAKey = 0;

        PlaceScaleText.SetActive(false);
        CannotPlaceScaleText.SetActive(false);
        ScaleKeyShowOne.SetActive(false);
        ScaleKeyShowTwo.SetActive(false);
        ScaleKeyShowThree.SetActive(false);

        scalesInHand = new GameObject[] { Scale, TwoScale, ThreeScale };
        scalesOnPedestal = new GameObject[] { ScaleKeyShowOne, ScaleKeyShowTwo, ScaleKeyShowThree };
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If already unlocked, don't show any interaction prompts
            if (!Locked.locked)
                return;

            // If player has at least 1 key
            if (Inventory.Keys > 0)
            {
                PlaceScaleText.SetActive(true);
                CannotPlaceScaleText.SetActive(false);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    
                    GivenAKey++;
                    Inventory.Keys--;
                    Debug.Log("Placed a scale! GivenAKey: " + GivenAKey);

                    for (int i = 0; i < scalesInHand.Length; i++)
                    {
                        scalesInHand[i].SetActive(  (i+1)<= Inventory.Keys);
                        scalesOnPedestal[i].SetActive((i + 1) <= GivenAKey);
                    }

                }

                if (GivenAKey == 3)
                {
                    Locked.locked = false;
                    PlaceScaleText.SetActive(false);
                    CannotPlaceScaleText.SetActive(false);
                    
                    Debug.Log("You Unlocked the gate!");
                }
            }

            

            else
            {
                // Not enough keys
                if (Locked.locked && GivenAKey < 3)
                {
                    PlaceScaleText.SetActive(false);
                    CannotPlaceScaleText.SetActive(true);
                }
            }
        }
        
        
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlaceScaleText.SetActive(false);
            CannotPlaceScaleText.SetActive(false);
        }
    }
}

public class Locked
{
    public static bool locked;
}
