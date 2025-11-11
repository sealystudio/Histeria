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


    private SpriteRenderer spriteRenderer;
    private Color originalColor; 
    private Coroutine flickerCoroutine;

    void Awake()
    {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void TriggerHitEffect()
    {
        // si ya estábamos parpadeando paramos para empezar el nuevo
        
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
        }

        flickerCoroutine = StartCoroutine(FlickerRoutine());
    }
    private IEnumerator FlickerRoutine()
    {
        // repetir el numero de veces
        for (int i = 0; i < numberOfFlickers; i++)
        {
            // rojo
            spriteRenderer.color = hitColor;
            // esperar
            yield return new WaitForSeconds(flickerDuration);

            // devuelta el color
            spriteRenderer.color = originalColor;
            // esperar
            yield return new WaitForSeconds(flickerDuration);
        }
        // original ed vuelta
        spriteRenderer.color = originalColor;
        flickerCoroutine = null;
    }
}