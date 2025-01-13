using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossController bossController;
    public GameObject healthBarUI;

    private void Start()
    {
        if (bossController == null)
        {
            bossController = FindObjectOfType<BossController>();
        }

        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);
        }
        else
        {
            Debug.LogError("HealthBar UI is not assigned in the BossTrigger!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the boss fight trigger area!");

            if (healthBarUI != null)
            {
                healthBarUI.SetActive(true);
            }

            if (bossController != null)
            {
                bossController.StartBossFight();
            }
            else
            {
                Debug.LogError("BossController is not assigned or found in the scene!");
            }

            // Play boss music if AudioManager exists
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBossMusic();
            }
            else
            {
                Debug.LogError("AudioManager instance not found in the scene!");
            }

            gameObject.SetActive(false);  // Disable trigger after activation
        }
    }
}
