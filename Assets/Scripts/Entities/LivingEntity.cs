using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public int maxHealth = 100;
    protected int currentHealth;
    protected bool isDead = false; // Pojistka proti zacyklení animace

    // Awake se stará o okamžité nastavení životù pro Health Bar
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    // Tuto prázdnou metodu zde MUSÍŠ nechat, aby ji EnemyAI a PlayerController mohly pøepsat (override)
    protected virtual void Start()
    {
        // Mùže zùstat prázdná
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Death");

        // Vypnutí kolizí, aby mrtvola nepøekážela
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;

        // Pokud to není hráè, smažeme ho za chvíli
        if (!gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, 1.5f);
        }
    }
}