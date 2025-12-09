using UnityEngine;
using System.Collections;

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
    public float meleeStopDistance = 2.5f;
    public float shootingStopDistance = 5.5f;
    public float detectionRange = 15f;

    [Header("Fases")]
    public BossPhase phase;

    [Header("Referencias")]
    public BossActions actions;
    public BossWorldInteraction world;
    public BossPerception perception;
    public Animator animator;
    public BossMusicManager musicManager;

    [Header("Configuración Final")]
    public float secondsToSurvive = 15f; // Tiempo que el jugador debe aguantar sin disparar

    private float timeWithoutDamage = 0f;
    private bool isDead = false;
    private bool isFacingRight = true;

    // Para controlar que la secuencia de muerte solo empiece una vez
    private bool deathSequenceStarted = false;

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

        // Solo evaluamos cambio de fase si NO estamos ya en la fase final de autodestrucción
        if (phase != BossPhase.Autodestruccion)
        {
            EvaluatePhase();
        }

        RunPhaseBehaviour();
        UpdateAnimatorSpeed();
    }

    public void TakeDamage(float dmg)
    {
        // --- AQUÍ ESTÁ LA LÓGICA DE LA FASE FINAL ---
        if (phase == BossPhase.PreAutoDestruccion)
        {
            // 1. NO bajamos vida (es invencible)

            // 2. CASTIGO: Si el jugador le pega, reiniciamos el contador a 0
            timeWithoutDamage = 0f;

            Debug.Log("¡NO DISPARES! Has reiniciado el contador de autodestrucción.");
            return;
        }

        // Si ya está muriendo, ignoramos todo
        if (phase == BossPhase.Autodestruccion) return;

        // Daño normal en el resto de fases
        currentHP -= dmg;
    }

    void EvaluatePhase()
    {
        // Si ya estamos en Pre-Autodestrucción, no volvemos atrás
        if (phase == BossPhase.PreAutoDestruccion) return;

        float hpPercent = currentHP / maxHP * 100;

        if (testHysteriaMode && hpPercent > 50) return;

        if (hpPercent > 50)
            phase = BossPhase.Oleada;
        else if (hpPercent <= 50 && hpPercent >= 10)
            phase = BossPhase.Histeria;
        else if (hpPercent < 10)
            phase = BossPhase.PreAutoDestruccion;
    }

    void UpdateAnimatorSpeed()
    {
        if (animator == null) return;

        // En Pre-Autodestrucción puede vibrar o ir rápido, tú decides. 
        // Aquí lo dejo normal o rápido según gustes.
        animator.speed = (phase == BossPhase.Histeria || phase == BossPhase.PreAutoDestruccion) ? 1.5f : 1f;
    }

    void RunPhaseBehaviour()
    {
        bool isWalking = false;

        switch (phase)
        {
            case BossPhase.Oleada:
                RunOleadaLogic(ref isWalking);
                break;

            case BossPhase.Histeria:
                RunHisteriaLogic(ref isWalking);
                break;

            case BossPhase.PreAutoDestruccion:
                // En esta fase, el Boss sigue atacando (para poner nervioso al jugador)
                // Pero es invencible y cuenta el tiempo.
                RunHisteriaLogic(ref isWalking);
                HandleAutoDestructionCountdown();
                break;

            case BossPhase.Autodestruccion:
                // Aquí ya no hace nada en el Update, la Corutina 'DieSequence' tiene el control
                isWalking = false; // Aseguramos que se quede quieto
                break;
        }

        if (animator != null) animator.SetBool("IsWalking", isWalking);
    }

    // He separado las lógicas para tener el código más limpio
    void RunOleadaLogic(ref bool isWalking)
    {
        if (perception.player != null)
        {
            float dist = Vector3.Distance(transform.position, perception.player.position);
            if (dist < detectionRange && dist > meleeStopDistance)
            {
                MoveTowardsPlayer();
                isWalking = true;
            }
            else if (dist <= meleeStopDistance)
            {
                FacePlayer();
                actions.BasicAttack();
            }
            actions.TrySpawnMinionEvery10Sec();
        }
    }

    void RunHisteriaLogic(ref bool isWalking)
    {
        if (perception.player != null)
        {
            float dist = Vector3.Distance(transform.position, perception.player.position);
            if (dist > shootingStopDistance)
            {
                MoveTowardsPlayer();
                isWalking = true;
            }
            else
            {
                FacePlayer();
                actions.SpecialAttack();
                world.DropHazard();
            }
        }
    }

    void HandleAutoDestructionCountdown()
    {
        // Sumamos tiempo si nadie le ha pegado en este frame
        timeWithoutDamage += Time.deltaTime;

        // Si aguantamos 15 segundos...
        if (timeWithoutDamage >= secondsToSurvive)
        {
            phase = BossPhase.Autodestruccion;

            if (!deathSequenceStarted)
            {
                StartCoroutine(DieSequence());
            }
        }
    }

    IEnumerator DieSequence()
    {
        deathSequenceStarted = true;
        isDead = true;

        Debug.Log("Fase Final completada. Iniciando muerte...");


        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.ResetTrigger("Attack");
            animator.Play("IdleBoss");

            yield return null;
            animator.speed = 0f;
        }

        yield return new WaitForSeconds(2f);

        actions.Explode();
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
}