using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager instance; // Para acceder desde cualquier lado
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void ReproducirNarrativa(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.clip = clip;
        audioSource.Play();
    }
}