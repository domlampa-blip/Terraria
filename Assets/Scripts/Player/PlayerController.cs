using System.Collections;
using UnityEngine;

public class PlayerController : LivingEntity
{
    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1.5f;
    public float flickerInterval = 0.1f;
    private bool isInvincible = false;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Physics")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private float moveInput;

    // JEDNA METODA START - kombinuje vsechnu inicializaci
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            if (anim != null) anim.SetFloat("Speed", 0);
            return;
        }

        ProcessInput();
    }

    void FixedUpdate()
    {
        Move();
        CheckGround();
        ApplyBetterJumpPhysics();

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
            anim.SetBool("IsGrounded", isGrounded);
        }
    }

    void ProcessInput()
    {
        float x = 0;
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1;

            if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
            {
                Jump();
            }
        }
        moveInput = x;
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void CheckGround()
    {
        if (groundCheck == null) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // JEDNA METODA TAKEDAMAGE - kombinuje logiku blikani i odrazu
    public override void TakeDamage(int damage)
    {
        if (isInvincible) return;

        base.TakeDamage(damage);

        if (currentHealth > 0)
        {
            if (anim != null) anim.SetTrigger("Hurt");

            // Odraz od nepritele
            rb.AddForce(new Vector2(-transform.localScale.x * 3f, 3f), ForceMode2D.Impulse);

            StartCoroutine(HandleInvincibility());
        }
    }

    private IEnumerator HandleInvincibility()
    {
        isInvincible = true;
        float timer = 0;

        while (timer < invincibilityDuration)
        {
            Color c = spriteRenderer.color;
            c.a = (c.a == 1f) ? 0.3f : 1f;
            spriteRenderer.color = c;

            yield return new WaitForSeconds(flickerInterval);
            timer += flickerInterval;
        }

        Color finalColor = spriteRenderer.color;
        finalColor.a = 1f;
        spriteRenderer.color = finalColor;
        isInvincible = false;
    }

    void ApplyBetterJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    protected override void Die()
    {
        // Spusti zakladni logiku (animaci) z LivingEntity
        base.Die();

        // Zakaze skript pro pohyb a utok
        this.enabled = false;
        if (GetComponent<PlayerAttack>() != null)
            GetComponent<PlayerAttack>().enabled = false;

        // Zastavi veskery zbyvajici pohyb fyziky
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; // Rytir zustane lezet na miste
        }

        Debug.Log("You are dead");
        
        // Vyvolat Game Over
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }
    }
}
