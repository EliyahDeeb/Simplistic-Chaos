using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LevelTutorial"); // Replace with your actual game scene name
    }
}
