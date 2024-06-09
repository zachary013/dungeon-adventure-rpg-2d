using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance

    private int score = 0;
    [SerializeField] private TMP_Text scoreText; // Using TMP_Text instead of Text

    // Health potion variables
    private int healthPotionCount = 0;
    [SerializeField] private TMP_Text healthPotionText; // Using TMP_Text for health potion count
    private bool isGamePaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the GameManager persists across scenes if needed
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();
        UpdateHealthPotionUI();
    }

    // Method to add score
    public void AddScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }

    public int GetScore()
    {
        return score;
    }

    // Method to update the score UI
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    // Method to add health potion
    public void AddHealthPotion(int value)
    {
        healthPotionCount += value;
        UpdateHealthPotionUI();
    }

    public int GetHealthPotionCount()
    {
        return healthPotionCount;
    }

    // Method to update the health potion UI
    private void UpdateHealthPotionUI()
    {
        if (healthPotionText != null)
        {
            healthPotionText.text = healthPotionCount.ToString();
        }
    }

    // Method to pause the game
    public void PauseGame()
    {
        if (isGamePaused) return;

        Time.timeScale = 0; // Set time scale to 0 to pause game
        isGamePaused = true;

        // Add any other logic to pause enemy behavior, animations, etc.
        // Example: EnemyManager.instance.PauseEnemies();
    }

    // Method to resume the game
    public void ResumeGame()
    {
        if (!isGamePaused) return;

        Time.timeScale = 1; // Set time scale back to 1 to resume game
        isGamePaused = false;

        // Add any other logic to resume enemy behavior, animations, etc.
        // Example: EnemyManager.instance.ResumeEnemies();
    }

    // Method to pause the game for a specific duration in real-time seconds
    public void PauseGameForSeconds(float seconds)
    {
        StartCoroutine(PauseForSeconds(seconds));
    }

    private IEnumerator PauseForSeconds(float seconds)
    {
        PauseGame();
        yield return new WaitForSecondsRealtime(seconds);
        ResumeGame();
    }

    // Method to check if the game is paused
    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    // Method to load the victory scene
    public void LoadVictoryScene(float delay = 0f) // Add a parameter for delay, default is 0
    {
        StartCoroutine(LoadVictorySceneCoroutine(delay));
    }

    private IEnumerator LoadVictorySceneCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Wait for the specified delay
        SceneManager.LoadSceneAsync(2); // Load the victory scene by name
        Time.timeScale = 1;
    }
}
