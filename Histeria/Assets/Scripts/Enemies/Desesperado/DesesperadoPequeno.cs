/*using UnityEngine;
using BehaviourAPI.Core;
using System.Collections;

public class DesesperadoPequeno : EnemyBase
{
    private GameObject player;
    private Rigidbody2D rb;
    private Vector2 direccionMovimiento;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        InitializeStats(maxHealth, detectionRange, rb);
    }

    private void Update()
    {
        if (rb != null && !isDead)
        {
            rb.linearVelocity = direccionMovimiento * moveSpeed;
        }
    }

    // --- PERCEPCIONES (PULL) ---

    public bool Killed()
    {
        return currentHealth <= 0 || isDead;
    }

    public bool JugadorEnRango()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.transform.position) < detectionRange;
    }

    // --- ACCIONES (STATUS FLAGS) ---

    public StatusFlags AccionHuir()
    {
        if (player == null) return StatusFlags.Failure;

        // Dirección opuesta al jugador
        direccionMovimiento = (transform.position - player.transform.position).normalized;

        return StatusFlags.Running;
    }
}*/