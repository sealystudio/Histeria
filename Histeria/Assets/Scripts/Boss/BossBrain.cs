using System.Collections.Generic;
using BehaviourAPI.Core;
using UnityEngine;

public class BossBrain : MonoBehaviour
{
    [Header("Referencias Internas")]
    public BossContext context;
    public BossActions actions;

    [Header("Configuración Movimiento")]
    public float moveSpeed = 2.0f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    [Header("Fase Supervivencia")]
    private float survivalTimer = 0f;
    private bool isDead = false;

    private float nextHazardTime = 0f;
    private float nextSpecialAttackTime = 0f;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (context == null) context = GetComponent<BossContext>();
        if (actions == null) actions = GetComponent<BossActions>();
    }

    private void Update()
    {
        if (isDead) return;

        // 1. GESTIÓN FÍSICA
        if (rb != null)
        {
            rb.linearVelocity = moveDirection.normalized * moveSpeed;
        }

        // 2. GESTIÓN DE ANIMACIÓN
        if (context.animator != null)
        {
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            context.animator.SetBool("IsWalking", isMoving);
        }

        // 3. ACTUALIZAR DIRECCIÓN DE MIRADA
        if (context.playerTransform != null && context.DistanceToPlayer > 0.5f)
        {
            if (context.playerTransform.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // =================================================================================
    //                                  SISTEMA DE DAÑO
    // =================================================================================

    /// <summary>
    /// Llama a este método desde el script donde el Boss recibe daño.
    /// </summary>
    public void OnBossHit()
    {
        if (IsPhaseSurvival())
        {
            Debug.Log("<color=red>BOSS: ¡Golpe recibido! Reiniciando contador de 15s.</color>");
            survivalTimer = 0f; // Reinicia el tiempo si el jugador le pega
        }
    }

    // =================================================================================
    //                                ACCIÓN FASE FINAL
    // =================================================================================

    public StatusFlags Action_SurvivalMode()
    {
        if (isDead) return StatusFlags.Success;

        // 1. COMPORTAMIENTO AGRESIVO: Sigue atacando como en Histeria
        if (IsPlayerCloseMelee())
        {
            Action_DoMeleeAttack();
        }
        else
        {
            Action_ChasePlayer();
        }

        // 2. CONTADOR DE SUPERVIVENCIA: Solo muere si el jugador NO le pega
        survivalTimer += Time.deltaTime;

        // Debug visual del tiempo restante
        if (Mathf.FloorToInt(survivalTimer) % 5 == 0 && survivalTimer > 1f)
            Debug.Log($"BOSS: Tiempo en paz: {survivalTimer:F1}/15s");

        if (survivalTimer >= 15f)
        {
            Die();
            return StatusFlags.Success;
        }

        return StatusFlags.Running;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        moveDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("<color=green>BOSS: El jugador ha superado la prueba de paz. Muerte final.</color>");

        // Disparar aquí tu animación de muerte o evento
        // actions.ExecuteDeath(); 
        Destroy(gameObject, 1.0f);
    }

    // =================================================================================
    //                                  CONDICIONES
    // =================================================================================

    public bool IsPhaseOleada() => context.HealthPercentage > 0.5f;

    public bool IsPhaseHisteria() => context.HealthPercentage <= 0.5f && context.HealthPercentage > 0.10f;

    public bool IsPhaseSurvival() => context.HealthPercentage <= 0.10f && !isDead;

    public bool IsPlayerCloseMelee() => context.DistanceToPlayer <= 4.5f;

    // =================================================================================
    //                                   ACCIONES
    // =================================================================================

    public StatusFlags Action_ChasePlayer()
    {
        if (context.playerTransform == null) return StatusFlags.Failure;

        moveDirection = (context.playerTransform.position - transform.position).normalized;
        moveSpeed = 2.5f;

        if (context.DistanceToPlayer <= 2.5f)
        {
            moveDirection = Vector2.zero;
            return StatusFlags.Success;
        }
        return StatusFlags.Running;
    }

    public StatusFlags Action_DoMeleeAttack()
    {
        moveDirection = Vector2.zero;
        if (Time.time < lastAttackTime + 2.0f) return StatusFlags.Failure;

        if (!actions.IsBusy)
        {
            actions.ExecuteBasicAttack();
            lastAttackTime = Time.time;
            return StatusFlags.Running;
        }
        return StatusFlags.Success;
    }

    public StatusFlags Action_SpawnMinion()
    {
        moveDirection = Vector2.zero;
        if (!actions.IsBusy) actions.ExecuteSpawnMinion();
        return actions.IsBusy ? StatusFlags.Running : StatusFlags.Success;
    }

    public StatusFlags Action_DoSpecialAttack()
    {
        if (context.playerTransform != null)
        {
            moveDirection = (context.playerTransform.position - transform.position).normalized;
            moveSpeed = 1.5f;
        }

        if (Time.time >= nextSpecialAttackTime && !actions.IsBusy)
        {
            actions.ExecuteSpecialAttack();
            nextSpecialAttackTime = Time.time + 2.5f;
        }
        return StatusFlags.Running;
    }

    public StatusFlags Action_DropHazard()
    {
        if (context.playerTransform != null)
        {
            moveDirection = (context.playerTransform.position - transform.position).normalized;
            moveSpeed = 2.5f;
        }

        if (Time.time >= nextHazardTime)
        {
            context.worldInteraction.DropHazard();
            nextHazardTime = Time.time + 4.0f;
        }
        return StatusFlags.Running;
    }
}