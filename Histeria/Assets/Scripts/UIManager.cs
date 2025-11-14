using UnityEngine;
using TMPro; // Necesario para controlar el texto

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Referencias de la Escena")]
    [Tooltip("Arrastra aquí el objeto Canvas que muestra el texto 'Pulsa F'")]
    public Canvas canvasTexto;
    
    [Tooltip("Arrastra aquí el objeto Canvas de tu Inventario")]
    public Canvas inventoryCanvas;

    // --- Referencias privadas ---
    private TextMeshProUGUI pickUpTextComponent;
    private InventoryUI inventoryUIComponent;

    void Awake()
    {
        // Configurar el Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }

        // Obtener los componentes una sola vez para ser más eficientes
        if (canvasTexto != null)
        {
            pickUpTextComponent = canvasTexto.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (inventoryCanvas != null)
        {
            inventoryUIComponent = inventoryCanvas.GetComponent<InventoryUI>();
        }
    }

    // --- Funciones de Ayuda ---

    /// <summary>
    /// Muestra el texto de "recoger" en la UI.
    /// </summary>
    public void SetPickUpText(string text)
    {
        if (pickUpTextComponent != null)
        {
            pickUpTextComponent.text = text;
        }
    }

    /// <summary>
    /// Actualiza la UI del inventario.
    /// </summary>
    public void RefreshInventoryUI()
    {
        if (inventoryUIComponent != null)
        {
            inventoryUIComponent.RefreshUI();
        }
    }
}