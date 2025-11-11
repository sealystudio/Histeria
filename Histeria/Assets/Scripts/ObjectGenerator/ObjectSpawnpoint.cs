using UnityEngine;

public class ObjectSpawnPoint : MonoBehaviour
{
    // Esta función se llama tan pronto como el prefab de la habitación se instancia
    void Start()
    {
        // 1. Busca el manager
        if (DungeonPopulator.instance != null)
        {
            // 2. Se añade a sí mismo a la lista de "disponibles"
            DungeonPopulator.instance.availableSpawnPoints.Add(this.transform);
        }
        else
        {
            Debug.LogWarning("No se encontró DungeonPopulator.instance. El spawn point no se pudo registrar.");
        }
    }

    // (Opcional) Esto dibuja un icono en tu editor de escenas
    // para que puedas ver dónde están tus spawn points.
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f); // Verde semitransparente
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}