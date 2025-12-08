using UnityEngine;

public class LagrimasAttack : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public int damage = 1;

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

        if (Time.time - spawnTime < spawnIgnoreTime) return;

        // --- SI DISPARA EL JUGADOR ---
        if (team == Team.Player)
        {

            EnemyBase enemigo = collision.GetComponent<EnemyBase>();
            if (enemigo != null)
            {
                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                enemigo.TakeDamage(damage, hitDirection);
                Destroy(gameObject);
                return; 
            }


            BossController boss = collision.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Destroy(gameObject); 
                return;
            }

            SombraAbandono sombra = collision.GetComponent<SombraAbandono>();
            if (sombra != null)
            {
                // sombra.TakeDamage(damage); // Descomenta si tienes método de daño en la sombra
                Destroy(gameObject);
                return;
            }

            MinionAI minion = collision.GetComponent<MinionAI>();
            if (minion != null)
            {
                minion.TakeDamage(damage); //
                Destroy(gameObject);       // Destruye la lágrima
                return;
            }
        }
        // --- SI DISPARA EL ENEMIGO (ELI CORRUPTA) ---
        else if (team == Team.Corrupta)
        {
            PlayerHealthHearts player = collision.GetComponent<PlayerHealthHearts>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        if (!collision.isTrigger && !collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
