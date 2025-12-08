using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Objetos de la escena")]
    public GameObject objetoCambioEscena;
    public GameObject eli;

    [Header("Contador enemigos")]
    public int numeroDeEnemigos;
    private bool objetoAparecido = false;

    private bool contadorInicializado = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
        StartCoroutine(InitContador());
    }

    private IEnumerator InitContador()
    {
        // esperamos a que se spawneen los enemigos
        yield return new WaitForEndOfFrame();

        EnemyBase[] enemigos = FindObjectsOfType<EnemyBase>();
        numeroDeEnemigos = enemigos.Length;

        contadorInicializado = true;

        Debug.Log("[LevelManager] Enemigos iniciales: " + numeroDeEnemigos);
    }

    private void Update()
    {
        if (!contadorInicializado) return;

        if (numeroDeEnemigos <= 0 && !objetoAparecido)
        {
            DropObject();
        }
    }

    // llamado por cada enemigo al morir
    public void EnemyMuerto()
    {
        numeroDeEnemigos--;
        Debug.Log("[LevelManager] Enemigo muerto. Restantes: " + numeroDeEnemigos);
    }

    private void DropObject()
    {
        if (eli == null || objetoCambioEscena == null) return;

        objetoCambioEscena.transform.position = eli.transform.position;
        objetoCambioEscena.SetActive(true);
        objetoAparecido = true;

        Debug.Log("[LevelManager] Objeto activado.");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadNextScene()
    {
        int idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(idx + 1);
    }
}
