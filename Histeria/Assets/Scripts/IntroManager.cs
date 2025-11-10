using UnityEngine;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup logoEmpresa;    // CanvasGroup del logo de la empresa
    public CanvasGroup textoDialogo;   // CanvasGroup del texto
    public CanvasGroup logoHisteria;   // CanvasGroup del logo Histeria
    public CanvasGroup presents;   // CanvasGroup de PRESENTA

    [Header("Audio")]
    public AudioSource vozTexto;       // Voz en off del texto
    public AudioSource musicaFondo;    // Música final que se mantiene
    public AudioSource gasp;    // Música final que se mantiene
    public AudioSource glassBreaking;    // Música final que se mantiene
    public AudioSource heartbeat;    // Música final que se mantiene

    [Header("Timing")]
    public float fadeDuration = 1f;         // Duración de cada fade
    public float tiempoLogoEmpresa = 3f;    // Tiempo que el logo de empresa permanece visible
    public float delayMusicaInicial = 2f;   // Tiempo que la musica inicial sigue antes del fade
    public float tiempoTextoVisible = 2f;   // Tiempo que el texto permanece visible
    public float tiempoLogoHisteria = 2f;   // Tiempo que el logo de Histeria permanece visible

    void Start()
    {
        // Inicializamos UI invisible
        logoEmpresa.alpha = 0f;
        textoDialogo.alpha = 0f;
        logoHisteria.alpha = 0f;

        StartCoroutine(SecuenciaIntro());
    }

    IEnumerator SecuenciaIntro()
    {
        yield return new WaitForSeconds(2f);
        if (heartbeat != null)
            heartbeat.Play();

        // 1️ Fade in del logo de la empresa
        StartCoroutine(FadeCanvasGroup(logoEmpresa, 0f, 1f, fadeDuration));
        StartCoroutine(FadeCanvasGroup(presents, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(tiempoLogoEmpresa);

        // 2️ Fade out del logo de la empresa
        StartCoroutine(FadeCanvasGroup(logoEmpresa, 1f, 0f, fadeDuration));
        StartCoroutine(FadeCanvasGroup(presents, 1f, 0f, fadeDuration));

        if (gasp  != null)
            gasp.Play();

        yield return new WaitForSeconds(1f);
        if (heartbeat != null)
            yield return StartCoroutine(FadeOutAudio(heartbeat, 1f));

        yield return new WaitForSeconds(1f);
        if (glassBreaking != null)
            glassBreaking.Play();

        yield return new WaitForSeconds(3f);

        // 5️ Fade in del texto + reproducir voz
        yield return StartCoroutine(FadeCanvasGroup(textoDialogo, 0f, 1f, fadeDuration));
        if (vozTexto != null)
            vozTexto.Play();

        yield return new WaitForSeconds(tiempoTextoVisible);

        // 6️ Fade out del texto
        yield return StartCoroutine(FadeCanvasGroup(textoDialogo, 1f, 0f, fadeDuration));

        if (musicaFondo != null)
            musicaFondo.Play();
        // 7️ Música de fondo vuelve a sonar
        //yield return StartCoroutine(FadeInAudio(musicaFondo, fadeDuration));

        // 8️ Fade in del logo Histeria
        yield return StartCoroutine(FadeCanvasGroup(logoHisteria, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(tiempoLogoHisteria);

        // 10️ Aquí podrías activar pasillo y jugador
        // pasillo.SetActive(true);
        // player.SetActive(true);
    }

    // Función genérica para fade de UI
    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }

    // Función para fade out de audio
    IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0f;
    }

    // Función para fade in de audio
    IEnumerator FadeInAudio(AudioSource audioSource, float duration)
    {
        audioSource.volume = 0f;
        audioSource.Play();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(0f, 0.3f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0.3f;
    }

}
