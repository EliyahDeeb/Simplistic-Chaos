using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed = 2f;
    public float fallThreshold = -0.1f;
    public float attackRange = 2f;
    public float attackDashSpeed = 5f;
    public float attackCooldown = 3f;
    public float attackDamage = 10f;
    public GameObject bloodParticlePrefab;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Health health;

    private bool isFacingRight = true;
    private bool isAttacking = false;
    private bool canAttack = true;
    private float cooldownTimer = 0f;
    public static int defeatedEnemies = 0;
    private bool hasDied = false;  // Prevents double counting

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();

        health.onHealthChanged += UpdateHealthUI;
        health.onDeath += HandleDeath;
    }

    void Update()
    {
        if (player == null) return;

        if (!isAttacking && canAttack && Vector2.Distance(transform.position, player.transform.position) <= attackRange)
        {
            StartCoroutine(PerformAttack());
        }
        else if (!isAttacking)
        {
            float direction = player.transform.position.x - transform.position.x;

            if (Mathf.Abs(direction) > 0.1f)
            {
                animator.SetBool("EnemyIsRunning", true);
                rb.velocity = new Vector2(Mathf.Sign(direction) * moveSpeed, rb.velocity.y);
                if (direction > 0 && !isFacingRight)
                    Flip();
                else if (direction < 0 && isFacingRight)
                    Flip();
            }
            else
            {
                animator.SetBool("EnemyIsRunning", false);
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        animator.SetBool("EnemyIsFalling", rb.velocity.y < fallThreshold);

        if (!canAttack)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown)
            {
                canAttack = true;
                cooldownTimer = 0f;
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        canAttack = false;
        animator.SetTrigger("EnemyIsAttacking");
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);

        Vector2 dashDirection = (player.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(dashDirection.x * attackDashSpeed, rb.velocity.y);
        yield return new WaitForSeconds(0.5f);

        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
        {
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(attackDamage);
            }
        }

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void HandleDeath()
    {
        if (hasDied) return;  // Prevents double counting

        hasDied = true;
        defeatedEnemies++;
        Debug.Log("Enemy defeated! Total: " + defeatedEnemies);

        if (bloodParticlePrefab != null)
        {
            Instantiate(bloodParticlePrefab, transform.position, Quaternion.identity);
        }

        rb.velocity = Vector2.zero;
        StartCoroutine(DisableAfterDeath());
    }

    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
    }

    public void TakeDamage(float damage)
    {
        health.TakeDamage(damage);
    }
}
