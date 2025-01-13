using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public AudioSource playerAudioSource;  // The AudioSource attached to the player
    public AudioClip walkSound;           // Sound for walking
    public AudioClip jumpSound;           // Sound for jumping
    public AudioClip dashSound;           // Sound for dashing
    public AudioClip attackSound;         // Sound for attacking

    private bool isWalking = false;       // To track walking state

    void Start()
    {
        // Ensure AudioSource is assigned
        if (playerAudioSource == null)
        {
            playerAudioSource = GetComponent<AudioSource>(); // Automatically get AudioSource if not assigned
        }

        if (playerAudioSource == null)
        {
            Debug.LogError("AudioSource is not assigned on " + gameObject.name);
        }
        else
        {
            Debug.Log("AudioSource successfully assigned to " + gameObject.name);
        }
    }

    // Call this method when the player starts walking
    public void StartWalking()
    {
        if (!isWalking && walkSound != null)
        {
            isWalking = true;
            playerAudioSource.loop = true; // Loop the walking sound
            playerAudioSource.clip = walkSound;
            playerAudioSource.Play();
            Debug.Log("Playing walking sound");
        }
        else if (walkSound == null)
        {
            Debug.LogWarning("Walking sound is not assigned.");
        }
    }

    // Call this method when the player stops walking
    public void StopWalking()
    {
        if (isWalking)
        {
            isWalking = false;
            playerAudioSource.loop = false;
            playerAudioSource.Stop();
            Debug.Log("Stopped walking sound");
        }
    }

    // Call this method when the player jumps
    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            playerAudioSource.PlayOneShot(jumpSound);
            Debug.Log("Playing jump sound");
        }
        else
        {
            Debug.LogWarning("Jump sound is not assigned.");
        }
    }

    // Call this method when the player dashes
    public void PlayDashSound()
    {
        if (dashSound != null)
        {
            playerAudioSource.PlayOneShot(dashSound);
            Debug.Log("Playing dash sound");
        }
        else
        {
            Debug.LogWarning("Dash sound is not assigned.");
        }
    }

    // Call this method when the player attacks
    public void PlayAttackSound()
    {
        if (attackSound != null)
        {
            playerAudioSource.PlayOneShot(attackSound);
            Debug.Log("Playing attack sound");
        }
        else
        {
            Debug.LogWarning("Attack sound is not assigned.");
        }
    }
}
