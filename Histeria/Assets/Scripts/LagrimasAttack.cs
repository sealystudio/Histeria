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
        // Evitar colisiones inmediatamente al spawnear
        if (Time.time - spawnTime < spawnIgnoreTime) return;

        // --- SI DISPARA EL JUGADOR ---
        if (team == Team.Player)
        {
            // 1. Detectar Enemigos normales (Eli Corrupta, etc.)
            EnemyBase enemigo = collision.GetComponent<EnemyBase>();
            if (enemigo != null)
            {
                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                enemigo.TakeDamage(damage, hitDirection);
                Destroy(gameObject);
                return; // Importante salir para no seguir comprobando
            }

            // 2. Detectar al BOSS (AQUÍ ESTÁ EL AÑADIDO)
            BossController boss = collision.GetComponent<BossController>();
            if (boss != null)
            {
                // El Boss recibe el daño. Como tu variable 'damage' es int y el boss pide float, 
                // Unity lo convierte automáticamente.
                boss.TakeDamage(damage);
                Destroy(gameObject); // Destruir la lágrima
                return;
            }

            // 3. Detectar Sombras (Opcional, si quieres que las lágrimas les den)
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

        // Destruir si choca con paredes (que no sean triggers)
        if (!collision.isTrigger && !collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
