using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public BossContext bossContext; // ANTES: BossController
    public Slider slider;
    private CanvasGroup canvasGroup;

    [Header("Configuración")]
    public float appearDistance = 25f; // Distancia para que aparezca la barra

    [Header("Audio")]
    public AudioClip appearSound;
    [Range(0f, 1f)] public float volume = 1f;
    private AudioSource audioSource;

    private bool isBarActive = false;

    void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();

        // AQUÍ EL CAMBIO: Buscamos el Contexto, no el Controller antiguo
        if (bossContext == null) bossContext = FindObjectOfType<BossContext>();

        canvasGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();

        if (bossContext != null && slider != null)
        {
            slider.maxValue = bossContext.maxHP;
            slider.value = bossContext.currentHP;
        }

        if (canvasGroup == null)
        {
            // Si no tienes CanvasGroup, intenta añadirlo o busca el componente
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Empezamos invisibles
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (bossContext == null || slider == null) return;

        // Actualizamos el valor de la vida desde el nuevo sistema
        slider.value = bossContext.currentHP;

        // Lógica de aparición
        if (!isBarActive)
        {
            // Usamos playerTransform que ya está guardado en el contexto
            if (bossContext.playerTransform != null)
            {
                float distance = Vector3.Distance(bossContext.transform.position, bossContext.playerTransform.position);

                if (distance < appearDistance)
                {
                    ShowHealthBar();
                }
            }
        }

        // Si muere, ocultamos
        if (bossContext.currentHP <= 0)
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }
    }

    void ShowHealthBar()
    {
        isBarActive = true;
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        if (audioSource != null && appearSound != null)
        {
            audioSource.PlayOneShot(appearSound, volume);
        }
    }
}