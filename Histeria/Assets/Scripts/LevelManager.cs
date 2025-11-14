using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public GameObject objetoCollar;
    public GameObject eli;

    private DungeonPopulator dp;
    public int numeroDeSombras;
    private bool objetoAparecido = false;
    private void Awake()
    {
        dp = FindAnyObjectByType<DungeonPopulator>();
        numeroDeSombras = dp.enemyNumber;


        objetoCollar.gameObject.SetActive(false);
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

    private void Update()
    {

        if (dp == null)
        {
            dp = FindAnyObjectByType<DungeonPopulator>();
            if (dp == null) return; // Evita la excepción
        }

        if (numeroDeSombras < 1 && !objetoAparecido)
        {
            dropObject();
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
        objetoCollar.transform.position = posEli;

        objetoCollar.gameObject.SetActive(true);

        objetoAparecido = true;
    }
}
