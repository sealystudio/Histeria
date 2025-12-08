using UnityEngine;

public class MinionAI : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 3.5f;  
    public int damage = 1;      



    private Transform player;

    void Start()
    {

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        Destroy(gameObject, 15f);
    }

    void Update()
    {
        if (player == null) return;

        // LÓGICA DE PERSECUCIÓN
        // 1. Moverse hacia el jugador
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1); 
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            PlayerHealthHearts health = collision.GetComponent<PlayerHealthHearts>();
            if (health != null)
            {
                health.TakeDamage(damage); //
                Debug.Log("¡Minion te mordió!");
            }

            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        Destroy(gameObject);
    }
}