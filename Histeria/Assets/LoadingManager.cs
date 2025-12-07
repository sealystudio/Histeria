using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;

    public GameObject loadingPanel;
    public float showTime = 3f;

    void Awake()
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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // La primera escena empieza con el panel activo
        StartCoroutine(ShowAndHide());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cada escena nueva → mostrar panel
        StartCoroutine(ShowAndHide());
    }

    System.Collections.IEnumerator ShowAndHide()
    {
        loadingPanel.SetActive(true);

        yield return new WaitForSeconds(showTime);

        loadingPanel.SetActive(false);
    }
}
