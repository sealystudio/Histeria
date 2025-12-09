using UnityEngine;

public class MobileUIManager : MonoBehaviour
{
    // Variable para controlar que solo exista uno
    public static MobileUIManager instance;

    public GameObject canvasMobile;

    void Awake()
    {
        // --- INICIO SINGLETON ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ¡Hazme inmortal!
        }
        else
        {
            Destroy(gameObject); // Ya existe uno, así que yo sobro.
            return; // Importante para que no siga ejecutando código
        }
        // --- FIN SINGLETON ---
    }

    void Start()
    {
        // Tu lógica original para mostrarlo solo en móvil/editor
#if UNITY_ANDROID || UNITY_IOS //|| UNITY_EDITOR
        if (canvasMobile != null)
            canvasMobile.SetActive(true);
#else
        if(canvasMobile != null) 
            canvasMobile.SetActive(false);
#endif
    }
}