using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    [Header("Configuración del inventario")]
    public int maxSlots = 1;
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
            bool abrir = !inventoryCanvas.enabled;
            inventoryCanvas.enabled = abrir;
            isInventoryOpen = abrir; // <-- Indica si está abierto

            if (abrir)
            {
                Time.timeScale = 0f;
                Cursor.visible = true;
                inventoryCanvas.GetComponent<InventoryUI>().RefreshUI();
                hudCanvas.enabled = false;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.visible = false;
                hudCanvas.enabled = true;
            }
        }
    }


    public bool AddItem(InventoryItem newItem)
    {
        // Buscar un slot libre en la lista de items
        for (int i = 0; i < maxSlots; i++)
        {
            if (i >= items.Count) // slot libre
            {
                items.Add(new Inventory.ItemSlot { itemData = newItem, quantity = 1 });
                return true;
            }
        }

        Debug.Log("Inventario lleno");
        return false;
    }


    public void RemoveItem(InventoryItem itemToRemove)
    {
        var slot = items.Find(i => i.itemData == itemToRemove);
        if (slot != null)
        {
            slot.quantity--;
            if (slot.quantity <= 0)
                items.Remove(slot);
        }
    }

    public void UseItemAt(int index)
    {
        if (index < 0 || index >= items.Count) return;

        InventoryItem item = items[index].itemData;
        if (item == null)
        {
            Debug.LogWarning("El slot está vacío.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No se encontró el jugador para aplicar el objeto.");
            return;
        }

        PlayerHealthHearts playerHearts = player.GetComponent<PlayerHealthHearts>();
        PlayerEquipment playerEquipment = player.GetComponent<PlayerEquipment>();

        switch (item.itemType)
        {
            case ItemType.Consumible:
                if (item.consumableData != null && playerHearts != null)
                {
                    playerHearts.Heal(item.consumableData.healAmount);
                    RemoveItem(item);
                }
                else
                {
                    Debug.LogWarning("No se pudo usar el objeto consumible, falta referencia.");
                }
                break;

            case ItemType.Narrativo:
                if (item.storyData != null && item.storyData.narrationClip != null)
                {
                    AudioSource.PlayClipAtPoint(item.storyData.narrationClip, player.transform.position);
                    RemoveItem(item);
                }
                else
                {
                    Debug.LogWarning("No se pudo reproducir el objeto narrativo, falta clip o datos.");
                }
                break;

            case ItemType.Equipable:
                if (item.equipableData != null && playerEquipment != null)
                {
                    playerEquipment.Equip(item.equipableData.equipPrefab);
                }
                else
                {
                    Debug.LogWarning("No se pudo equipar el objeto, falta referencia.");
                }
                break;
        }

        // Refrescar UI
        if (inventoryCanvas != null)
        {
            InventoryUI ui = inventoryCanvas.GetComponent<InventoryUI>();
            if (ui != null) ui.RefreshUI();
        }
    }

}
