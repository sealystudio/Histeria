using UnityEngine;
using BehaviourAPI.Core;
using System.Collections;

public class MirrorBossBrain : EnemyBase
{
    [Header("Referencias Visuales")]
    public Animator animatorVisual;

    [Header("Referencias Fase 1")]
    public GameObject projectilePrefab;
    public GameObject shockwavePrefab;

    [Header("Referencias Fase 2")]
    public GameObject projectilePhase2Prefab;

    private Transform playerTransform;
    private Vector3 originalScale;

    [Header("Configuración de Ataque")]
    public float projectileSpeed = 10f;

    [Header("Configuración Fase 1 (Espejo)")]
    public float detectionRange = 10f;
    public int hitsForSpecialP1 = 3;
    private int hitCounterP1 = 0;

    [Header("Configuración Fase 2 (Ráfagas)")]
    public int hitsForSpecialP2 = 2;
    public float burstCooldown = 1.5f;
    public float chaseSpeed = 3.5f;
    private int hitCounterP2 = 0;
    private float nextBurstTime = 0f;

    [Header("Configuración Fase 3 (Destrucción)")]
    public float destructionTime = 5.0f;
    private float currentDestructTimer = 0f;

    private bool executingSpecial = false;

    // =========================================================================
    //                            INICIALIZACIÓN
    // =========================================================================

    private void Awake()
    {
        if (maxHealth <= 0) maxHealth = 100;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) { rb.gravityScale = 0f; rb.constraints = RigidbodyConstraints2D.FreezeRotation; }
    }

    private void Start()
    {
        originalScale = transform.localScale;
        if (animatorVisual != null) animator = animatorVisual;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) playerTransform = player.transform;

        InitializeStats(maxHealth, 10f, rb);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (playerTransform != null && !isDead)
        {
            // Si el jugador está a la derecha (x mayor)
            if (playerTransform.position.x > transform.position.x)
            {
                // Ponemos el signo MENOS delante del Abs para que mire a la IZQUIERDA
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            // Si el jugador está a la izquierda (x menor)
            else if (playerTransform.position.x < transform.position.x)
            {
                // Quitamos el signo menos para que mire a la DERECHA
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
        }
    }

    // =========================================================================
    //                            SISTEMA DE DAÑO (RECIBIDO)
    // =========================================================================

    public override void TakeDamage(int dmg, Vector2 hitDirection)
    {
        if (IsPhase3()) return;

        base.TakeDamage(dmg, hitDirection);

        // Debug para controlar la vida y transiciones
        Debug.Log($"BOSS: Vida {currentHealth}/{maxHealth}");

        if (IsPhase1()) hitCounterP1++;
        else if (IsPhase2()) hitCounterP2++;
    }

    // =========================================================================
    //               ¡NUEVO! SISTEMA DE DAÑO POR CONTACTO (REALIZADO)
    // =========================================================================

    // Esto se activa automáticamente cuando el Boss toca al Jugador físicamente
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Buscamos el script de vida del jugador (en el objeto o padre)
            PlayerHealthHearts playerHealth = collision.gameObject.GetComponentInParent<PlayerHealthHearts>();

            if (playerHealth != null)
            {
                // Le quitamos 1 de vida por tocar al Boss
                playerHealth.TakeDamage(1);
                Debug.Log("BOSS: ¡Te he tocado! Daño aplicado.");
            }
        }
    }

    // =========================================================================
    //                      ACCIONES (LÓGICA FASES)
    // =========================================================================

    // --- FASE 1 ---
    public StatusFlags Action_TryReplicate()
    {
        if (executingSpecial) return StatusFlags.Running;
        if (playerTransform == null) return StatusFlags.Failure;

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist > detectionRange) return StatusFlags.Failure;

        if (hitCounterP1 >= hitsForSpecialP1)
        {
            StartCoroutine(Routine_Shockwave());
            hitCounterP1 = 0;
            return StatusFlags.Running;
        }

        if (dist < 4.5f) StartCoroutine(Routine_MeleeAttack());
        else StartCoroutine(Routine_RangedAttack());

        return StatusFlags.Running;
    }

    public StatusFlags Action_Idle()
    {
        if (playerTransform != null)
        {
            if (Vector2.Distance(transform.position, playerTransform.position) <= detectionRange)
                return StatusFlags.Failure;
        }
        rb.linearVelocity = Vector2.zero;
        return StatusFlags.Running;
    }

    // --- FASE 2 ---

    private bool TengoTrabajoFase2()
    {
        return (Time.time >= nextBurstTime) || (hitCounterP2 >= hitsForSpecialP2);
    }

    public StatusFlags Action_CombatPhase2()
    {
        if (executingSpecial) return StatusFlags.Running;

        // Si NO toca atacar, fallamos -> Pasa a FollowPlayer
        if (!TengoTrabajoFase2()) return StatusFlags.Failure;

        if (hitCounterP2 >= hitsForSpecialP2)
        {
            StartCoroutine(Routine_Shockwave());
            hitCounterP2 = 0;
        }
        else
        {
            StartCoroutine(Routine_BurstShot());
            nextBurstTime = Time.time + burstCooldown;
        }
        return StatusFlags.Running;
    }

    public StatusFlags Action_FollowPlayer()
    {
        // VERSIÓN SEGURA: Siempre devuelve Running si no hay error grave.
        // Esto mantiene el árbol vivo para que chequee transiciones a Fase 3.

        if (playerTransform == null) return StatusFlags.Failure;

        Vector2 dir = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = dir * chaseSpeed;

        return StatusFlags.Running;
    }

    // --- FASE 3 ---
    public StatusFlags Action_SelfDestructSequence()
    {
        rb.linearVelocity = Vector2.zero;
        currentDestructTimer += Time.deltaTime;

        if (animator) animator.SetTrigger("HitInvulnerable");

        if (currentDestructTimer >= destructionTime)
        {
            Die();
            return StatusFlags.Success;
        }
        return StatusFlags.Running;
    }

    // =========================================================================
    //                   CORUTINAS DE ATAQUE
    // =========================================================================

    IEnumerator Routine_MeleeAttack()
    {
        executingSpecial = true;
        rb.linearVelocity = Vector2.zero;
        if (animator) animator.SetTrigger("AttackMelee");
        yield return new WaitForSeconds(0.5f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2.0f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) hit.GetComponentInParent<PlayerHealthHearts>()?.TakeDamage(1);
        }
        yield return new WaitForSeconds(1.0f);
        executingSpecial = false;
    }

    IEnumerator Routine_RangedAttack()
    {
        executingSpecial = true;
        rb.linearVelocity = Vector2.zero;
        if (animator) animator.SetTrigger("AttackRange");
        if (projectilePrefab) SpawnProjectile(projectilePrefab);
        yield return new WaitForSeconds(1.5f);
        executingSpecial = false;
    }

    IEnumerator Routine_Shockwave()
    {
        executingSpecial = true;
        rb.linearVelocity = Vector2.zero;
        if (animator) animator.SetTrigger("AreaAttack");
        yield return new WaitForSeconds(0.5f);
        if (shockwavePrefab) SpawnProjectile(shockwavePrefab);
        yield return new WaitForSeconds(1.0f);
        executingSpecial = false;
    }

    IEnumerator Routine_BurstShot()
    {
        executingSpecial = true;
        rb.linearVelocity = Vector2.zero;
        GameObject bulletToUse = projectilePhase2Prefab != null ? projectilePhase2Prefab : projectilePrefab;
        for (int i = 0; i < 3; i++)
        {
            if (bulletToUse) SpawnProjectile(bulletToUse);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.5f);
        executingSpecial = false;
    }

    void SpawnProjectile(GameObject prefab)
    {
        if (playerTransform == null || prefab == null) return;
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);
        Vector2 dir = (playerTransform.position - transform.position).normalized;
        Rigidbody2D rbProj = obj.GetComponent<Rigidbody2D>();
        if (rbProj != null) rbProj.linearVelocity = dir * projectileSpeed;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Collider2D myCol = GetComponent<Collider2D>();
        Collider2D objCol = obj.GetComponent<Collider2D>();
        if (myCol && objCol) Physics2D.IgnoreCollision(myCol, objCol);

        LagrimasAttack attackScript = obj.GetComponent<LagrimasAttack>();
        if (attackScript == null) try { attackScript = obj.AddComponent<LagrimasAttack>(); } catch { }
        if (attackScript != null) attackScript.Initialize(dir, LagrimasAttack.Team.Corrupta);
    }

    // =========================================================================
    //                      CONDICIONES DE TRANSICIÓN
    // =========================================================================

    public bool CheckTransition_ToPhase2() => GetHealthPct() <= 0.5f && GetHealthPct() > 0.25f;
    public bool CheckTransition_ToPhase3() => GetHealthPct() <= 0.25f;

    private float GetHealthPct()
    {
        if (maxHealth <= 0) return 1;
        return (float)currentHealth / (float)maxHealth;
    }

    private bool IsPhase1() => GetHealthPct() > 0.5f;
    private bool IsPhase2() => GetHealthPct() <= 0.5f && GetHealthPct() > 0.25f;
    private bool IsPhase3() => GetHealthPct() <= 0.25f;
}