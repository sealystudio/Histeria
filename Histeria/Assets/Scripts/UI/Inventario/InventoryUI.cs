using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory playerInventory;
    public Button[] slots; 

    [Header("Detalle del item")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    private int selectedIndex = -1;
    public Button useButton;

    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int index = i;
            slots[i].onClick.AddListener(() => OnSlotClicked(index));
            slots[i].transform.SetAsLastSibling();

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
    }

    public void OnSlotClicked(int index)
    {
        if (index < playerInventory.items.Count)
        {
            selectedIndex = index;
            var slot = playerInventory.items[index];
            itemNameText.text = slot.itemData.itemName;
            itemDescriptionText.text = slot.itemData.description;
        }
    }

    private void OnUseButtonClicked()
    {
        if (selectedIndex >= 0)
        {
            playerInventory.UseItemAt(selectedIndex);
            selectedIndex = -1; // opcional: limpiar selección después
        }
        else
        {
            Debug.LogWarning("No hay ningún objeto seleccionado para usar.");
        }
    }
}