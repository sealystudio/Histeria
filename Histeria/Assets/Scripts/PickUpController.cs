using UnityEngine;


public class PickUpController : MonoBehaviour
{
    private bool isPlayerNearby = false;

    void Update()
    {
        // Si el jugador está cerca y pulsa F
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        Debug.Log("Objeto recogido: " + gameObject.name);

        // Aquí más adelante lo meterás en el inventario
        // Inventario.Instance.AgregarObjeto(this);

        Destroy(gameObject); // Elimina el objeto de la escena
    }

    // Detecta cuando el jugador entra en el área del objeto
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Pulsa F para recoger " + gameObject.name);
        }
    }

    // Detecta cuando el jugador se aleja del objeto
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}

