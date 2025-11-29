using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonPopulator : MonoBehaviour
{
    public static DungeonPopulator instance;

    [Header("Configuración de Objetos")]
    [Tooltip("La lista de prefabs de objetos (llaves, pociones, etc.) que DEBEN aparecer en este nivel.")]
    public GameObject[] objectsToSpawn;
    public TextMeshProUGUI contadorObjetos;
    [Header("Configuración de Enemigos")]
    [Tooltip("La lista de prefabs de enemigos que pueden aparecer en este nivel.")]
    public GameObject[] enemiesToSpawn;
    public TextMeshProUGUI contadorEnemigos;

    [Header("Configuración Interna")]
    [Tooltip("Delay (en segundos) para esperar a que el mapa se termine de generar antes de poblar.")]
    public float populationDelay = 1.0f;
    public int enemyNumber = 15;
    public int numObj;
    // Listas separadas para los distintos tipos de spawn
    [HideInInspector] public List<Transform> objectSpawnPoints = new List<Transform>();
    [HideInInspector] public List<Transform> enemySpawnPoints = new List<Transform>();

    private bool isPopulated = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Invoke("PopulateDungeon", populationDelay);
        numObj = objectsToSpawn.Length;
    }
    void Update()
    {
        // Actualizar los textos en cada frame
        contadorObjetos.text = numObj.ToString();
        contadorEnemigos.text = enemyNumber.ToString();
    }

    void PopulateDungeon()
    {
        if (isPopulated) return;
        isPopulated = true;

        // Spawnear objetos
        SpawnFromList(objectsToSpawn, objectSpawnPoints);

        // Spawnear enemigos
        SpawnEnemies(enemyNumber);
    }

    void SpawnEnemies(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            if (enemySpawnPoints.Count == 0)
            {
                Debug.LogWarning("No quedan spawn points para enemigos.");
                return;
            }

            int randIndex = Random.Range(0, enemySpawnPoints.Count);
            int randEnemy = Random.Range(0, enemiesToSpawn.Length);

            Transform point = enemySpawnPoints[randIndex];
            Instantiate(enemiesToSpawn[randEnemy], point.position, Quaternion.identity);

            enemySpawnPoints.RemoveAt(randIndex);
            Destroy(point.gameObject);
        }
    }


    private void SpawnFromList(GameObject[] prefabs, List<Transform> spawnPoints)
    {
        foreach (GameObject prefab in prefabs)
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("¡No quedan Spawn Points! No se pudo generar: " + prefab.name);
                break;
            }

            int randIndex = Random.Range(0, spawnPoints.Count);
            Transform spawnPoint = spawnPoints[randIndex];

            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            spawnPoints.RemoveAt(randIndex);
        }

        // Destruir los spawn points que no se usaron
        foreach (Transform point in spawnPoints)
        {
            Destroy(point.gameObject);
        }
    }
}