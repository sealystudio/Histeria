using UnityEngine;
using TMPro; // Necesario para el texto del UI

public class Level3Timer : MonoBehaviour
{
    [Header("Configuración")]
    public float minutosTotales = 10f; // 10 minutos
    public int golpesTotales = 3;      // Se dividirá el tiempo entre 3

    [Header("UI (Opcional)")]
    public TextMeshProUGUI textoContador;

    // Referencias internas
    private PlayerHealthHearts saludEli;
    private float tiempoRestante;
    private float intervaloTiempo;
    private float siguienteGolpe;

    void Start()
    {
        // 1. BUSCAR A ELI AUTOMÁTICAMENTE
        // Esto evita que se pierda la referencia si Eli viene del Nivel 1
        saludEli = FindObjectOfType<PlayerHealthHearts>();

        if (saludEli == null)
        {
            Debug.LogError("¡ERROR! No encuentro el script PlayerHealthHearts en la escena.");
            return;
        }

        // 2. MATEMÁTICAS
        // 10 min * 60 = 600 segundos
        tiempoRestante = minutosTotales * 60f;

        // 600 / 3 = 200 segundos por golpe
        intervaloTiempo = tiempoRestante / golpesTotales;

        // El primer golpe será cuando falten 400 segundos (6:40)
        siguienteGolpe = tiempoRestante - intervaloTiempo;
    }

    void Update()
    {
        // Si ya se acabó el tiempo, no hacemos nada más
        if (tiempoRestante <= 0) return;

        tiempoRestante -= Time.deltaTime;

        // Actualizar el texto en pantalla (00:00)
        if (textoContador != null)
        {
            ActualizarRelojVisual();
        }

        // --- MOMENTO DEL DAÑO ---
        // Si el tiempo baja del hito marcado (ej: baja de 6:40, luego de 3:20, luego de 0:00)
        if (tiempoRestante <= siguienteGolpe)
        {
            HacerDaño();

            // Calculamos el siguiente hito restando otros 200 segundos
            siguienteGolpe -= intervaloTiempo;
        }
    }

    void HacerDaño()
    {
        if (saludEli != null)
        {
            Debug.Log($"[Timer Nivel 3] ¡TIEMPO! Golpeando a Eli. Tiempo restante: {tiempoRestante}");

            // Usamos tu método exacto
            saludEli.TakeDamage(1);
        }
    }

    void ActualizarRelojVisual()
    {
        // Evitar números negativos al final
        float tiempoParaMostrar = Mathf.Max(0, tiempoRestante);

        int minutos = Mathf.FloorToInt(tiempoParaMostrar / 60);
        int segundos = Mathf.FloorToInt(tiempoParaMostrar % 60);

        // Formato 00:00
        textoContador.text = string.Format("{0:00}:{1:00}", minutos, segundos);

        // (Opcional) Poner el texto rojo cuando queda poco
        if (tiempoRestante < 10f) textoContador.color = Color.red;
    }
}
