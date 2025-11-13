using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Configuración del inventario")]
    public int maxSlots = 1;
    public List<ItemSlot> items = new List<ItemSlot>();

    [Header("Referencias")]
    public Canvas inventoryCanvas;
    public Canvas hudCanvas;
    public static bool isInventoryOpen = false;

    private PlayerHealthHearts playerHealth;
    private PlayerEquipment playerEquipment;

    [System.Serializable]
    public class ItemSlot
    {
        public InventoryItem itemData;
        public int quantity;
    }

    void Start()
    {
        if (inventoryCanvas != null)
            inventoryCanvas.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Escape))
        {
            bool abrir = !inventoryCanvas.enabled;
            PlayerMovement pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            inventoryCanvas.enabled = abrir;
            isInventoryOpen = abrir;

            if (abrir)
            {
                Time.timeScale = 0f;
                Cursor.visible = true;
                inventoryCanvas.GetComponent<InventoryUI>().RefreshUI();
                pm.canPunch = false;
                hudCanvas.enabled = false;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.visible = false;
                pm.canPunch = true;
                hudCanvas.enabled = true;
                
            }
        }
    }

    public bool AddItem(InventoryItem newItem)
    {
        if (items.Count < maxSlots)
        {
            items.Add(new ItemSlot { itemData = newItem, quantity = 1 });
            Debug.Log($"[Inventory] Añadido item '{newItem.itemName}'. Total ahora: {items.Count}");
            return true;
        }

        Debug.Log("[Inventory] Inventario lleno");
        return false;
    }

    public void UseItemSlot(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning("[Inventory] Índice de slot inválido.");
            return;
        }

        ItemSlot slot = items[index];
        InventoryItem item = slot.itemData;
        Debug.Log($"[Inventory] >>> Usando '{item.itemName}' en índice {index}. Tipo: {item.itemType}");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[Inventory] No se encontró Player con tag 'Player'.");
            return;
        }

        playerHealth = player.GetComponent<PlayerHealthHearts>();
        playerEquipment = player.GetComponent<PlayerEquipment>();

        switch (item.itemType)
        {
            case ItemType.Consumible:
                if (item.consumableData != null && playerHealth != null)
                    playerHealth.Heal(item.consumableData.healAmount);
                break;

            case ItemType.Narrativo:
                if (item.storyData != null && item.storyData.narrationClip != null)
                    AudioSource.PlayClipAtPoint(item.storyData.narrationClip, player.transform.position);
                break;

            case ItemType.Equipable:
                if (item.equipableData != null && playerEquipment != null)
                    playerEquipment.Equip(item.equipableData.equipPrefab);
                break;
        }

        // Reducir cantidad o eliminar slot
        if (slot.quantity > 1)
            slot.quantity--;
        else
            items.RemoveAt(index);

        inventoryCanvas.GetComponent<InventoryUI>().RefreshUI();

        CloseInventory();
    }

    public void CloseInventory()
    {
        inventoryCanvas.enabled = false;
        hudCanvas.enabled = true;
        Time.timeScale = 1f;
        Cursor.visible = false;
        isInventoryOpen = false;
    }
}
