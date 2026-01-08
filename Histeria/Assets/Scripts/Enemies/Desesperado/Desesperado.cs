using UnityEngine;
using BehaviourAPI.Core;
using System.Collections;

public class Desesperado : EnemyBase
{
    [Header("Configuración Desesperado")]
    public float velocidadPersecucion = 3f;
    public GameObject desesperadoPequenoPrefab;

    [Header("Configuración de Daño")]
    public int damageAmount = 1;
    public float attackRadius = 1.5f;
    [Range(0, 1)]
    public float damagePoint = 0.5f;

    private GameObject player;
    private Rigidbody2D rb;
    private Vector2 direccionMovimiento;
    private bool yaSeDividio = false;
    private bool yaHizoDañoEnEsteCiclo = false;
    Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        InitializeStats(maxHealth, detectionRange, rb);
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (rb != null && !isDead)
        {
            rb.linearVelocity = direccionMovimiento * moveSpeed;
        }
    }

    // --- PERCEPCIONES ---
    public bool Killed() => currentHealth <= 0;
    public bool JugadorEnRango() => player != null && Vector2.Distance(transform.position, player.transform.position) < detectionRange;
    public bool JugadorEnRangoAtaque() => player != null && Vector2.Distance(transform.position, player.transform.position) < attackRange;

    // --- ACCIONES ---

    public StatusFlags AccionIdle()
    {
        direccionMovimiento = Vector2.zero;
        return !JugadorEnRango() ? StatusFlags.Running : StatusFlags.Failure;
    }

    public StatusFlags AccionPerseguir()
    {
        if (player == null) return StatusFlags.Failure;

        if (anim != null)
        {
            anim.SetTrigger("Mover");
        }


        if (JugadorEnRangoAtaque())
        {
            direccionMovimiento = Vector2.zero;
            return StatusFlags.Failure;
        }

        direccionMovimiento = (player.transform.position - transform.position).normalized;
        moveSpeed = velocidadPersecucion;
        return JugadorEnRango() ? StatusFlags.Running : StatusFlags.Failure;
    }

    public StatusFlags AccionAtaque()
    {
        direccionMovimiento = Vector2.zero;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("Atacar") && !anim.IsInTransition(0))
        {
            anim.SetTrigger("Atacar");
            yaHizoDañoEnEsteCiclo = false;
            return StatusFlags.Running;
        }

        if (stateInfo.IsName("Atacar"))
        {
            if (stateInfo.normalizedTime >= damagePoint && !yaHizoDañoEnEsteCiclo)
            {
                EjecutarDeteccionDeDaño();
                yaHizoDañoEnEsteCiclo = true;
            }

            if (stateInfo.normalizedTime < 1.0f) return StatusFlags.Running;
        }

        return StatusFlags.Success;
    }

    private void EjecutarDeteccionDeDaño()
    {
        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(transform.position, attackRadius);

        foreach (Collider2D hit in objectsHit)
        {
            if (hit.CompareTag("Player"))
            {
                var playerHealth = hit.GetComponentInParent<PlayerHealthHearts>();

                if (playerHealth != null)
                {
                    Debug.Log("<color=green>[ÉXITO]</color> Daño aplicado a Eli.");
                    playerHealth.TakeDamage(damageAmount);
                }
                break;
            }
        }
    }
    public void HacerDano()
    {
        float distancia = Vector2.Distance(transform.position, player.transform.position);
        if (distancia <= 1.5f)
        {
            PlayerHealthHearts ph = player.GetComponent<PlayerHealthHearts>();
            if (ph != null)
            {
                ph.TakeDamage();
            }
        }
    }
    /*
    public void AccionDivision()
    {
        Debug.Log("DIVIDIÉNDOSE");

        if (desesperadoPequenoPrefab == null)
        {
            Debug.LogError("Prefab NULL");
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            var go = Instantiate(
                desesperadoPequenoPrefab,
                transform.position,
                Quaternion.identity
            );

            Debug.Log("Instanciado: " + go.name);
        }

        yaSeDividio = true;
    }
    */
    
    public void AccionDivision()
    {
        if (yaSeDividio) return;

        if (desesperadoPequenoPrefab != null)
        {
            int numToSpawn = Random.Range(2,5); 

            for (int i = 0; i < numToSpawn; i++)
            {
                Vector3 spawnOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(-0.5f, 0.5f),
                    0
                );

                Instantiate(
                    desesperadoPequenoPrefab,
                    transform.position + spawnOffset,
                    Quaternion.identity
                );
            }
        }

        yaSeDividio = true;
    }
   

    protected override void Die()
    {
        if (!yaSeDividio)
        {
            AccionDivision();
        }

        isDead = true;
        StartCoroutine(DieDelayed());
    }
    private IEnumerator DieDelayed()
    {
        yield return null; // espera 1 frame
        base.Die();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}