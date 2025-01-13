using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;    // Reference to the Animator component
    private Rigidbody2D rb;       // Reference to the Rigidbody2D component

    // Attack-related variables
    private int currentAttackIndex = 0; // Tracks which attack in the sequence
    private float attackCooldown = 0.5f; // Cooldown between attacks
    private float attackCooldownTimer = 0f; // Timer for attack cooldown
    private bool canAttack = true; // Whether the player can attack
    public int totalAttacks = 3; // Number of attacks in the sequence

    private bool isLockedInAttack = false; // Lock player movement during attack

    // Damage-related variables
    public float attackDamage = 10f; // Damage dealt by each attack
    public Transform attackPoint; // Point from which attacks are cast
    public float attackRange = 1f; // Range of the attack
    public LayerMask enemyLayer; // Layer mask to detect enemies

    // Reference to PlayerSoundManager
    public PlayerSoundManager playerSoundManager;  // Make sure this is linked in the Inspector

    // Reset timer variables
    private Coroutine resetToIdleCoroutine;
    private float resetDelay = 2f; // Time to reset to idle after attack

    void Start()
    {
        // Get the Animator and Rigidbody2D components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Check if the playerSoundManager is assigned (either in script or via Inspector)
        if (playerSoundManager == null)
        {
            Debug.LogWarning("PlayerSoundManager is not assigned! Make sure it's linked in the Inspector.");
        }
    }

    void Update()
    {
        // Handle attack cooldown
        if (!canAttack)
        {
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer >= attackCooldown)
            {
                canAttack = true; // Reset ability to attack
                attackCooldownTimer = 0f; // Reset attack cooldown timer
            }
        }

        // Prevent movement during attack lock
        if (isLockedInAttack) return;

        // Handle attack input
        if (Input.GetButtonDown("Fire1") && canAttack) // Left click for attack
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        currentAttackIndex = (currentAttackIndex % totalAttacks) + 1; // Cycle through attacks
        animator.SetInteger("AttackIndex", currentAttackIndex); // Set the current attack index in the Animator

        canAttack = false; // Prevent further attacks until cooldown ends
        isLockedInAttack = true; // Lock movement during attack

        ApplyDamage(); // Apply damage to enemies

        // Play attack sound if the sound manager is assigned
        if (playerSoundManager != null)
        {
            playerSoundManager.PlayAttackSound(); // Play the attack sound
        }

        StartCoroutine(EndAttackLock(0.5f)); // Lock for 0.5 seconds

        // Reset to idle after delay if no further input
        if (resetToIdleCoroutine != null)
        {
            StopCoroutine(resetToIdleCoroutine);
        }
        resetToIdleCoroutine = StartCoroutine(ResetToIdleAfterDelay(resetDelay));
    }

    private IEnumerator ResetToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetInteger("AttackIndex", 0);  // Reset attack index to idle in Animator
        currentAttackIndex = 0;  // Reset attack index in script
    }

    private void ApplyDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            // Damage regular enemies
            if (enemy.TryGetComponent<EnemyBehavior>(out EnemyBehavior enemyBehavior))
            {
                enemyBehavior.TakeDamage(attackDamage);
                Debug.Log("Enemy hit by player!");
            }

            // Damage the boss if detected
            if (enemy.CompareTag("Boss"))  // Target boss by tag
            {
                if (enemy.TryGetComponent<BossController>(out BossController boss))
                {
                    boss.TakeDamage(attackDamage);  // Apply damage to boss
                    Debug.Log("Boss hit by player!");
                }
            }
        }
    }

    private IEnumerator EndAttackLock(float lockDuration)
    {
        yield return new WaitForSeconds(lockDuration); // Wait for the lock duration
        isLockedInAttack = false; // Unlock movement

        if (currentAttackIndex == totalAttacks)
        {
            currentAttackIndex = 0; // Reset attack sequence
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
