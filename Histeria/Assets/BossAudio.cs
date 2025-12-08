using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossAudio : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Pasos (Andar)")]
    public AudioClip[] stepSounds;
    [Range(0f, 1f)] public float stepVolume = 0.5f;

    [Header("Ataque (Golpe al suelo)")]
    public AudioClip smashSound; // Arrastra aquí tu sonido de golpe
    [Range(0f, 1f)] public float smashVolume = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Función para los pasos (ya la tenías)
    public void PlayStep()
    {
        if (stepSounds.Length > 0)
        {
            int index = Random.Range(0, stepSounds.Length);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(stepSounds[index], stepVolume);
        }
    }

    // --- NUEVA FUNCIÓN PARA EL GOLPE ---
    public void PlaySmash()
    {
        if (smashSound != null)
        {
            // Variamos muy poquito el tono para darle "vida", pero menos que los pasos
            audioSource.pitch = Random.Range(0.95f, 1.05f);

            // Reproducimos el golpe
            audioSource.PlayOneShot(smashSound, smashVolume);
        }
    }
}