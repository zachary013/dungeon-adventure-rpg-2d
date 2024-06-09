using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameOverMenu; // Reference to the Game Over menu UI

    private void Start()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false); // Ensure the Game Over menu is inactive at start
        }
    }

    public void ShowGameOverMenu()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
            Time.timeScale = 0; // Pause the game
        }
        else
        {
            Debug.LogWarning("GameOverMenu GameObject is not assigned.");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
