using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelMusic : MonoBehaviour
{
    [Header("Configuración")]
    public AudioClip musicClip; 

    [Range(0f, 1f)]
    public float volume = 0.3f; 
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.volume = volume;

            audioSource.loop = true;

            audioSource.playOnAwake = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("¡Falta asignar la canción en el script LevelMusic!");
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (audioSource != null)
            audioSource.volume = volume;
#endif
    }
}