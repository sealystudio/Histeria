using UnityEngine;
using System.Collections;
// Si tu API usa un namespace específico para StatusFlag, añádelo aquí o usa el enum propio
// using BehaviourAPI; 

public class BossActions : MonoBehaviour
{
    [Header("Configuración Ataque Básico")]
    public int damageAmount = 1;
    public float attackRange = 3.5f;
    public float damageDelay = 0.5f;

    [Header("Ataque Cono (Fase 2)")]
    public GameObject projectilePrefab;
    public float coneAngle = 15f;
    public float shootDelay = 0.5f;

    [Header("Referencias")]
    public Animator animator;
    public GameObject minionPrefab;

    private Transform playerTransform;

    // --- ESTADO PARA LA API ---
    // Esta variable le dice a la IA si el Boss sigue haciendo la animación
    public bool IsBusy { get; private set; } = false;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    private void TriggerAnimation()
    {
        if (animator != null) animator.SetTrigger("Attack");
    }

    // --- MÉTODOS DE EJECUCIÓN ---
    // Ahora comprueban si ya está ocupado para no "tartamudear"

    public void ExecuteBasicAttack()
    {
        if (IsBusy) return; // Si ya está atacando, no reiniciamos

        IsBusy = true; // ¡Ocupado!
        TriggerAnimation();
        StartCoroutine(DoMeleeDamage());
        Debug.Log("IA: Iniciando Ataque Básico");
    }

    public void ExecuteSpecialAttack()
    {
        if (IsBusy) return;

        IsBusy = true;
        TriggerAnimation();
        StartCoroutine(ShootConeAttack());
        Debug.Log("IA: Iniciando Ataque Cono");
    }

    public void ExecuteSpawnMinion()
    {
        if (IsBusy) return;

        IsBusy = true;
        TriggerAnimation();
        StartCoroutine(SpawnMinionRoutine()); // Lo metemos en corutina para gestionar el tiempo
        Debug.Log("IA: Spawneando Minion");
    }

    public void ExecuteExplosion()
    {
        Destroy(gameObject);
    }

    // --- CORUTINAS (Gestionan el tiempo y liberan el estado) ---

    IEnumerator DoMeleeDamage()
    {
        // Esperamos el tiempo del golpe
        yield return new WaitForSeconds(damageDelay);

        // Lógica de daño
        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in objectsHit)
        {
            if (hit.CompareTag("Player"))
            {
                var playerHealth = hit.GetComponent<PlayerHealthHearts>();
                if (playerHealth != null) playerHealth.TakeDamage(damageAmount);
                break;
            }
        }

        // Esperamos un poco más para terminar la animación (opcional, ajusta según tu animación)
        yield return new WaitForSeconds(0.5f);

        IsBusy = false; // ¡LIBRE! La IA ya puede mandar otra orden
    }

    IEnumerator ShootConeAttack()
    {
        yield return new WaitForSeconds(shootDelay);

        if (playerTransform != null)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            float[] angles = { -coneAngle, 0f, coneAngle };

            foreach (float angle in angles)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Vector3 finalDirection = rotation * directionToPlayer;

                if (projectilePrefab != null)
                {
                    GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    BossProjectile script = proj.GetComponent<BossProjectile>();
                    if (script != null) script.Initialize(finalDirection);
                }
            }
        }

        yield return new WaitForSeconds(0.5f); // Tiempo de recuperación
        IsBusy = false;
    }

    IEnumerator SpawnMinionRoutine()
    {
        // Simulamos que tarda un poco en invocar
        yield return new WaitForSeconds(0.5f);
        Instantiate(minionPrefab, transform.position + Vector3.right * 2f, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        IsBusy = false;
    }
}