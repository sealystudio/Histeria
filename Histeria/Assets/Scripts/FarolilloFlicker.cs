using UnityEngine;
// ¡Importante! Necesitas esta línea para poder controlar las luces 2D (que son parte de URP)
using UnityEngine.Rendering.Universal;

// Esto es una buena práctica: asegura que el script SÓLO se pueda añadir
// a un GameObject que ya tenga un componente Light2D.
[RequireComponent(typeof(Light2D))]
public class FarolilloFlicker : MonoBehaviour
{
    [Header("Configuración del Parpadeo")]

    [Tooltip("La intensidad MÍNIMA a la que bajará la luz.")]
    public float minIntensity = 0.47f;

    [Tooltip("La intensidad MÁXIMA a la que subirá la luz.")]
    public float maxIntensity = 1.9f;

    [Tooltip("La VELOCIDAD del parpadeo. Un valor más alto significa un parpadeo más rápido.")]
    [Range(0.1f, 10f)]
    public float flickerSpeed = 1.0f;

    // --- Variables Privadas ---
    private Light2D myLight;    // Referencia a nuestro componente de luz
    private float perlinOffset; // Un "desfase" aleatorio para el ruido

    void Start()
    {
        // 1. Obtener el componente Light2D de este GameObject
        myLight = GetComponent<Light2D>();

        // 2. Asignar un valor aleatorio único a este farolillo
        // Esto es MUY importante: evita que todos los farolillos
        // parpadeen exactamente al mismo tiempo (sincronizados).
        perlinOffset = Random.Range(0f, 1000f);
    }

    void Update()
    {
        // 1. Calcular el valor de ruido "suave"
        // Usamos el tiempo del juego (Time.time) multiplicado por la velocidad,
        // y le sumamos nuestro desfase aleatorio para que sea único.
        // Mathf.PerlinNoise nos da un valor "ondulante" que siempre está entre 0.0 y 1.0
        float perlinValue = Mathf.PerlinNoise(
            (Time.time * flickerSpeed) + perlinOffset,
            0f // El segundo parámetro es 'y', podemos dejarlo en 0 para un ruido 1D
        );

        // 2. Mapear ese valor (0.0 a 1.0) a nuestro rango deseado (min a max)
        // Mathf.Lerp (Interpolación Lineal) es perfecto para esto.
        // Cuando perlinValue es 0, devuelve minIntensity.
        // Cuando perlinValue es 1, devuelve maxIntensity.
        float newIntensity = Mathf.Lerp(minIntensity, maxIntensity, perlinValue);

        // 3. Aplicar la nueva intensidad a la luz
        myLight.intensity = newIntensity;
    }
}