using UnityEngine;

public class MostrarCartel : MonoBehaviour
{
    [Header("Referencias")]
    public Canvas cartelCanvas1;
    public Canvas cartelCanvas2;
    public Canvas cartelCanvas3;
    public Canvas cartelCanvas4;

    [Header("Selecciona qué cartel mostrar")]
    public int canvasSelected = 1; // 1,2,3 o 4

    private bool isPlayerNearby = false;

    void Start()
    {
        cartelCanvas1.enabled = false;
        cartelCanvas2.enabled = false;
        cartelCanvas3.enabled = false;
        cartelCanvas4.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            MostrarCanvas();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            OcultarTodos();
        }
    }

    void MostrarCanvas()
    {
        OcultarTodos();

        switch (canvasSelected)
        {
            case 1: cartelCanvas1.enabled = true; break;
            case 2: cartelCanvas2.enabled = true; break;
            case 3: cartelCanvas3.enabled = true; break;
            case 4: cartelCanvas4.enabled = true; break;

            default:
                Debug.LogWarning("canvasSelected no válido (usa 1–4).");
                break;
        }
    }

    void OcultarTodos()
    {
        cartelCanvas1.enabled = false;
        cartelCanvas2.enabled = false;
        cartelCanvas3.enabled = false;
        cartelCanvas4.enabled = false;
    }
}
