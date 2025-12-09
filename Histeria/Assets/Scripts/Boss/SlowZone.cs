using UnityEngine;

public class SlowZone : MonoBehaviour
{
    [Range(0.1f, 0.9f)] // Esto pone una barrita en el inspector para que no pongas 0 por error
    public float slowAmount = 0.5f;

    // CAMBIO IMPORTANTE: OnTriggerEnter2D (con el '2D' al final)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.moveSpeed *= slowAmount; // Reducimos la velocidad
                Debug.Log("Jugador ralentizado"); // Debug para comprobar
            }
        }
    }

    // CAMBIO IMPORTANTE: OnTriggerExit2D
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.moveSpeed /= slowAmount; // Restauramos la velocidad
                Debug.Log("Velocidad restaurada");
            }
        }
    }
}