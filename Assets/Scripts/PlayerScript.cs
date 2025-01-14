using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float jumpForce = 10f; 
    public float jumpCooldown = 1.0f; 
    public float dashDistance = 10f; 
    public float dashCooldown = 0.8f; 
    public float dashSpeedBoost = 2f; 
    public float dashDuration = 0.2f; 

    private float defaultMoveSpeed; 
    private Animator animator;    
    private Rigidbody2D rb;       
    private SpriteRenderer spriteRenderer; 
    private Health health;        
    private Collider2D playerCollider; 
    private PlayerUI playerUI; 

    private bool isFacingRight = true; 
    private int jumpCount = 0;         
    private float jumpCooldownTimer = 0f; 
    private bool canJump = true;       
    private bool canDash = true;       
    private float dashCooldownTimer = 0f; 
    private float dashTimer = 0f; 
    private bool isInvincible = false;

    private PlayerSoundManager playerSoundManager;  

    private float attackCooldown = 0.5f; // Default attack cooldown
    private bool isInDashCooldown = false; // Whether we are in the dash cooldown period

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        playerCollider = GetComponent<Collider2D>();
        playerUI = FindAnyObjectByType<PlayerUI>();

        defaultMoveSpeed = moveSpeed; 

        health.onHealthChanged += UpdateHealthUI;
        health.onDeath += HandlePlayerDeath;

        rb.velocity = new Vector2(0, -1f); 

        playerSoundManager = GetComponent<PlayerSoundManager>();  

    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        animator.SetBool("isRunning", Mathf.Abs(move) > 0.1f);

        if (move > 0 && !isFacingRight) Flip();
        else if (move < 0 && isFacingRight) Flip();

        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                moveSpeed = defaultMoveSpeed;
                SetInvincibility(false); 
            }
        }

        if (isInDashCooldown)
        {
            attackCooldown = 1.5f; // 1.5 second cooldown after dash
        }
        else
        {
            attackCooldown = 0.5f; // Regular attack cooldown
        }
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        animator.SetFloat("moveDirection", Mathf.Abs(move));

        if (!canJump)
        {
            jumpCooldownTimer += Time.deltaTime;
            if (jumpCooldownTimer >= jumpCooldown)
            {
                canJump = true;
                jumpCount = 0;
                jumpCooldownTimer = 0f;
            }
        }

        if (Input.GetButtonDown("Jump") && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("isJumping", true);
            jumpCount++;
            
            playerSoundManager.PlayJumpSound();  

            if (jumpCount >= 1) canJump = false;
        }

        if (rb.velocity.y < 0) 
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }
        else if (rb.velocity.y == 0) 
        {
            animator.SetBool("isFalling", false);
            if (jumpCount == 0)
                animator.SetBool("isJumping", false);
        }

        if (!canDash)
        {
            dashCooldownTimer += Time.deltaTime;
            if (dashCooldownTimer >= dashCooldown)
            {
                canDash = true;
                dashCooldownTimer = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            Dash();
        }
    }

    private void Dash()
    {
        float dashDirection = Input.GetAxis("Horizontal") != 0 
            ? Mathf.Sign(Input.GetAxis("Horizontal")) 
            : (isFacingRight ? 1f : -1f);

        rb.velocity = new Vector2(dashDirection * dashDistance, rb.velocity.y);
        moveSpeed *= dashSpeedBoost;
        dashTimer = dashDuration;
        canDash = false;

        animator.SetTrigger("Dash");

        playerSoundManager.PlayDashSound();  

        SetInvincibility(true);

        isInDashCooldown = true;
        StartCoroutine(ResetDashCooldown());

    }

    private IEnumerator ResetDashCooldown()
    {
        yield return new WaitForSeconds(dashDuration);
        isInDashCooldown = false;
    }

    private void SetInvincibility(bool state)
    {
        isInvincible = state;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), state);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (playerUI != null)
        {
            playerUI.UpdateHealthBar(currentHealth, maxHealth);
        }
        Debug.Log($"Player Health: {currentHealth}/{maxHealth}");
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Player has died!");
    }

    public bool CanAttack() 
    {
        return attackCooldown == 0.5f; // Returns true if the regular attack cooldown is active
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible)
            health.TakeDamage(damage);
    }
}
