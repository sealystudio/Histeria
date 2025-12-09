using UnityEngine;

public class MostrarCartel : MonoBehaviour
{
    [Header("Referencias")]
    public Canvas cartelCanvas1;
    public Canvas cartelCanvas2;
    public Canvas cartelCanvas3;
    public Canvas cartelCanvas4;
    public Canvas cartelCanvas5;

    [Header("Selecciona qué cartel mostrar")]
    public int canvasSelected = 1; // 1,2,3 o 4

    private bool isPlayerNearby = false;



    void Start()
    {
        if (cartelCanvas1 != null) cartelCanvas1.enabled = false;
        if (cartelCanvas2 != null) cartelCanvas2.enabled = false;
        if (cartelCanvas3 != null) cartelCanvas3.enabled = false;
        if (cartelCanvas4 != null) cartelCanvas4.enabled = false;
        if (cartelCanvas5 != null) cartelCanvas5.enabled = false;
    }

    void OcultarTodos()
    {
        if (cartelCanvas1 != null) cartelCanvas1.enabled = false;
        if (cartelCanvas2 != null) cartelCanvas2.enabled = false;
        if (cartelCanvas3 != null) cartelCanvas3.enabled = false;
        if (cartelCanvas4 != null) cartelCanvas4.enabled = false;
        if (cartelCanvas5 != null) cartelCanvas5.enabled = false;
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
            case 1: if (cartelCanvas1 != null) cartelCanvas1.enabled = true; break;
            case 2: if (cartelCanvas2 != null) cartelCanvas2.enabled = true; break;
            case 3: if (cartelCanvas3 != null) cartelCanvas3.enabled = true; break;
            case 4: if (cartelCanvas4 != null) cartelCanvas4.enabled = true; break;
            case 5: if (cartelCanvas5 != null) cartelCanvas5.enabled = true; break;
            default:
                Debug.LogWarning("canvasSelected no válido (usa 1–5).");
                break;
        }
    }
    
}
