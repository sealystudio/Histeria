using UnityEngine;

public enum ItemType
{
    Consumible,
    Narrativo,
    Equipable
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
    public int healAmount;             // Para Consumibles
    public AudioClip narrationClip;    // Para Narrativos
    public GameObject equipPrefab;     // Para Equipables
}
