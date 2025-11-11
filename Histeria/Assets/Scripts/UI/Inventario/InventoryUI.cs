using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory playerInventory;       // Inventario del jugador
    public Button[] slots;                  // Slots fijos como botones

    [Header("Detalle del item")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    private InventoryItem selectedItem = null;
    public Button useButton;

    void Start()
    {
        // Asignar los listeners a cada botón
        for (int i = 0; i < slots.Length; i++)
        {
            int index = i; // Captura la variable
            slots[i].onClick.AddListener(() => OnSlotClicked(index));
            slots[i].transform.SetAsLastSibling(); // Trae el botón al frente

        }

        if (useButton != null)
            useButton.onClick.AddListener(OnUseButtonClicked);

        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Image img = slots[i].GetComponent<Image>();

            if (i < playerInventory.items.Count)
            {
                Sprite itemSprite = playerInventory.items[i].itemData.icon;
                img.sprite = itemSprite;
                img.color = Color.white;
                img.enabled = true;
                img.preserveAspect = true;

                // Ajustar tamaño según lado más largo
                RectTransform rt = img.GetComponent<RectTransform>();
                float slotWidth = rt.rect.width;
                float slotHeight = rt.rect.height;

                if (itemSprite.rect.width > itemSprite.rect.height)
                {
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotWidth);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotWidth * itemSprite.rect.height / itemSprite.rect.width);
                }
                else
                {
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotHeight);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotHeight * itemSprite.rect.width / itemSprite.rect.height);
                }

                slots[i].interactable = true;
            }
            else
            {
                img.sprite = null;
                img.enabled = false;
                slots[i].interactable = false;
            }
        }
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    public void OnSlotClicked(int index)
    {
        if (index < playerInventory.items.Count)
        {
            var slot = playerInventory.items[index];
            selectedItem = slot.itemData;  // guardamos el objeto, no el índice
            itemNameText.text = slot.itemData.itemName;
            itemDescriptionText.text = slot.itemData.description;
        }
        else
        {
            selectedItem = null;
            itemNameText.text = "";
            itemDescriptionText.text = "";
        }
    }


    // 🔹 Al pulsar el botón "Usar"
    private void OnUseButtonClicked()
    {
        if (selectedItem != null)
        {
            playerInventory.UseItem(selectedItem);
            selectedItem = null;
        }
        else
        {
            Debug.LogWarning("No hay ningún objeto seleccionado para usar.");
        }
    }

    public void ClearSelection()
    {
        selectedItem = null;
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

}
