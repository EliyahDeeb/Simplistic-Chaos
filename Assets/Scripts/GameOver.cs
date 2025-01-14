using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel;

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectWithTag("Player") == null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void Restart()
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloads the current scene
    }

    public void MainMenu()
    {
       SceneManager.LoadScene("MainMenu"); // Load the Main Menu scene (ensure it's named correctly in your build settings)
    }
}
