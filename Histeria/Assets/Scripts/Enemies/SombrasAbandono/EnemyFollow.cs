using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 3f;
    public float followDistance = 15f; // Distancia a la que el enemigo empieza a seguir
    Transform player;
    Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            Debug.LogError("NO SE ENCONTRÓ PLAYER.");
        else
            Debug.Log("Enemigo encontró a Eli correctamente.");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Calcula la distancia entre el enemigo y el jugador
        float distance = Vector2.Distance(transform.position, player.position);

        // Solo seguir si está dentro de followDistance
        if (distance <= followDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;

            if (rb != null)
            {
                rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
            }
            else
            {
                transform.Translate(dir * speed * Time.deltaTime);
            }
        }
    }
}
