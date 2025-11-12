using UnityEngine;

public class EnemiesSpawnPoint : MonoBehaviour
{
    void Start()
    {
        if (DungeonPopulator.instance != null)
        {
            DungeonPopulator.instance.enemySpawnPoints.Add(this.transform);
        }
        else
        {
            Debug.LogWarning("No se encontró DungeonPopulator.instance. El spawn point de enemigo no se pudo registrar.");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f); // Rojo semitransparente
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
