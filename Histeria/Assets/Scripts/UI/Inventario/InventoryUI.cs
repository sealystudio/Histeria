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
    public Button useButton;

    private int selectedIndex = -1;
    void Update()
    {
        useButton.interactable = selectedIndex >= 0;
    }

    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int index = i;
            slots[i].onClick.RemoveAllListeners();
            slots[i].onClick.AddListener(() => OnSlotClicked(index));

            // Esto es lo clave:
            var btnNav = slots[i].navigation;
            btnNav.mode = Navigation.Mode.None;
            slots[i].navigation = btnNav;
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
                var slot = playerInventory.items[i];
                img.sprite = slot.itemData.icon;
                img.color = (i == selectedIndex) ? Color.yellow : Color.white; // resalta seleccionado
                img.enabled = true;
                img.preserveAspect = true;
                slots[i].interactable = true;
            }
            else
            {
                img.sprite = null;
                img.enabled = false;
                slots[i].interactable = false;
            }
        }

        // Si el índice seleccionado ya no existe
        if (selectedIndex >= playerInventory.items.Count)
        {
            selectedIndex = -1;
            ClearItemDetails();
        }
    }

    public void OnSlotClicked(int index)
    {
        if (index >= playerInventory.items.Count)
            return;

        selectedIndex = index;

        var slotData = playerInventory.items[index];
        itemNameText.text = slotData.itemData.itemName;
        itemDescriptionText.text = slotData.itemData.description;

        RefreshUI();
        Debug.Log($"[InventoryUI] Seleccionado '{slotData.itemData.itemName}' (índice {index})");
    }

    public void OnUseButtonClicked()
    {
        if (selectedIndex >= 0 && selectedIndex < playerInventory.items.Count)
        {
            playerInventory.UseItemSlot(selectedIndex);

            selectedIndex = -1;
            ClearItemDetails();
            RefreshUI();
        }
        else
        {
            Debug.LogWarning("No hay ningún objeto seleccionado para usar.");
        }
    }


    private void ClearItemDetails()
    {
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }
}
