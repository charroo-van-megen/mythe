using UnityEngine;
using UnityEngine.SceneManagement;

public class ToCredits : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("End Credits");
    }
}
