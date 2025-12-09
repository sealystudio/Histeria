using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(AudioSource))]
public class BossHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public BossController boss;
    public Slider slider;
    private CanvasGroup canvasGroup;

    [Header("Configuración")]
    public float appearDistance = 25f; // ¡AQUÍ ESTÁ LA CLAVE! Pon un número mayor que 15

    [Header("Audio")]
    public AudioClip appearSound;
    [Range(0f, 1f)] public float volume = 1f;
    private AudioSource audioSource;

    private bool isBossActive = false;

    void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (boss == null) boss = FindObjectOfType<BossController>();
        canvasGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();

        if (boss != null && slider != null)
        {
            slider.maxValue = boss.maxHP;
            slider.value = boss.currentHP;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    void Update()
    {
        if (boss == null || slider == null) return;

        slider.value = boss.currentHP;

        if (!isBossActive)
        {
            if (boss.perception.player != null)
            {
                float distance = Vector3.Distance(boss.transform.position, boss.perception.player.position);

                // AHORA USAMOS TU DISTANCIA PERSONALIZADA, NO LA DEL BOSS
                if (distance <= appearDistance)
                {
                    ShowHealthBar();
                }
            }
        }

        if (boss.currentHP <= 0)
        {
            canvasGroup.alpha = 0f;
        }
    }

    void ShowHealthBar()
    {
        isBossActive = true;
        canvasGroup.alpha = 1f;

        if (appearSound != null && audioSource != null)
        {
            audioSource.pitch = 1f; // Aseguramos tono normal
            audioSource.PlayOneShot(appearSound, volume);
        }
    }
}