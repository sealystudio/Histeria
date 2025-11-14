using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Mantener entre escenas
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
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
}
