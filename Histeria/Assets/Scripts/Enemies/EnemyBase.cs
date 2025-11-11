using BehaviourAPI.Core.Perceptions;
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

    [Header("Comportamiento general")]
    public bool isDead;
    public bool canMove;
    public bool canAttack;

    [Header("Animaciones")]
    public Animator animator;
    public Vector2 knockbackForce = new Vector2(2f, 2f);
    private Rigidbody2D rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Dynamic; // Correcto para recibir fuerzas
            rb.freezeRotation = true;
            rb.linearDamping = 5f;
        }
    }

    public virtual void InitializeStats(int health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
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
            rb.linearVelocity = Vector2.zero; //resetea velocidad previa
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
    }

    //el enemigo muere
    protected virtual void Die()
    {
        if (animator != null)
            animator.SetTrigger("Die");
        
        Destroy(gameObject, 0.5f); //Destruir el gameobject
    }

    // Este método se llama automáticamente cuando este collider toca a otro
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Comprobamos si el objeto con el que chocamos tiene el tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. Si es el jugador, intentamos obtener su script de salud
            //    (Asumo que tu jugador tiene un script como "PlayerHealthHearts")
            PlayerHealthHearts playerHealth = collision.gameObject.GetComponent<PlayerHealthHearts>();

            if (playerHealth != null)
            {
                // 3. Si encontramos el script, le decimos que reciba daño
                //    (Asegúrate de que tu script de jugador tenga un método "TakeDamage")
                playerHealth.TakeDamage(damage);
            }
        }
    }

}
