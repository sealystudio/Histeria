using UnityEngine;

public enum BossPhase { Oleada, Histeria, PreAutoDestruccion, Autodestruccion }

public class BossController : MonoBehaviour
{
    [Header("DEBUG MODE")]
    public bool testHysteriaMode = false;

    [Header("Stats")]
    public float maxHP = 1000;
    public float currentHP;
    public float moveSpeed = 2.0f;

    [Header("Distancias")]
    public float meleeStopDistance = 2.5f;   // Distancia para pegar puñetazo (Fase 1)
    public float shootingStopDistance = 5.5f;// Distancia para disparar (Fase 2) - ¡MÁS LEJOS!
    public float detectionRange = 15f;

    [Header("Fases")]
    public BossPhase phase;

    [Header("Referencias")]
    public BossActions actions;
    public BossWorldInteraction world;
    public BossPerception perception;
    public Animator animator;

    private float autoDestructionTimer = 15f;
    private float timeWithoutDamage = 0f;
    private bool isDead = false;
    private bool isFacingRight = true;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        isFacingRight = transform.localScale.x > 0;

        if (testHysteriaMode)
        {
            currentHP = maxHP * 0.45f;
            phase = BossPhase.Histeria;
            Debug.LogWarning(" MODO DEBUG ACTIVADO: Boss empieza en HISTERIA");
        }
        else
        {
            currentHP = maxHP;
            phase = BossPhase.Oleada;
        }
    }

    void Update()
    {
        if (isDead) return;

        EvaluatePhase();
        RunPhaseBehaviour();
        UpdateAnimatorSpeed();
    }

    public void TakeDamage(float dmg)
    {
        if (phase == BossPhase.PreAutoDestruccion) return;
        currentHP -= dmg;
        timeWithoutDamage = 0f;
    }

    void EvaluatePhase()
    {
        float hpPercent = currentHP / maxHP * 100;

        if (testHysteriaMode && hpPercent > 50) return;

        if (hpPercent > 50) phase = BossPhase.Oleada;
        if (hpPercent <= 50 && hpPercent >= 10) phase = BossPhase.Histeria;
        if (hpPercent < 10) phase = BossPhase.PreAutoDestruccion;
    }

    void UpdateAnimatorSpeed()
    {
        if (animator == null) return;
        animator.speed = (phase == BossPhase.Histeria) ? 1.5f : 1f;
    }

    void RunPhaseBehaviour()
    {
        bool isWalking = false;

        switch (phase)
        {
            // --- FASE 1: ATAQUE CUERPO A CUERPO ---
            case BossPhase.Oleada:
                if (perception.player != null)
                {
                    float dist = Vector3.Distance(transform.position, perception.player.position);

                    // Se mueve si está lejos, pero para cerca (2.5m)
                    if (dist < detectionRange && dist > meleeStopDistance)
                    {
                        MoveTowardsPlayer();
                        isWalking = true;
                    }
                    // Si está cerca (2.5m) -> PUÑETAZO
                    else if (dist <= meleeStopDistance)
                    {
                        FacePlayer();
                        actions.BasicAttack(); //
                    }
                    actions.TrySpawnMinionEvery10Sec();
                }
                break;

            // --- FASE 2: ATAQUE A DISTANCIA ---
            case BossPhase.Histeria:
                if (perception.player != null)
                {
                    float dist = Vector3.Distance(transform.position, perception.player.position);

                    // Se mueve, pero para mucho antes (5.5m)
                    if (dist > shootingStopDistance)
                    {
                        MoveTowardsPlayer();
                        isWalking = true;
                    }
                    // Si llega a la distancia de disparo (5.5m) -> DISPARA PROYECTILES
                    else
                    {
                        FacePlayer();
                        actions.SpecialAttack(); // Dispara el cono, NO pega el puño
                        world.DropHazard();
                    }
                }
                break;

            case BossPhase.PreAutoDestruccion:
                HandleAutoDestructionCountdown();
                break;

            case BossPhase.Autodestruccion:
                if (!isDead)
                {
                    actions.Explode();
                    isDead = true;
                }
                break;
        }

        if (animator != null) animator.SetBool("IsWalking", isWalking);
    }

    void MoveTowardsPlayer()
    {
        FacePlayer();
        Vector3 targetPosition = new Vector3(perception.player.position.x, perception.player.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void FacePlayer()
    {
        if (perception.player == null) return;
        if (perception.player.position.x > transform.position.x && !isFacingRight) Flip();
        else if (perception.player.position.x < transform.position.x && isFacingRight) Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    void HandleAutoDestructionCountdown()
    {
        timeWithoutDamage += Time.deltaTime;
        if (timeWithoutDamage >= autoDestructionTimer) phase = BossPhase.Autodestruccion;
    }
}