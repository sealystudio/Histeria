using UnityEngine;

public class MinionAI : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 3.5f;   // Velocidad a la que persigue
    public int damage = 1;       // Daño al tocar al jugador

    // Opcional: Vida del minion si quieres que se puedan matar
    // (Por ahora morirá de un golpe si le disparas gracias a tu LagrimasAttack)

    private Transform player;

    void Start()
    {
        // Busca al jugador automáticamente al nacer
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Opcional: Destruirlo después de 15 segundos si no te alcanza, para no llenar el mapa
        Destroy(gameObject, 15f);
    }

    void Update()
    {
        if (player == null) return;

        // LÓGICA DE PERSECUCIÓN
        // 1. Moverse hacia el jugador
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        // 2. Girar el sprite para mirar al jugador (Opcional, si tienes sprite lateral)
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1); // Mira derecha (asumiendo sprite original a derecha)
        else
            transform.localScale = new Vector3(-1, 1, 1); // Mira izquierda
    }

    // CUANDO TOCA ALGO
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si toca al JUGADOR
        if (collision.CompareTag("Player"))
        {
            PlayerHealthHearts health = collision.GetComponent<PlayerHealthHearts>();
            if (health != null)
            {
                health.TakeDamage(damage); //
                Debug.Log("¡Minion te mordió!");
            }

            // El minion muere al impactar (estilo Kamikaze)
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        Destroy(gameObject);
    }
}