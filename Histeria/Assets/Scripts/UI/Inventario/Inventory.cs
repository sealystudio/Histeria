using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    [Header("Configuración del inventario")]
    public int maxSlots = 10;
    public List<ItemSlot> items = new List<ItemSlot>();

    [Header("Referencias")]
    public Canvas inventoryCanvas; // Asigna el Canvas del inventario aquí
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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        bool abrir = !inventoryCanvas.enabled;
        inventoryCanvas.enabled = abrir;
        isInventoryOpen = abrir;

        if (abrir)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            hudCanvas.enabled = false;

            var ui = inventoryCanvas.GetComponent<InventoryUI>();
            if (ui != null) ui.RefreshUI();
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            hudCanvas.enabled = true;
        }
    }

    public bool AddItem(InventoryItem newItem)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventario lleno");
            return false;
        }

        items.Add(new ItemSlot { itemData = newItem, quantity = 1 });
        return true;

    }

    public void RemoveItem(InventoryItem itemToRemove)
    {
        if (itemToRemove == null) return;

        // Buscar el primer slot que tenga ese objeto
        var slot = items.Find(i => i.itemData == itemToRemove);
        if (slot == null)
        {
            Debug.LogWarning("El objeto no se encontró en el inventario.");
            return;
        }

        if (itemToRemove.isStackable)
        {
            slot.quantity--;
            if (slot.quantity <= 0)
                items.Remove(slot);
        }
        else
        {
            items.Remove(slot);
        }

        // Refrescar UI si existe
        if (inventoryCanvas != null)
        {
            InventoryUI ui = inventoryCanvas.GetComponent<InventoryUI>();
            if (ui != null) ui.RefreshUI();
        }

        Debug.Log($"Eliminado {itemToRemove.itemName} del inventario.");
    }


    public void UseItem(InventoryItem item)
    {
        if (item == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No se encontró el jugador para aplicar el objeto.");
            return;
        }

        playerHealth = player.GetComponent<PlayerHealthHearts>();
        playerEquipment = player.GetComponent<PlayerEquipment>();

        switch (item.itemType)
        {
            case ItemType.Consumible:
                if (playerHealth != null)
                {
                    playerHealth.Heal(item.healAmount);
                    RemoveItem(item);
                }
                break;

            case ItemType.Narrativo:
                if (item.narrationClip != null)
                {
                    AudioSource.PlayClipAtPoint(item.narrationClip, player.transform.position);
                    RemoveItem(item);
                }
                else
                {
                    Debug.LogWarning("No hay clip asignado en el ítem narrativo.");
                }
                break;

            case ItemType.Equipable:
                if (item.equipPrefab != null && playerEquipment != null)
                {
                    // 1. Simplemente le decimos a PlayerEquipment que equipe el prefab.
                    playerEquipment.Equip(item.equipPrefab);

                    // 2. Eliminamos el item del inventario.
                    RemoveItem(item);

                    // 🔹 (Hemos quitado la lógica de "pa.tieneLinterna = true", 
                    // ya no es necesaria)
                }
                else
                {
                    Debug.LogWarning("El ítem equipable no tiene prefab o no se encontró PlayerEquipment.");
                }
                break;
        }

        var ui = inventoryCanvas.GetComponent<InventoryUI>();
        if (ui != null) ui.RefreshUI();

        CloseInventory();
    }


    private void CloseInventory()
    {
        inventoryCanvas.enabled = false;
        hudCanvas.enabled = true;
        Time.timeScale = 1f;
        Cursor.visible = false;
        isInventoryOpen = false;


        var ui = inventoryCanvas.GetComponent<InventoryUI>();
        if (ui != null)
        {
            ui.ClearSelection(); // limpia la selección visual y la variable interna
        }
    }
    


}
