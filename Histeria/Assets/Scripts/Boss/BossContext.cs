using UnityEngine;

public class BossContext : MonoBehaviour
{
    [Header("Estado Vital")]
    public float maxHP = 1000f;
    public float currentHP;
    public bool isInvulnerable = false; // Para la fase final
    public float timeSinceLastDamage = 0f; // Para reiniciar la cuenta atrás en fase final

    [Header("Referencias a Actuadores")]
    public BossActions actions;
    public BossWorldInteraction worldInteraction;
    public Transform playerTransform;
    public Animator animator;
    public float moveSpeed = 2.0f;

    private void Awake()
    {
        currentHP = maxHP;

        // Autocompletar referencias si faltan
        if (!actions) actions = GetComponent<BossActions>();
        if (!worldInteraction) worldInteraction = GetComponent<BossWorldInteraction>();
        if (!animator) animator = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) playerTransform = player.transform;
    }

    public void TakeDamage(float amount)
    {
        if (isInvulnerable)
        {
            timeSinceLastDamage = 0f; // Reiniciamos contador si le pegan en fase final
            Debug.Log("Boss Invulnerable: Contador reiniciado.");
            return;
        }

        currentHP -= amount;
        timeSinceLastDamage = 0f;

        Debug.Log($"Vida Boss: {currentHP}/{maxHP}");
    }

    private void Update()
    {
        // Contador global de tiempo sin daño
        timeSinceLastDamage += Time.deltaTime;
    }


    // 1. Porcentaje de vida (0.0 a 1.0) -> Lo usa BossBrain.IsPhase...
    public float HealthPercentage
    {
        get { return Mathf.Clamp01(currentHP / maxHP); }
    }

    // 2. Tiempo sobrevivido -> Lo usa BossBrain.IsSurvivalTimeOver
    public float SurvivalTime => timeSinceLastDamage;

    // 3. Distancia al jugador -> Lo usa BossBrain.IsPlayerCloseMelee
    public float DistanceToPlayer
    {
        get
        {
            if (playerTransform == null) return 999f; // Si no hay jugador, devolvemos "muy lejos"
            return Vector3.Distance(transform.position, playerTransform.position);
        }
    }
}