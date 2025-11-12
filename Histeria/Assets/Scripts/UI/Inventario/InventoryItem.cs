using UnityEngine;

// Define los tipos de items que tu script Inventory.cs ya espera
public enum ItemType { Consumible, Narrativo, Equipable }

// --- 1. Definiciones de los tipos de datos ---
// Estas son las clases que tu Inventory.cs está buscando.
// Puedes ponerlas en este mismo archivo, por encima de la clase InventoryItem.

[System.Serializable]
public class ConsumableData
{
    // Inventory.cs busca esto: item.consumableData.healAmount
    public int healAmount;
}

[System.Serializable]
public class StoryData
{
    // Inventory.cs busca esto: item.storyData.narrationClip
    public AudioClip narrationClip;
}

[System.Serializable]
public class EquipableData
{
    // Inventory.cs busca esto: item.equipableData.equipPrefab
    public GameObject equipPrefab;
}


// --- 2. Tu clase ScriptableObject principal ---

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    [Header("Info General")]
    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;

    // Inventory.cs usa esto en el 'switch'
    public ItemType itemType;

    // Estas son las variables que causan tus errores.
    // Al añadirlas aquí, Inventory.cs podrá encontrarlas.

    [Header("Datos Específicos del Tipo")]
    public ConsumableData consumableData;
    public StoryData storyData;
    public EquipableData equipableData;
}