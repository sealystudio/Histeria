using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Objetos de la escena")]
    public GameObject objetoCambioEscena; // objeto que aparece al terminar nivel
    public GameObject eli;                // referencia al jugador

    [Header("DungeonPopulator")]
    public bool usaDungeonPopulator = true; // marcar en inspector si la escena tiene enemigos
    private DungeonPopulator dp;

    [Header("Contador enemigos")]
    public int numeroDeEnemigos;
    private bool objetoAparecido = false;
    private bool contadorInicializado = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // si quieres que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
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
            StartCoroutine(InitContadorEnemigos());
        }
        else
        {
            contadorInicializado = true;
        }
    }

    private IEnumerator InitContadorEnemigos()
    {
        // Esperar a que exista el DungeonPopulator
        while (dp == null && usaDungeonPopulator)
        {
            dp = FindAnyObjectByType<DungeonPopulator>();
            yield return null;
        }

        // Esperar un pequeño tiempo a que se populen los enemigos
        float wait = usaDungeonPopulator ? Mathf.Max(0f, dp.populationDelay) + 0.1f : 0f;
        yield return new WaitForSeconds(wait);

        // Contar todos los enemigos que hereden de EnemyBase
        var enemigos = FindObjectsOfType<EnemyBase>();
        numeroDeEnemigos = enemigos.Length;

        contadorInicializado = true;

        Debug.Log("[LevelManager] Enemigos iniciales contabilizados: " + numeroDeEnemigos);
    }

    private void Update()
    {
        if (!contadorInicializado) return;

        // Solo dropear objeto cuando el contador llega a 0
        if (numeroDeEnemigos <= 0 && !objetoAparecido)
        {
            DropObject();
        }
    }

    /// <summary>
    /// Llamar desde cualquier enemigo cuando muere
    /// </summary>
    public void EnemyMuerto()
    {
        numeroDeEnemigos--;

        Debug.Log("[LevelManager] Enemigo muerto. Restantes: " + numeroDeEnemigos);

        if (numeroDeEnemigos <= 0 && !objetoAparecido)
        {
            DropObject();
        }
    }

    private void DropObject()
    {
        if (eli == null || objetoCambioEscena == null) return;

        objetoCambioEscena.transform.position = eli.transform.position;
        objetoCambioEscena.SetActive(true);
        objetoAparecido = true;

        Debug.Log("[LevelManager] Objeto de cambio de escena activado.");
    }

    /// <summary>
    /// Carga una escena por nombre
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[LevelManager] Nombre de escena vacío.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Carga la siguiente escena según build index
    /// </summary>
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }
}