using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 3f;
    public float followDistance = 15f;

    Transform player;
    Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            Debug.LogError("NO SE ENCONTRÓ PLAYER.");
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= followDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;

            rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

            // giro visual
            transform.localScale = player.position.x > transform.position.x
                ? new Vector3(1, 1, 1)
                : new Vector3(-1, 1, 1);
        }
    }
}
