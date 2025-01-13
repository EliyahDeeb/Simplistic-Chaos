using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20;  // Amount of health restored
    public int hpGain = 1;       // Amount of health points (HP) gained

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            
            if (playerHealth != null)
            {
                // Heal the player if not at max health
                if (playerHealth.GetCurrentHealth() < playerHealth.GetMaxHealth())
                {
                    playerHealth.ModifyHealth(healAmount);
                }
                else
                {
                    // Add HP if the player is already full health
                    playerHealth.GainHealthPoint(hpGain);
                }

                Debug.Log("Player collected health pickup!");
                Destroy(gameObject);
            }
        }
    }
}
