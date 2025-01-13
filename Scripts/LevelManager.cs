using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public int enemiesToDefeat = 10; // Number of enemies required to complete the level
    public GameObject levelCompletePanel; // Panel for the level complete screen
    public TMP_Text scoreText;  // Change this to TMP_Text
    public TMP_Text timerText;  // Change this to TMP_Text


    private float timer = 0f;
    private bool levelComplete = false; // This flag ensures the level complete UI is shown only once

    void Start()
    {
        // Ensure the panel is hidden at the start of the level
        if (levelCompletePanel == null) Debug.LogError("levelCompletePanel is not assigned!");
        if (scoreText == null) Debug.LogError("scoreText is not assigned!");
        if (timerText == null) Debug.LogError("timerText is not assigned!");

        levelCompletePanel.SetActive(false);
        timer = 0f;
        Time.timeScale = 1f; // Ensure the game starts unpaused
    }


    void Update()
    {
        if (!levelComplete)
        {
            timer += Time.deltaTime;

            // Check if the required number of enemies are defeated
            if (EnemyBehavior.defeatedEnemies >= enemiesToDefeat)
            {
                CompleteLevel(); // Call CompleteLevel when the player defeats the required number of enemies
            }

            // Update the timer UI if the level is not complete
            UpdateTimerUI();
        }
    }

    public void CompleteLevel()
    {
        levelComplete = true; // Mark the level as complete to avoid repeated calls
        levelCompletePanel.SetActive(true); // Show the level complete panel
        Time.timeScale = 0f; // Pause the game upon level completion

        // Update UI elements with the current score and timer
        scoreText.text = "Score: " + EnemyBehavior.defeatedEnemies;
        timerText.text = "Time: " + Mathf.FloorToInt(timer) + "s";

        Debug.Log("Level Complete! Score: " + EnemyBehavior.defeatedEnemies + ", Time: " + Mathf.FloorToInt(timer));
    }

    public void NextLevel()
    {
        // Reset the defeated enemies count for the next level
        EnemyBehavior.defeatedEnemies = 0;
        Time.timeScale = 1f; // Resume the game

        // Get the index of the next scene
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Check if the next scene exists, otherwise go to the main menu
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels! Returning to main menu.");
            SceneManager.LoadScene("MainMenu"); // Go to main menu if no more levels
        }
    }

    public void RestartLevel()
    {
        // Reset the defeated enemies count and reload the current scene
        EnemyBehavior.defeatedEnemies = 0;
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        // Reset the defeated enemies count and go to the main menu
        EnemyBehavior.defeatedEnemies = 0;
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene("MainMenu");
    }

    void UpdateTimerUI()
    {
        // Update the timer display on the UI
        timerText.text = "Time: " + Mathf.FloorToInt(timer) + "s";
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
