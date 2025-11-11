using System.Collections;
using UnityEngine;

// Asegura que este script esté en un objeto con un SpriteRenderer
[RequireComponent(typeof(SpriteRenderer))]
public class HitFlicker : MonoBehaviour
{
    [Header("Configuración del Efecto")]
    [Tooltip("El color que usará para teñir al ser golpeado.")]
    public Color hitColor = Color.red;

    [Tooltip("Número de veces que parpadeará.")]
    public int numberOfFlickers = 2;

    [Tooltip("Duración de cada parpadeo (ej: 0.1s rojo, 0.1s normal).")]
    public float flickerDuration = 0.1f;

    // --- Referencias Internas ---
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // Para guardar el color normal
    private Coroutine flickerCoroutine; // Para controlar si ya estamos parpadeando

    void Awake()
    {
        // 1. Obtener el SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 2. Guardar el color original (normalmente es 'white')
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// Llama a esta función desde otro script cuando el personaje recibe daño.
    /// </summary>
    public void TriggerHitEffect()
    {
        // Si ya estábamos parpadeando por un golpe anterior, detenemos esa
        // corutina para empezar la nueva. Evita errores visuales.
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
        }

        // Empezamos la nueva corutina de parpadeo
        flickerCoroutine = StartCoroutine(FlickerRoutine());
    }

    /// <summary>
    /// La Corutina que maneja el parpadeo en el tiempo.
    /// </summary>
    private IEnumerator FlickerRoutine()
    {
        // Repetir el número de veces que dijimos
        for (int i = 0; i < numberOfFlickers; i++)
        {
            // Poner color ROJO
            spriteRenderer.color = hitColor;
            // Esperar (ej: 0.1 segundos)
            yield return new WaitForSeconds(flickerDuration);

            // Poner color NORMAL
            spriteRenderer.color = originalColor;
            // Esperar (ej: 0.1 segundos)
            yield return new WaitForSeconds(flickerDuration);
        }

        // Al terminar el bucle, nos aseguramos de que el color
        // SIEMPRE vuelva a ser el original.
        spriteRenderer.color = originalColor;

        // Marcamos que la corutina ha terminado
        flickerCoroutine = null;
    }
}