using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 7f;
    public int damage = 1;      // Cuantos corazones quita
    public float lifeTime = 5f; // Tiempo antes de desaparecer si no choca

    private Vector3 direction;
    private bool isInitialized = false;

    // BossActions llama a esto para darle dirección
    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;

        // Rotamos el sprite para que mire hacia donde va (opcional)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        isInitialized = true;
        Destroy(gameObject, lifeTime); // Autodestrucción por tiempo
    }

    void Update()
    {
        if (!isInitialized) return;

        // Movimiento constante
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Si choca con el JUGADOR
        if (collision.CompareTag("Player"))
        {
            // Buscamos tu script de corazones
            PlayerHealthHearts playerHealth = collision.GetComponent<PlayerHealthHearts>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); //
                Debug.Log("¡El proyectil golpeó al jugador!");
            }

            Destroy(gameObject); // La bola explota/desaparece
        }
        // 2. Si choca con PAREDES (Evitamos que se rompa al salir del propio Boss)
        else if (!collision.isTrigger && !collision.CompareTag("Enemy") && !collision.CompareTag("Boss"))
        {
            // Aquí puedes poner Destroy(gameObject) si quieres que se rompa con paredes
            // Destroy(gameObject); 
        }
    }
}