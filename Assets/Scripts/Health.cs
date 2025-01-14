using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;  // Maximum health
    public int healthPoints = 3;    // Extra health points (lives)
    private float currentHealth;

    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public event OnHealthChanged onHealthChanged;

    public delegate void OnDeath();
    public event OnDeath onDeath;

    private void Start()
    {
        currentHealth = maxHealth;  // Initialize health
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Ensure health doesn't drop below 0 or exceed max

        onHealthChanged?.Invoke(currentHealth, maxHealth);  // Trigger health change event

        if (currentHealth <= 0)
        {
            if (healthPoints > 0)
            {
                UseHealthPoint();  // Consume a health point if available
            }
            else
            {
                Die();
            }
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void GainHealthPoint(int amount)
    {
        healthPoints += amount;
        Debug.Log("Gained " + amount + " HP. Total HP: " + healthPoints);
    }

    private void UseHealthPoint()
    {
        healthPoints--;
        currentHealth = maxHealth;  // Restore to full health
        Debug.Log("Used 1 health point. Remaining HP: " + healthPoints);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        onDeath?.Invoke();  // Trigger death event
        Destroy(gameObject);  // Destroy this object (can be replaced with animations or other effects)
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // Add this to the Health script
    public void ModifyHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);  // Notify UI of health changes
    }

}
