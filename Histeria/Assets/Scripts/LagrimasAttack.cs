using UnityEngine;

public class LagrimasAttack : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 10f;
    public float lifeTime = 2f;
    public int damage = 1;

    [Header("Audio Impacto")]
    public AudioClip hitSound; 
    [Range(0f, 1f)] public float hitVolume = 1f;

    public enum Team { Player, Corrupta }
    public Team team = Team.Player;

    private Vector3 direction;
    private float spawnIgnoreTime = 0.05f;
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

    void PlayHitSound()
    {
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, hitVolume);
        }
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
                enemigo.TakeDamage(damage, (collision.transform.position - transform.position).normalized);
                PlayHitSound(); 
                Destroy(gameObject);
                return;
            }
            BossController boss = collision.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                PlayHitSound(); 
                Destroy(gameObject);
                return;
            }
            SombraAbandono sombra = collision.GetComponent<SombraAbandono>();
            if (sombra != null)
            {

                PlayHitSound(); 
                Destroy(gameObject);
                return;
            }


            MinionAI minion = collision.GetComponent<MinionAI>();
            if (minion != null)
            {
                minion.TakeDamage(damage);
                PlayHitSound(); 
                Destroy(gameObject);
                return;
            }
        }

        else if (team == Team.Corrupta)
        {
            PlayerHealthHearts player = collision.GetComponent<PlayerHealthHearts>();
            if (player != null)
            {
                player.TakeDamage(damage);
                PlayHitSound(); 
                Destroy(gameObject);
                return;
            }
        }

        if (!collision.isTrigger && !collision.CompareTag("Player"))
        {
            PlayHitSound(); 
            Destroy(gameObject);
        }
    }
}