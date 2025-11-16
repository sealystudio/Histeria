// (Probablemente puedas borrar esta línea, no parece necesaria)
// using UnityEditorInternal.Profiling.Memory.Experimental; 
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [SerializeField]
    public InventoryItem itemData;

    private UIManager uiManager;
    private bool isPlayerNearby = false;
    
    public AudioClip itemSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // si no tiene audioSource, se lo añadimos
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        // obtenemos la referencia al manager central
        uiManager = UIManager.instance;
        if(uiManager == null)
        {
            Debug.LogError("¡UIManager.instance no encontrado! Asegúrate de que el script UIManager está en un objeto en la escena.");
        }
    }

    void Update()
    {
        // si esta cerca y pulsa F:
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            PickUp();

        }
    }

    void PickUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Inventory inventory = player.GetComponent<Inventory>();
        if (inventory == null) return;

        bool added = inventory.AddItem(itemData);



        if (added)
        {
            // reproducir el sonido sin que se corte
            if (itemSound != null)
                AudioSource.PlayClipAtPoint(itemSound, transform.position, 0.8f);

            uiManager.RefreshInventoryUI();
            uiManager.SetPickUpText($"Se ha añadido {itemData.itemName} al inventario");

            Destroy(gameObject); // se destruye el ítem después de reproducir el sonido
        }
        else
        {
            Debug.Log("Inventario lleno, no se puede recoger.");
        }
    }


    // detecta cuando el jugador entra en el área del objeto
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Pulsa F para recoger " + gameObject.name);

            uiManager.SetPickUpText($"Pulsa F para recoger {itemData.itemName}");
        }
    }

    // cuando el jugador se aleja del objeto
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            uiManager.SetPickUpText("");
        }
    }
}