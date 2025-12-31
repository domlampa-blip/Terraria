using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint; // Pøetáhni sem objekt "attack" z hierarchie
    private Animator anim;

    [Header("Settings")]
    public float attackRange = 0.8f;
    public int attackDamage = 20;
    public float attackRate = 2f;
    public LayerMask enemyLayer; // Nastav na "Enemy"

    private float nextAttackTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            // Útok levým tlaèítkem myši
            if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            {
                PerformAttack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void PerformAttack()
    {
        // Spustí vizuální švih v Animátoru
        if (anim != null) anim.SetTrigger("Attack");

        // Detekce nepøátel v kruhu kolem attackPointu
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Každý nepøítel musí mít skript dìdící z LivingEntity
            LivingEntity entity = enemy.GetComponent<LivingEntity>();
            if (entity != null)
            {
                entity.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}