using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonPopulator : MonoBehaviour
{
    public static DungeonPopulator instance;

    [Header("Configuración de Objetos")]
    public GameObject[] objectsToSpawn;
    public TextMeshProUGUI contadorObjetos;

    [Header("Configuración de Enemigos")]
    public GameObject[] enemiesToSpawn;
    public TextMeshProUGUI contadorEnemigos; // La UI se queda aquí

    [Header("Configuración Interna")]
    public float populationDelay = 1.0f;
    public int enemyNumber = 15; // Este número irá bajando
    public int numObj;

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

        // Actualizamos los textos al inicio
        ActualizarTextoObjetos();
        ActualizarTextoEnemigos();
    }

    void Update()
    {
        // Solo actualizamos objetos si es necesario, pero NO sobrescribimos enemigos aquí
        // para evitar el bug de que el número no baje.
        if (contadorObjetos != null)
            contadorObjetos.text = numObj.ToString();
    }

    // --- FUNCIÓN NUEVA: LLAMAR CUANDO UN ENEMIGO MUERE ---
    public void RestarEnemigo()
    {
        enemyNumber--;
        if (enemyNumber < 0) enemyNumber = 0; // Evitar números negativos

        ActualizarTextoEnemigos();
    }

    void ActualizarTextoObjetos()
    {
        if (contadorObjetos != null) contadorObjetos.text = numObj.ToString();
    }

    void ActualizarTextoEnemigos()
    {
        if (contadorEnemigos != null)
            contadorEnemigos.text = enemyNumber.ToString();
    }
    // -----------------------------------------------------

    void PopulateDungeon()
    {
        if (isPopulated) return;
        isPopulated = true;

        SpawnFromList(objectsToSpawn, objectSpawnPoints);
        SpawnEnemies(enemyNumber);

        // Aseguramos que el texto esté bien tras spawnear
        ActualizarTextoEnemigos();
    }

    void SpawnEnemies(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            if (enemySpawnPoints.Count == 0) return;

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
            if (spawnPoints.Count == 0) break;

            int randIndex = Random.Range(0, spawnPoints.Count);
            Transform spawnPoint = spawnPoints[randIndex];

            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            spawnPoints.RemoveAt(randIndex);
        }

        foreach (Transform point in spawnPoints)
        {
            Destroy(point.gameObject);
        }
    }
}