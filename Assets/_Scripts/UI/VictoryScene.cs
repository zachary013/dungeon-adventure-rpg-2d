using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScene : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(1); // Assuming the game scene is index 1
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0); // Assuming the main menu is index 0
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
