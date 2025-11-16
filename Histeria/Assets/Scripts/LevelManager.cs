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
    public bool usaDungeonPopulator = true; // ✅ marcar en el inspector si la escena usa enemigos

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
        }

        if (usaDungeonPopulator)
        {
            dp = FindAnyObjectByType<DungeonPopulator>();
            if (dp != null)
            {
                numeroDeSombras = dp.enemyNumber;
                objetoCambioEscena.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (usaDungeonPopulator)
        {
            // 🔹 Solo se ejecuta en niveles con enemigos
            if (dp == null)
            {
                dp = FindAnyObjectByType<DungeonPopulator>();
                if (dp == null) return; // Evita excepción
            }

            if (numeroDeSombras < 1 && !objetoAparecido)
            {
                dropObject();
            }
        }
        else
        {
            // 🔹 En tutorial no hay DungeonPopulator
            // Aquí puedes poner la condición de inventario
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
        Vector3 posEli = eli.transform.position;
        objetoCambioEscena.transform.position = posEli;
        objetoCambioEscena.gameObject.SetActive(true);
        objetoAparecido = true;
    }

    // 🔹 Método que puedes conectar con tu sistema de inventario del tutorial
    private bool TutorialItemConsumido()
    {
        // Aquí pones la lógica de inventario del tutorial
        // Ejemplo: return Inventory.instance.HasConsumed("LlaveTutorial");
        return false;
    }
}
