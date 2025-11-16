using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public GameObject objetoCambioEscena; // objeto que aparece al terminar nivel
    public GameObject eli;                // referencia al jugador

    private DungeonPopulator dp;
    public int numeroDeSombras;
    private bool objetoAparecido = false;

    [Header("Configuración de escena")]
    public bool usaDungeonPopulator = true; // marcar en el inspector si la escena usa enemigos

    // Evitar que Update dispare lógica antes de inicializar el contador
    private bool contadorInicializado = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
            return;
        }

        if (objetoCambioEscena != null)
            objetoCambioEscena.SetActive(false);
    }

    private void Start()
    {
        if (usaDungeonPopulator)
        {
            dp = FindAnyObjectByType<DungeonPopulator>();
            // Inicializamos el contador de forma segura, esperando a que el dungeon esté poblado
            StartCoroutine(InitContadorSombras());
        }
        else
        {
            // En escenas sin enemigos, no usamos el contador
            contadorInicializado = true;
        }
    }

    private IEnumerator InitContadorSombras()
    {
        // Esperar a que exista el DungeonPopulator
        while (dp == null)
        {
            dp = FindAnyObjectByType<DungeonPopulator>();
            yield return null;
        }

        // Esperar a que termine de poblar (usa populationDelay en DP). 
        float wait = Mathf.Max(0f, dp.populationDelay) + 0.1f;
        yield return new WaitForSeconds(wait);

        // Contar enemigos reales presentes
        var enemigos = FindObjectsOfType<SombraAbandono>();
        numeroDeSombras = enemigos.Length;

        contadorInicializado = true;

        Debug.Log("[LevelManager] Enemigos iniciales contabilizados: " + numeroDeSombras);
    }

    private void Update()
    {
        if (!contadorInicializado) return;

        if (usaDungeonPopulator)
        {
            // Solo aparece cuando el contador llega exactamente a 0 (tras el último enemigo)
            if (numeroDeSombras == 0 && !objetoAparecido)
            {
                dropObject();
            }
        }
        else
        {
            // En tutorial no hay DungeonPopulator
            if (TutorialItemConsumido() && !objetoAparecido)
            {
                dropObject();
            }
        }
    }

    // Cargar escena concreta por nombre
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[LevelManager] Nombre de escena vacío.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    // Avanzar a la siguiente escena en build index
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

    private void dropObject()
    {
        if (eli == null || objetoCambioEscena == null) return;

        Vector3 posEli = eli.transform.position;
        objetoCambioEscena.transform.position = posEli;
        objetoCambioEscena.SetActive(true);
        objetoAparecido = true;

        Debug.Log("[LevelManager] Objeto de cambio de escena activado.");
    }

    // Método que puedes conectar con tu sistema de inventario del tutorial
    private bool TutorialItemConsumido()
    {
        // Aquí pones la lógica de inventario del tutorial
        // Ejemplo: return Inventory.instance.HasConsumed("LlaveTutorial");
        return false;
    }
}
