// (Probablemente puedas borrar esta línea, no parece necesaria)
// using UnityEditorInternal.Profiling.Memory.Experimental; 
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [SerializeField]
    public InventoryItem itemData;
    
    // --- VARIABLES ELIMINADAS ---
    // public Canvas canvasTexto;
    // public Canvas inventoryCanvas;
    
    // --- NUEVAS VARIABLES ---
    private UIManager uiManager;
    private bool isPlayerNearby = false;

    // --- NUEVA FUNCIÓN Start() ---
    void Start()
    {
        // Obtenemos la referencia al manager central
        uiManager = UIManager.instance;
        if(uiManager == null)
        {
            Debug.LogError("¡UIManager.instance no encontrado! Asegúrate de que el script UIManager está en un objeto en la escena.");
        }
    }

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Inventory inventory = player.GetComponent<Inventory>();
            if (inventory != null)
            {
                bool added = inventory.AddItem(itemData);
                if (added)
                {
                    // --- MODIFICADO ---
                    // Actualizar la UI usando el manager
                    uiManager.RefreshInventoryUI();

                    // Mostrar texto usando el manager
                    uiManager.SetPickUpText($"Se ha añadido {itemData.itemName} al inventario");

                    // Ahora sí se destruye el objeto del mundo
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Inventario lleno, no se puede recoger.");
                }
            }
        }
    }


    // Detecta cuando el jugador entra en el área del objeto
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Pulsa F para recoger " + gameObject.name);
            
            // --- MODIFICADO ---
            uiManager.SetPickUpText($"Pulsa F para recoger {itemData.itemName}");
        }
    }

    // Detecta cuando el jugador se aleja del objeto
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            
            // --- MODIFICADO (Esta era la línea 75 que fallaba) ---
            uiManager.SetPickUpText("");
        }
    }
}