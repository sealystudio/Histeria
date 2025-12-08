using UnityEngine;
using System.Collections;

public class BossActions : MonoBehaviour
{
    [Header("Configuración Ataque Básico")]
    public float attackCooldown = 2f;
    public int damageAmount = 1;      // Faltaba esta variable
    public float attackRange = 3.5f;  // Faltaba esta variable
    public float damageDelay = 0.5f;  // Faltaba esta variable

    [Header("Ataque Cono (Fase 2)")]
    public GameObject projectilePrefab;
    public float coneAngle = 15f;
    public float shootDelay = 0.5f;

    [Header("Referencias")]
    public Animator animator;
    public GameObject minionPrefab;

    private float lastAttack = 0f;
    private float spawnTimer = 0f;
    private Transform playerTransform;

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    private void TriggerAnimation()
    {
        if (animator != null) animator.SetTrigger("Attack");
    }

    // --- ATAQUE BÁSICO (Cuerpo a cuerpo) ---
    public void BasicAttack()
    {
        if (Time.time - lastAttack > attackCooldown)
        {
            lastAttack = Time.time;
            TriggerAnimation();

            // ¡ESTO ES LO QUE FALTABA! Iniciamos la corutina de daño
            StartCoroutine(DoMeleeDamage());

            Debug.Log("Boss hace ataque básico.");
        }
    }

    // Esta es la función que calcula el golpe físico
    IEnumerator DoMeleeDamage()
    {
        // Esperamos a que baje el brazo
        yield return new WaitForSeconds(damageDelay);

        // Detectamos si el jugador está cerca
        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in objectsHit)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealthHearts playerHealth = hit.GetComponent<PlayerHealthHearts>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount); //
                }
                break; // Solo dañamos una vez
            }
        }
    }

    // --- ATAQUE ESPECIAL (Cono de balas) ---
    public void SpecialAttack()
    {
        if (Time.time - lastAttack > attackCooldown * 0.8f)
        {
            lastAttack = Time.time;
            TriggerAnimation();
            StartCoroutine(ShootConeAttack());
            Debug.Log("Boss dispara en CONO.");
        }
    }

    IEnumerator ShootConeAttack()
    {
        yield return new WaitForSeconds(shootDelay);

        if (playerTransform == null) yield break;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float[] angles = { -coneAngle, 0f, coneAngle };

        foreach (float angle in angles)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 finalDirection = rotation * directionToPlayer;

            // Asegúrate de que projectilePrefab está asignado en el Inspector
            if (projectilePrefab != null)
            {
                GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                BossProjectile script = proj.GetComponent<BossProjectile>();
                if (script != null)
                {
                    script.Initialize(finalDirection);
                }
            }
        }
    }

    public void TrySpawnMinionEvery10Sec()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= 10f)
        {
            TriggerAnimation();
            Instantiate(minionPrefab, transform.position + Vector3.right * 2f, Quaternion.identity);
            spawnTimer = 0f;
        }
    }

    public void Explode()
    {
        Destroy(gameObject);
    }

    // Dibujo para ver el rango en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}