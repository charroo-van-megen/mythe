using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
