using UnityEngine;

public class ObjectSpawnPoint : MonoBehaviour
{
    void Start()
    {
        if (DungeonPopulator.instance != null)
        {
            DungeonPopulator.instance.objectSpawnPoints.Add(this.transform);
        }
        else
        {
            Debug.LogWarning("No se encontró DungeonPopulator.instance. El spawn point no se pudo registrar.");
        }
    }

    //funcion visual para ver los spawnpoints
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f); 
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}