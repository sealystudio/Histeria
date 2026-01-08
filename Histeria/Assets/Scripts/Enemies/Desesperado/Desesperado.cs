using UnityEngine;
using BehaviourAPI.Core;
using System.Collections;

public class Desesperado : EnemyBase
{
    [Header("Configuración Desesperado")]
    public float velocidadPersecucion = 3f;
    public GameObject desesperadoPequenoPrefab;

    private GameObject player;
    private Rigidbody2D rb;
    private Vector2 direccionMovimiento;
    private bool yaSeDividio = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        // Inicializamos las stats directamente con las variables del Inspector de EnemyBase
        InitializeStats(maxHealth, detectionRange, rb);
    }

    private void Update()
    {
        if (rb != null && !isDead)
        {
            // Movimiento plano físico
            rb.linearVelocity = direccionMovimiento * moveSpeed;
        }
    }

    // --- PERCEPCIONES (PULL) ---

    public bool Killed()
    {
        return currentHealth <= 0 ;
    }

    public bool JugadorEnRango()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.transform.position) < detectionRange;
    }

    public bool JugadorEnRangoAtaque()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.transform.position) < attackRange;
    }

    // --- ACCIONES (STATUS FLAGS) ---

    public void AccionIdle()
    {
        direccionMovimiento = Vector2.zero;
        
    }

    public StatusFlags AccionPerseguir()
    {
        if (animator != null)
            animator.SetTrigger("Mover");

        if (player == null) return StatusFlags.Failure;

        direccionMovimiento = (player.transform.position - transform.position).normalized;
        moveSpeed = velocidadPersecucion;

        if(JugadorEnRangoAtaque()) return StatusFlags.Failure;

        return JugadorEnRango() ? StatusFlags.Running : StatusFlags.Failure;
    }

    public void AccionAtaque()
    {
        direccionMovimiento = Vector2.zero;
        
    }

    public void AccionDivision()
    {
        if (yaSeDividio) return;

        if (desesperadoPequenoPrefab != null)
        {
            for (int i = 0; i < 2; i++) // Se divide en 2 según el doc
            {
                Vector3 spawnOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                Instantiate(desesperadoPequenoPrefab, transform.position + spawnOffset, Quaternion.identity);
            }
        }
        
        yaSeDividio = true;
        Die();
        }

    protected override void Die()
    {
        direccionMovimiento = Vector2.zero;
        isDead = true;
        base.Die();
    }
}