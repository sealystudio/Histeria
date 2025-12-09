using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Configuración del inventario")]
    public int maxSlots = 1;
    public List<ItemSlot> items = new List<ItemSlot>();

    [Header("Cambio de escena al usar")]
    public bool triggersLevelChange = false;
    public string nextSceneName = "";

    [Header("Referencias")]
    public Canvas inventoryCanvas;
    public Canvas hudCanvas;
    public static bool isInventoryOpen = false;
    public static bool canOpenInventory = true;

    private PlayerHealthHearts playerHealth;
    private PlayerEquipment playerEquipment;

    private AudioSource audioSource;

    [System.Serializable]
    public class ItemSlot
    {
        public InventoryItem itemData;
        public int quantity;
    }


    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // si no tiene audioSource, se lo añadimos
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        if (inventoryCanvas != null)
            inventoryCanvas.enabled = false;
    }

    void Update()
    {
        if (canOpenInventory)
        {
            bool inventoryKey = false;

#if UNITY_ANDROID || UNITY_IOS //|| UNITY_EDITOR
            // Leemos el botón del bridge móvil
            inventoryKey = MobileInputBridge.InventoryPressed;
#else
            // Leemos la tecla E del teclado
            inventoryKey = Input.GetKeyDown(KeyCode.E);
#endif

            if (inventoryKey)
            {
                // ¡IMPORTANTE! Apagamos el botón inmediatamente para que no se repita
                MobileInputBridge.InventoryPressed = false;

                bool abrir = !inventoryCanvas.enabled;

                // Buscamos al jugador (asegúrate de que tu Player tenga el tag "Player")
                PlayerMovement pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

                inventoryCanvas.enabled = abrir;
                isInventoryOpen = abrir;

                if (abrir)
                {
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    inventoryCanvas.GetComponent<InventoryUI>().RefreshUI();
                    if (pm != null) pm.canPunch = false; // Evitamos null check por seguridad
                    hudCanvas.enabled = false;
                }
                else
                {
                    Time.timeScale = 1f;
                    Cursor.visible = false;
                    if (pm != null) pm.canPunch = true;
                    hudCanvas.enabled = true;
                }
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
                {
                    if (NarrativeManager.instance != null)
                    {
                        NarrativeManager.instance.ReproducirNarrativa(item.storyData.narrationClip);
                    }
                    else
                    {
                        Debug.LogWarning("¡Falta el NarrativeManager en la escena!");
                    }
                }
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

        // Refrescar UI
        inventoryCanvas.GetComponent<InventoryUI>().RefreshUI();

        // NUEVO: cambiar de escena si el item lo requiere
        if (item.triggersLevelChange)
        {
            Debug.Log("[Inventory] El ítem usado activa cambio de escena → " + item.nextSceneName);

            // 🔓 DESBLOQUEAR NIVEL SEGÚN LA ESCENA QUE VAS A CARGAR
            if (item.nextSceneName == "SampleScene")      // Nivel 1
                PlayerPrefs.SetInt("Nivel1_Desbloqueado", 1);

            if (item.nextSceneName == "Nivel2")           // Nivel 2
                PlayerPrefs.SetInt("Nivel2_Desbloqueado", 1);

            if (item.nextSceneName == "Nivel3")           // Nivel 3
                PlayerPrefs.SetInt("Nivel3_Desbloqueado", 1);

            if (item.nextSceneName == "NivelFinal")       // Nivel Final
                PlayerPrefs.SetInt("NivelFinal_Desbloqueado", 1);

            // Guardar cambios
            PlayerPrefs.Save();

            // Cargar escena
            LevelManager.instance.LoadScene(item.nextSceneName);
        }

        // Cerrar inventario
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
