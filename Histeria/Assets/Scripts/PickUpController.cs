// (Probablemente puedas borrar esta línea, no parece necesaria)
// using UnityEditorInternal.Profiling.Memory.Experimental; 
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [SerializeField]
    public InventoryItem itemData;

    private UIManager uiManager;
    private bool isPlayerNearby = false;

    void Start()
    {
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
        if (player != null)
        {
            Inventory inventory = player.GetComponent<Inventory>();
            if (inventory != null)
            {
                bool added = inventory.AddItem(itemData);
                if (added)
                {
                    // actualiar UI manager
                    uiManager.RefreshInventoryUI();

                    // mostar texto
                    uiManager.SetPickUpText($"Se ha añadido {itemData.itemName} al inventario");

                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Inventario lleno, no se puede recoger.");
                }
            }
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