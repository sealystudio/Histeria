using UnityEngine;

// Define los tipos de items que tu script Inventory.cs ya espera
public enum ItemType { Consumible, Narrativo, Equipable }

// --- 1. Definiciones internas que usa Inventory.cs ---
// Esto evita que Inventory.cs dé errores al acceder a item.consumableData.healAmount
[System.Serializable]
public class ConsumableData
{
    public int healAmount;
}

[System.Serializable]
public class StoryData
{
    public AudioClip narrationClip;
}

[System.Serializable]
public class EquipableData
{
    public GameObject equipPrefab;
}


// --- 2. ScriptableObject principal: InventoryItem ---
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    [Header("Información General")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    // Inventory.cs usa esto en el switch:
    public ItemType itemType;

    [Header("Datos Específicos del Tipo")]
    public ConsumableData consumableData;
    public StoryData storyData;
    public EquipableData equipableData;

    // -------------------------
    //  3. CAMBIO DE ESCENA
    // -------------------------
    // ESTA es la parte que te faltaba
    [Header("Cambio de escena")]
    public bool triggersLevelChange = false;
    public string nextSceneName = "";
}
