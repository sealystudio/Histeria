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
    
  
    [HideInInspector]
    public List<Transform> availableSpawnPoints = new List<Transform>(); //se llena de los spawnpoints disponibles dependiendo de las salas que toquen aleatoriamente

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
        //espera un momento a que todos los RoomSpawners terminen antes de intentar poner los objetos
        Invoke("PopulateDungeon", populationDelay);
    }


    void PopulateDungeon()
    {
        if (isPopulated) return; 
        isPopulated = true;

      

        //recorremos la lista de objetos que queremos generar
        foreach (GameObject objectPrefab in objectsToSpawn)
        {
            //comprobamos si nos quedan sitios disponibles
            if (availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("¡No quedan Spawn Points! No se pudo generar: " + objectPrefab.name);
                break;//salir del bucle si no hay más sitios
            }

            //generar obj en el spawn disponible
            int randIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randIndex];
            Instantiate(objectPrefab, spawnPoint.position, Quaternion.identity);

            //eliminamos el spawn point de la lista para no volver a usarlo
            availableSpawnPoints.RemoveAt(randIndex);
        }

        //destruir los spawn points que no se usaron
        foreach (Transform point in availableSpawnPoints)
        {
            Destroy(point.gameObject);
        }
    }
}