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
    public GameObject ScaleKeyShowOne;
    public GameObject ScaleKeyShowTwo;
    public GameObject ScaleKeyShowThree;

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
                    Debug.Log("Placed a scale! GivenAKey: " + GivenAKey);

                    if (Inventory.Keys == 1)
                    {
                        Scale.gameObject.SetActive(false);
                        ScaleKeyShowOne.SetActive(true);
                    
                    }
                                                 
                    if(Inventory.Keys == 2)
                    {
                        TwoScale.gameObject.SetActive(false);
                        ScaleKeyShowTwo.SetActive(true);
                    
                    }

                    if (Inventory.Keys == 3)
                    {
                        ThreeScale.gameObject.SetActive(false);
                        ScaleKeyShowThree.SetActive(true);
                    }
                    Inventory.Keys--;
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
