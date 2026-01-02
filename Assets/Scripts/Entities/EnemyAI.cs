using UnityEngine;

public class EnemyAI : LivingEntity
{
    [Header("Enemy Stats")]
    public float speed = 3f;
    public int damage = 10;
    public float checkRadius = 50f; // Range to start chasing

    private Transform playerTransform;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        
        // Find player slightly inefficiently but okay for small prototype
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // Simple chase logic
        if (distance < checkRadius)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
            
            // Jump logic
            float verticalDist = playerTransform.position.y - transform.position.y;
            bool shouldJump = (verticalDist > 1.0f && Mathf.Abs(distance) < 3.0f) || (rb.linearVelocity.x == 0 && Mathf.Abs(direction.x) > 0.1f);
            
            if (shouldJump && Mathf.Abs(rb.linearVelocity.y) < 0.1f) // Ground check via velocity
            {
                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, 7f); // Jump force
            }

            // Flip sprite
            if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LivingEntity target = collision.gameObject.GetComponent<LivingEntity>();
            if (target != null)
            {
                target.TakeDamage(damage);
                // Optional: Knockback player or self?
            }
        }
    }

    protected override void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterKill();
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Death"); // Toto "zatahne" za trigger v Animatoru
        }

        GetComponent<Collider2D>().enabled = false; // Vypne kolize, aby hrac mohl mrtvolou projet
        Destroy(gameObject, 0.5f); // Smaze slajma po dohrani animace
    }
}
