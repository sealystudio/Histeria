using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 7f;
    public int damage = 1;      
    public float lifeTime = 5f;

    private Vector3 direction;
    private bool isInitialized = false;

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        isInitialized = true;
        Destroy(gameObject, lifeTime); 
    }

    void Update()
    {
        if (!isInitialized) return;


        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {

            PlayerHealthHearts playerHealth = collision.GetComponent<PlayerHealthHearts>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); //
                Debug.Log("¡El proyectil golpeó al jugador!");
            }

            Destroy(gameObject); 
        }

        else if (!collision.isTrigger && !collision.CompareTag("Enemy") && !collision.CompareTag("Boss"))
        {
            Destroy(gameObject); 
        }
    }
}