using System.Collections.Generic;
using UnityEngine;

public class DungeonPopulator : MonoBehaviour
{
    public static DungeonPopulator instance;

    [Header("Configuración de Nivel")]
    [Tooltip("La lista de prefabs de objetos (llaves, pociones, etc.) que DEBEN aparecer en este nivel.")]
    public GameObject[] objectsToSpawn;

    [Header("Lógica Interna")]
    [Tooltip("Delay (en segundos) para esperar a que el mapa se termine de generar antes de poner objetos.")]
    public float populationDelay = 1.0f; 
    
    // Esta lista se llenará sola
    [HideInInspector]
    public List<Transform> availableSpawnPoints = new List<Transform>();

    private bool isPopulated = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Espera un momento a que todos los RoomSpawners terminen
        // antes de intentar poner los objetos.
        Invoke("PopulateDungeon", populationDelay);
    }

    /// <summary>
    /// Esta es la función principal que implementa tu idea.
    /// </summary>
    void PopulateDungeon()
    {
        if (isPopulated) return; // Asegurarse de que solo se ejecuta una vez
        isPopulated = true;

        // --- Este es el núcleo de tu plan ---

        // 1. Recorrer la lista de objetos que queremos generar
        foreach (GameObject objectPrefab in objectsToSpawn)
        {
            // 2. Comprobar si nos quedan sitios disponibles
            if (availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("¡No quedan Spawn Points! No se pudo generar: " + objectPrefab.name);
                break; // Salir del bucle si no hay más sitios
            }

            // 3. Elegir un Spawn Point aleatorio de la lista de disponibles
            int randIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randIndex];

            // 4. Generar el objeto en esa posición
            Instantiate(objectPrefab, spawnPoint.position, Quaternion.identity);

            // 5. ¡Importante! Eliminar el spawn point de la lista
            // para no volver a usarlo.
            availableSpawnPoints.RemoveAt(randIndex);
        }

        // Opcional: Destruir los spawn points que no se usaron
        foreach (Transform point in availableSpawnPoints)
        {
            Destroy(point.gameObject);
        }
    }
}