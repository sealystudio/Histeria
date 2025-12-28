using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats base")]
    public int currentHealth;
    public int maxHealth;
    public float moveSpeed;

    [Header("Stats de ataque")]
    public float detectionRange;
    public float attackRange;
    public int damage;

    [Header("Daño por contacto")]
    public float contactCooldown = 1f;  // 1 segundo de cooldown entre daños
    private float lastContactTime = -10f;

    [Header("Comportamiento general")]
    public bool isDead;
    public bool canMove;
    public bool canAttack;

    [Header("Animaciones y efectos")]
    public Animator animator;
    public Vector2 knockbackForce = new Vector2(2f, 2f);
    public float flashDuration = 0.15f;  // duración del parpadeo
    private SpriteRenderer spriteRenderer;
    
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();


        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Dynamic; // Correcto para recibir fuerzas
            rb.freezeRotation = true;
            rb.linearDamping = 5f;
        }
    }

    public virtual void InitializeStats(int health , float dT , Rigidbody2D rigid)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        detectionRange = dT;
        rb = rigid;
    }

    //que ocurre cuando recibe daño (pierde vida)
    public virtual void TakeDamage(int dmg, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHealth -= dmg;
        OnHit(); //animación, efectos, sonidos

        //empuje hacia atras cuando reciben daño
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(hitDirection.x * knockbackForce.x, hitDirection.y * knockbackForce.y), ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //que se ve al recibir un golpe, momento exacto del impacto, pero puede que no le haga daño por eso se separa de takedamage
    //puede servir para meter efectos de sonido, partículas, animaciones...
    protected virtual void OnHit()
    {
        if (animator != null)
            animator.SetTrigger("Hit");

       
        // parpadeo visual
        if (spriteRenderer != null)
            StartCoroutine(FlashCoroutine());
    }

    //el enemigo muere
    protected virtual void Die()
    {
        if (animator != null)
            animator.SetTrigger("Die");


        Destroy(gameObject, 0.5f); //Destruir el gameobject
    }

    private IEnumerator FlashCoroutine()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.enabled = true;
    }

    // Este método se llama automáticamente cuando este collider toca a otro
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastContactTime < contactCooldown) return;

            PlayerHealthHearts playerHealth = other.GetComponent<PlayerHealthHearts>();
            if (playerHealth != null)
            {
                // Daño
                playerHealth.TakeDamage(damage);

                // Knockback más fuerte hacia atrás
                if (rb != null)
                {
                    Vector2 knockDir = (other.transform.position - transform.position).normalized;
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(-knockDir * knockbackForce.magnitude * 1.5f, ForceMode2D.Impulse); // más fuerte
                }

                lastContactTime = Time.time;
            }
        }
    }

}
