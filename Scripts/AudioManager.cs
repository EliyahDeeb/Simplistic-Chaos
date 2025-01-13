using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioClip bossMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBossMusic()
    {
        if (bossMusic != null && musicSource != null)
        {
            musicSource.clip = bossMusic;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Boss music or AudioSource is missing.");
        }
    }
}
