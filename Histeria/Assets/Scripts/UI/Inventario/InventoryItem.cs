using UnityEngine;
public enum ItemType
{
    Consumible,   // Corazones, comida, etc.
    Narrativo,        // Pistas, audios narrativos
    Equipable     // Linterna, armas, etc.
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventario/Item")]
public class InventoryItem : ScriptableObject
{
    [Header("Datos base")]
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    [TextArea] public string description;
    public bool isStackable = false;
    public int maxStack = 99;
   
    [Header("Datos específicos")]
    public ConsumableData consumableData;
    public StoryData storyData;
    public EquipableData equipableData;
}

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