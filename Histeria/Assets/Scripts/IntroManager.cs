using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup logoEmpresa;    // CanvasGroup del logo de la empresa
    public CanvasGroup textoDialogo;   // CanvasGroup del texto
    public CanvasGroup logoHisteria;   // CanvasGroup del logo Histeria
    public CanvasGroup presents;   // CanvasGroup de PRESENTA
    public CanvasGroup continuar;   // CanvasGroup de CONTINUAR
    public CanvasGroup panelNegro;   // CanvasGroup del panel de fondo
    public CanvasGroup jugar;   // CanvasGroup del boton jugar
    public CanvasGroup creditos;   // CanvasGroup del boton creditos
    public CanvasGroup salir;   // CanvasGroup del boton salir
    public CanvasGroup logoPeque;   // CanvasGroup del logo de Sealy Studio
    [SerializeField] private GameObject botonContinuar;
    [SerializeField] private GameObject botonJugar;
    [SerializeField] private GameObject botonCreditos;
    [SerializeField] private GameObject botonSalir;


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

    [Header("Move")]
    [SerializeField] private RectTransform logoHist;
    [SerializeField] private Vector3 targetPosition; // posición final del logo
    [SerializeField] private float moveDuration = 2f; // duración del movimiento
    [SerializeField] private RectTransform logoFinalPos; // Este es el empty que marca la posición final

    void Start()
    {
        // Inicializamos UI invisible
        logoEmpresa.alpha = 0f;
        textoDialogo.alpha = 0f;
        logoHisteria.alpha = 0f;
        jugar.alpha = 0f;
        creditos.alpha = 0f;
        salir.alpha = 0f;
        logoPeque.alpha = 0f;

        if (botonContinuar != null)
            botonContinuar.SetActive(false);
        if (botonJugar != null)
            botonJugar.SetActive(false);
        if (botonCreditos != null)
            botonCreditos.SetActive(false);
        if (botonSalir != null)
            botonSalir.SetActive(false);

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

        yield return new WaitForSeconds(4f);
        StartCoroutine(FadeOutAudio(vozTexto, 2f));

        // 6️ Fade out del texto
        yield return StartCoroutine(FadeCanvasGroup(textoDialogo, 1f, 0f, fadeDuration));

        if (musicaFondo != null)
            musicaFondo.Play();

        // 8️ Fade in del logo Histeria
        yield return StartCoroutine(FadeCanvasGroup(logoHisteria, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(tiempoLogoHisteria);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(MoverLogo(logoHist, targetPosition, moveDuration));

        yield return new WaitForSeconds(1f);
        botonContinuar.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(continuar, 0f, 1f, fadeDuration));

        if (continuar != null)
        {
            continuar.alpha = 1f; // comenzamos totalmente visible
            StartCoroutine(ParpadeoBoton(continuar, 0.5f, 1f, 1f));
        }
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

    IEnumerator MoverLogo(RectTransform logo, Vector3 destino, float duracion)
    {
        Vector3 inicio = logo.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            logo.anchoredPosition = Vector3.Lerp(inicio, destino, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        logo.anchoredPosition = destino;
    }

    IEnumerator ParpadeoBoton(CanvasGroup cg, float minAlpha = 0.01f, float maxAlpha = 1f, float speed = 0.2f)
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * speed * 0.5f;
            cg.alpha = Mathf.Lerp(minAlpha * 0.5f, maxAlpha, (Mathf.Sin(t * Mathf.PI * 2) + 1f) / 2f);
            yield return null;
        }
    }

    public void IrAlMenu()
    {
        StartCoroutine(IrAlMenuCoroutine());
    }

    IEnumerator MoverYRedimensionarLogo(RectTransform logo, RectTransform destino, float duracion)
    {
        Vector3 startPos = logo.anchoredPosition;
        Vector2 startSize = logo.sizeDelta;

        Vector3 endPos = destino.anchoredPosition; // <-- Tomamos la posición del empty
        Vector2 endSize = destino.sizeDelta;       // <-- Tomamos el tamaño del empty

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            logo.anchoredPosition = Vector3.Lerp(startPos, endPos, smoothT);
            logo.sizeDelta = Vector2.Lerp(startSize, endSize, smoothT);
            yield return null;
        }

        logo.anchoredPosition = endPos;
        logo.sizeDelta = endSize;
    }

    private IEnumerator IrAlMenuCoroutine()
    {
        StartCoroutine(MoverYRedimensionarLogo(logoHist, logoFinalPos, 3f));
        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeCanvasGroup(continuar, 1f, 0f, 2f));
        StartCoroutine(FadeCanvasGroup(panelNegro, 1f, 0f, 2f));
        botonContinuar.SetActive(false);

        StartCoroutine(FadeCanvasGroup(jugar, 0f, 1f, 2f));
        StartCoroutine(FadeCanvasGroup(creditos, 0f, 1f, 2f));
        StartCoroutine(FadeCanvasGroup(salir, 0f, 1f, 2f));
        StartCoroutine(FadeCanvasGroup(logoPeque, 0f, 1f, 2f));

        if (botonJugar != null)
            botonJugar.SetActive(true);
        if (botonCreditos != null)
            botonCreditos.SetActive(true);
        if (botonSalir != null)
            botonSalir.SetActive(true);
    }
}
