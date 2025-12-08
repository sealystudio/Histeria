using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossMusicManager : MonoBehaviour
{
    [Header("Configuración")]
    [Range(0f, 1f)]
    public float volume = 0.3f; // Volumen sutil ajustable

    [Header("Música")]
    public AudioClip bossMusic;    // Arrastra aquí tu canción principal (Loop)

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;

        // 1. Empezar música inmediatamente en bucle
        if (bossMusic != null)
        {
            audioSource.clip = bossMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }


    // Esto te permite ajustar el volumen en tiempo real mientras juegas en el editor
    void Update()
    {
#if UNITY_EDITOR
        audioSource.volume = volume;
#endif
    }
}