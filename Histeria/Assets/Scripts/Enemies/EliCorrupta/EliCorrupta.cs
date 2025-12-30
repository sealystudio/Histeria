using UnityEngine;
using System.Collections;
using BehaviourAPI.Core;

public class EliCorrupta : EnemyBase
{
    [Header("Datos")]
    public EliCorruptaData data;
    private Transform eliNormal;   // referencia al jugador
    private Rigidbody2D rb;

    [Header("Ataque espejo")]
    private bool puedeDisparar;
    public float lagrimasCooldown = 0.5f;

    public bool PuedeDispararDebug() => puedeDisparar;
    private LevelManager lm;
    private bool alreadyCounted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
        InitializeStats(data.maxHealth , 7f, this.GetComponent<Rigidbody2D>());
        puedeDisparar = true;
        moveSpeed = data.moveSpeed;
        damage = data.damage;
        detectionRange = data.detectionRange;
        attackRange = data.attackRange;
        // Buscar dinámicamente al jugador en la escena
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            eliNormal = player.transform;
        }
    }

    void Update()
    {
        if (isDead || eliNormal == null) return;

        float distancia = DistanciaJugador();


        // --- Movimiento hacia Eli ---
      
        if (distancia < detectionRange && distancia > attackRange)
        {
            Vector2 dir = (eliNormal.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;

        }
      
    
        // 🔹 Girar en el eje X según la posición del jugador
        if (eliNormal.position.x > transform.position.x)
        {
            // Eli está a la derecha → mirar a la derecha
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (eliNormal.position.x < transform.position.x)
        {
            // Eli está a la izquierda → mirar a la izquierda
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // --- Método público para disparo espejo ---
    public void DispararEspejo(Vector3 direccionOriginal) // HAY QUE QUITAR EL ARGUMENTO
    {
        if (!puedeDisparar || data.lagrimaPrefab == null || eliNormal == null) return;

        //Vector3 direccionOriginal = (eliNormal.position - transform.position).normalized;

        // comprobar distancia al jugador
        float distancia = Vector2.Distance(transform.position, eliNormal.position);
        if (distancia > attackRange + 3) return; // demasiado lejos, no dispara

        Vector3 dirContraria = -direccionOriginal.normalized;

        GameObject tear = Instantiate(data.lagrimaPrefab, transform.position, Quaternion.identity);

        LagrimasAttack la = tear.GetComponent<LagrimasAttack>();
        if (la != null) la.Initialize(dirContraria, LagrimasAttack.Team.Corrupta);

        float angle = Mathf.Atan2(dirContraria.y, dirContraria.x) * Mathf.Rad2Deg;
        tear.transform.rotation = Quaternion.Euler(0, 0, angle) * data.lagrimaPrefab.transform.rotation;

        StartCoroutine(CooldownDisparo());
    }

    public void CargarAtaqueArea()
    {
        animator.SetTrigger("LoadingArea");
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(returnCharging());
    }

    public IEnumerator returnCharging()
    {
        yield return new WaitForSeconds(5f);
        animator.SetTrigger("finished");
    }

    private IEnumerator CooldownDisparo()
    {
        puedeDisparar = false;
        yield return new WaitForSeconds(lagrimasCooldown);
        puedeDisparar = true;
    }


    public void DamageArea()
    {
        if (eliNormal == null) return;

        float distancia = DistanciaJugador();
        if (distancia <= 4.5f)
        {
            PlayerHealthHearts ph = eliNormal.GetComponent<PlayerHealthHearts>();
            if (ph != null)
            {
                ph.TakeDamage();
            }
        }
    }

    protected override void OnHit()
    {
        if (animator != null)
            animator.SetTrigger("Hit");

        if (data != null && data.hitEffect != null)
            Instantiate(data.hitEffect, transform.position, Quaternion.identity);
    }


    // Para el sistema de utilidad , distancia respecto al jugador
    private float DistanciaJugador()
    {
        if (eliNormal == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, eliNormal.position);    
    }


    public void Idle()
    {
        rb.linearVelocity = Vector2.zero;
    }
    public int VidaActual()
    {
        return currentHealth;
    }   

    public StatusFlags MovimientoCorrupto()
    {
        float distancia = DistanciaJugador();


        // --- Movimiento hacia Eli ---
        if (distancia < detectionRange && distancia > attackRange)
        {
            Vector2 dir = (eliNormal.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
            return StatusFlags.Success;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            return StatusFlags.Failure;
        }
    }

    protected override void Die()
    {
        if (animator != null)
            animator.SetTrigger("Die");

        if (data != null && data.dieEffect != null)
            Instantiate(data.dieEffect, transform.position, Quaternion.identity);

        if (!alreadyCounted)
        {
            if (LevelManager.instance != null)
                LevelManager.instance.EnemyMuerto();

            if (DungeonPopulator.instance != null)
                DungeonPopulator.instance.RestarEnemigo();

            alreadyCounted = true;
        }

        base.Die();
    }

}
