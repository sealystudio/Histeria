using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;


public class PickUpController : MonoBehaviour
{
    [SerializeField]
    public InventoryItem itemData; 
    public Canvas canvasTexto;
    public Canvas inventoryCanvas;
    
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Inventory inventory = player.GetComponent<Inventory>();
            if (inventory != null)
            {
                bool added = inventory.AddItem(itemData);
                if (added)
                {
                    // Actualizar la UI antes de destruir
                    InventoryUI inventoryUI = inventoryCanvas.GetComponent<InventoryUI>();
                    if (inventoryUI != null)
                        inventoryUI.RefreshUI();

                    // Mostrar texto
                    canvasTexto.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                        $"Se ha añadido {itemData.itemName} al inventario";

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
            canvasTexto.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Pulsa F para recoger {itemData.itemName}";
        }
    }

    // Detecta cuando el jugador se aleja del objeto
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            canvasTexto.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "";
        }
    }
}

