using UnityEngine;

public class LagrimasAttack : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public int damage = 1;

    // Nuevo: equipo/origen del proyectil
    public enum Team { Player, Corrupta }
    public Team team = Team.Player;

    private Vector3 direction;
    private float spawnIgnoreTime = 0.05f; // evitar daño inmediato al nacer
    private float spawnTime;

    public void Initialize(Vector3 dir, Team origin = Team.Player)
    {
        if (dir.sqrMagnitude < 0.001f) dir = Vector3.right;
        direction = dir.normalized;
        team = origin;
        spawnTime = Time.time;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evitar colisiones inmediatamente al spawnear (solapes iniciales)
        if (Time.time - spawnTime < spawnIgnoreTime) return;

        if (team == Team.Player)
        {
            // Las balas del jugador dañan a cualquier EnemyBase, incluida Eli corrupta
            EnemyBase enemigo = collision.GetComponent<EnemyBase>();
            if (enemigo != null)
            {
                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                enemigo.TakeDamage(damage, hitDirection);
                Destroy(gameObject);
            }
        }
        else if (team == Team.Corrupta)
        {
            // Las balas corruptas solo dañan al jugador
            PlayerHealthHearts player = collision.GetComponent<PlayerHealthHearts>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

}
