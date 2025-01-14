using UnityEngine;

public class BossAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("Boss SFX")]
    public AudioClip dashSFX;
    public AudioClip teleportSFX;
    public AudioClip dropKickSFX;
    public AudioClip deathSFX;

    public static BossAudioManager Instance;

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist between scenes if needed
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure the audio source is not null
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource not assigned. Automatically added one.");
        }
    }

    public void PlayDashSound()
    {
        PlaySFX(dashSFX, "Dash");
    }

    public void PlayTeleportSound()
    {
        PlaySFX(teleportSFX, "Teleport");
    }

    public void PlayDropKickSound()
    {
        PlaySFX(dropKickSFX, "Drop Kick");
    }

    public void PlayDeathSound()
    {
        PlaySFX(deathSFX, "Death");
    }

    // Centralized method to play sound effects with optional debug logging
    private void PlaySFX(AudioClip clip, string actionName)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Failed to play {actionName} sound. Missing AudioClip or AudioSource.");
        }
    }
}
