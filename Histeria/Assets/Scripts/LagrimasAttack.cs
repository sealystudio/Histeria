using UnityEngine;

public class LagrimasAttack : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public int damage = 1;

    private Vector3 direction;

    public void Initialize(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.001f)
            dir = Vector3.right; // dirección por defecto si el vector es casi cero

        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Daño a cualquier enemigo que herede de EnemyBase
        EnemyBase enemigo = collision.GetComponent<EnemyBase>();
        if (enemigo != null)
        {
            // Dirección del impacto para knockback
            Vector2 hitDirection = (collision.transform.position - transform.position).normalized;

            // Aplicar daño genérico
            enemigo.TakeDamage(damage, hitDirection);

            // Destruir la lágrima
            Destroy(gameObject);
        }
    }
}