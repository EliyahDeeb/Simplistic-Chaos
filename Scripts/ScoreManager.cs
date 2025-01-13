using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton pattern
    public int score = 0;
    public int targetScore = 10;  // Score needed to proceed
    public string nextLevelName = "NextLevel"; // Name of the next scene

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score >= targetScore)
        {
            ProceedToNextLevel();
        }
    }

    private void ProceedToNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
    }
}
