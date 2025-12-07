using UnityEngine;

public class BossActions : MonoBehaviour
{
    public float attackCooldown = 2f;
    float lastAttack = 0f;

    public GameObject minionPrefab;
    private float spawnTimer = 0f;

    public void BasicAttack()
    {
        if (Time.time - lastAttack > attackCooldown)
        {
            lastAttack = Time.time;
            Debug.Log("Boss hace ataque básico.");
        }
    }

    public void SpecialAttack()
    {
        if (Time.time - lastAttack > attackCooldown * 0.7f)
        {
            lastAttack = Time.time;
            Debug.Log("Boss hace ATAQUE ESPECIAL.");
        }
    }

    public void TrySpawnMinionEvery10Sec()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= 10f)
        {
            Instantiate(minionPrefab, transform.position + Vector3.right * 2f, Quaternion.identity);
            spawnTimer = 0f;
        }
    }

    public void Explode()
    {
        Debug.Log(" El boss se AUTODESTRUYE");
        Destroy(gameObject);
    }
}
