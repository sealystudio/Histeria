using UnityEngine;

public class LagrimasAttack : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;

    private Vector3 direction;

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime); // se destruye solo después de un tiempo
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}