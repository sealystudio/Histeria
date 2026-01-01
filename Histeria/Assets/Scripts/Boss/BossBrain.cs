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

    private float nextHazardTime = 0f;
    private float nextSpecialAttackTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (context == null) context = GetComponent<BossContext>();
        if (actions == null) actions = GetComponent<BossActions>();

        Debug.Log("BOSS BRAIN: Iniciado. ¿Contexto encontrado? " + (context != null));
    }

    private void Update()
    {
        // 1. GESTIÓN FÍSICA DEL MOVIMIENTO
        if (rb != null)
        {
            rb.linearVelocity = moveDirection.normalized * moveSpeed;
        }
        else
        {
            Debug.LogError("BOSS BRAIN: ¡Falta el Rigidbody2D!");
        }

        // 2. GESTIÓN DE ANIMACIÓN (¡LO NUEVO!)
        if (context.animator != null)
        {
            // Si la velocidad física es mayor que 0.1, es que se está moviendo
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            context.animator.SetBool("IsWalking", isMoving);
        }

        // 3. ACTUALIZAR DIRECCIÓN DE MIRADA (FLIP)
        if (context.playerTransform != null)
        {
            // Solo giramos si está persiguiendo o lejos, para evitar giros locos cuerpo a cuerpo
            if (context.DistanceToPlayer > 0.5f)
            {
                if (context.playerTransform.position.x > transform.position.x)
                    transform.localScale = new Vector3(1, 1, 1);
                else
                    transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    // =================================================================================
    //                CONDICIONES (CON LOGS)
    // =================================================================================

    public bool IsPhaseOleada()
    {
        return context.HealthPercentage > 0.5f;
    }

    public bool IsPhaseHisteria()
    {
        return context.HealthPercentage <= 0.5f && context.HealthPercentage > 0.10f;
    }

    public bool IsPhaseSurvival()
    {
        return context.HealthPercentage <= 0.10f;
    }

    public bool IsSurvivalTimeOver()
    {
        return context.SurvivalTime >= 15f;
    }

    public bool IsPlayerCloseMelee()
    {
        float dist = context.DistanceToPlayer;
        bool isClose = dist <= 4.5f;

        return isClose;
    }

    // =================================================================================
    //                  ACCIONES (CON LOGS)
    // =================================================================================

    public StatusFlags Action_ChasePlayer()
    {
        if (context.playerTransform == null)
        {
            Debug.LogError("BOSS ACCION: ¡No encuentro al PlayerTransform!");
            return StatusFlags.Failure;
        }

        // Calculamos dirección
        moveDirection = (context.playerTransform.position - transform.position).normalized;
        moveSpeed = 2.5f;

        Debug.Log($"BOSS ACCION: Persecución activa. Dirección: {moveDirection}");

        // Si llega, paramos
        if (context.DistanceToPlayer <= 2.5f)
        {
            moveDirection = Vector2.zero;
            Debug.Log("BOSS ACCION: He llegado al jugador. Paro.");
            return StatusFlags.Success;
        }

        return StatusFlags.Running;
    }

    private float lastAttackTime = 0f; // Variable para recordar cuándo pegó


    public StatusFlags Action_DoMeleeAttack()
    {
        moveDirection = Vector2.zero;

        // 1. CHEQUEO DE COOLDOWN (DESCANSO)
        if (Time.time < lastAttackTime + 2.0f) // 2.0f es el tiempo de espera
        {
            return StatusFlags.Failure; // "Aún estoy cansado, no pego"
        }

        // 2. EJECUCIÓN
        if (!actions.IsBusy)
        {
            Debug.Log("BOSS ACCION: Lanzando golpe melee...");
            actions.ExecuteBasicAttack();
            lastAttackTime = Time.time; // ¡Guardamos la hora del golpe!
            return StatusFlags.Running;
        }

        // 3. FINALIZACIÓN
        // Si IsBusy es falso AQUÍ, significa que ya ha terminado la animación
        // (porque el primer if ya habría saltado si estuviera libre para empezar)
        // NOTA: Con la lógica actual, este bloque final necesita cuidado, 
        // pero con el cooldown arriba, funcionará bien.

        return StatusFlags.Success;
    }

    // ... Resto de acciones (SpecialAttack, SpawnMinion, DropHazard) igual ...
    // Añádeles logs si los necesitas, pero céntrate en Chase y Melee primero.

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
            // Calculamos dirección hacia el jugador
            moveDirection = (context.playerTransform.position - transform.position).normalized;

            moveSpeed = 1.5f; 
        }


        if (Time.time >= nextSpecialAttackTime && !actions.IsBusy)
        {
            actions.ExecuteSpecialAttack();

            Debug.Log("BOSS: Disparo especial en movimiento");

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

            Debug.Log("BOSS: ¡Dejando rastro de veneno!");

            nextHazardTime = Time.time + 4.0f;
        }
        return StatusFlags.Running;
    }

    public float GetDistanceToPlayer()
    {
        return context.DistanceToPlayer;
    }

}