using UnityEngine;

public class IntroLoopMusic : MonoBehaviour
{
    [Header("Configuración")]
    [Range(0f, 1f)] public float volume = 0.3f;

    [Header("Archivos de Audio")]
    public AudioClip musicIntro; 
    public AudioClip musicLoop;  

    private AudioSource sourceIntro;
    private AudioSource sourceLoop;

    void Start()
    {

        sourceIntro = gameObject.AddComponent<AudioSource>();
        sourceLoop = gameObject.AddComponent<AudioSource>();


        sourceIntro.volume = volume;
        sourceLoop.volume = volume;

        sourceIntro.playOnAwake = false;
        sourceLoop.playOnAwake = false;


        sourceIntro.clip = musicIntro;
        sourceLoop.clip = musicLoop;


        sourceIntro.loop = false;
        sourceLoop.loop = true;


        if (musicIntro != null && musicLoop != null)
        {

            double startTime = AudioSettings.dspTime + 0.1; 

            sourceIntro.PlayScheduled(startTime);

            double loopStartTime = startTime + (double)musicIntro.samples / musicIntro.frequency;

            sourceLoop.PlayScheduled(loopStartTime);
        }
        else if (musicLoop != null)
        {
            sourceLoop.Play();
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (sourceIntro != null) sourceIntro.volume = volume;
        if (sourceLoop != null) sourceLoop.volume = volume;
#endif
    }
}