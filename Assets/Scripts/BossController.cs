using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public float dashSpeed = 20f;
    public float dashCooldown = 1f;
    public float leapHeight = 5f;
    public float teleportRange = 1f;
    public Transform player;
    public Slider healthBar;
    public GameObject bloodParticlePrefab;
    public float attackDamage = 20f;
    public bool isBoss = true;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDashing = false;
    private bool isTeleporting = false;
    private bool isStaggered = false;
    private bool hasDied = false;
    private float nextDashTime = 0f;
    private float staggerDuration = 10f;

    public static int defeatedEnemies = 0;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip dashSFX;
    public AudioClip teleportSFX;
    public AudioClip dropKickSFX;
    public AudioClip deathSFX;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;

        if (healthBar == null)
        {
            Debug.LogError("HealthBar is not assigned in the BossController!");
            return;
        }

        healthBar.maxValue = 1;
        UpdateHealthBar();
    }

    void Update()
    {
        FlipTowardsPlayer();
    }

    public void StartBossFight()
    {
        Debug.Log("Boss Fight Started!");
        StartCoroutine(StartAttackCycle());
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
        }
    }

    void ChooseAttack()
    {
        if (isStaggered) return;

        int attackChoice = Random.Range(0, 3);  // Increased range for more variety

        if (attackChoice == 0)
        {
            StartCoroutine(DashAttack());
        }
        else if (attackChoice == 1)
        {
            StartCoroutine(TeleportDropKick());
        }
        else
        {
            // Randomly chain both attacks for chaos
            StartCoroutine(DashAttack());
            StartCoroutine(TeleportDropKick());
        }
    }

    IEnumerator DashAttack()
    {
        isDashing = true;
        animator.SetTrigger("BossDash");

        if (dashSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(dashSFX);
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector2.zero;
        isDashing = false;
        nextDashTime = Time.time + dashCooldown;
    }

    IEnumerator TeleportDropKick()
    {
        isTeleporting = true;
        animator.SetTrigger("BossLeap");
        rb.velocity = new Vector2(rb.velocity.x, leapHeight);

        if (teleportSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSFX);
        }

        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));  // Randomized delay for unpredictability
        transform.position = player.position + new Vector3(Random.Range(-teleportRange, teleportRange), 0, 0);
        animator.SetTrigger("BossDropKick");

        if (dropKickSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(dropKickSFX);
        }

        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector2.zero;
        isTeleporting = false;
        nextDashTime = Time.time + dashCooldown;
    }

    public void TakeDamage(float damage)
    {
        if (hasDied) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();
        Debug.Log($"Boss Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (health <= 0) ? 0 : health / maxHealth;
        }
    }

    IEnumerator Stagger()
    {
        isStaggered = true;
        animator.SetBool("isStaggered", true);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(staggerDuration);

        isStaggered = false;
        animator.SetBool("isStaggered", false);

        StartCoroutine(StartAttackCycle());
    }

    IEnumerator StartAttackCycle()
    {
        float attackEndTime = Time.time + 10f;

        while (Time.time < attackEndTime && !isStaggered)
        {
            ChooseAttack();
            yield return new WaitForSeconds(Random.Range(0.8f, 0.8f));  // Vary the attack frequency
        }

        StartCoroutine(Stagger());
    }

    void Die()
    {
        if (hasDied) return;

        hasDied = true;
        defeatedEnemies++;

        if (deathSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSFX);
        }

        Instantiate(bloodParticlePrefab, transform.position, Quaternion.identity);
        Destroy(healthBar.gameObject);
        Destroy(gameObject);

        if (isBoss && LevelManager.Instance != null)
        {
            LevelManager.Instance.CompleteLevel();
            Debug.Log("Boss defeated - Level Complete!");
        }
    }

    void FlipTowardsPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((isDashing || isTeleporting) && collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<Health>(out Health playerHealth))
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("Boss hit the player!");
            }
        }
    }
}
