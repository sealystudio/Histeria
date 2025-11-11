using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{

    [Header("Configuración")]
    [Tooltip("Un objeto hijo vacío en el jugador que marca dónde debe aparecer el item equipado.")]
    public Transform equipMountPoint;
    private GameObject currentEquip;

    private PlayerAttack playerAttack;
    public bool IsEquipped { get; private set; }

    void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack == null)
        {
            Debug.LogError("PlayerEquipment no encontró el script PlayerAttack en el Jugador!");
        }
        playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack == null)
        {
            Debug.LogError("PlayerEquipment no encontró el script PlayerAttack en el Jugador!");
        }
    }

    //inventory llamará a esto cuando le demos a usar
    public void Equip(GameObject equipPrefab)
    {
        if (currentEquip != null)
            Destroy(currentEquip);

        Transform parentTransform = equipMountPoint != null ? equipMountPoint : transform;

        currentEquip = Instantiate(equipPrefab, parentTransform);

        currentEquip.transform.localPosition = Vector3.zero;

        Debug.Log("Equipado: " + equipPrefab.name);

        IsEquipped = true;

        FlashlightController flashlight = currentEquip.GetComponent<FlashlightController>();
        if (flashlight != null)
        {
            if (playerAttack != null && playerAttack.crosshair != null)
            {
                flashlight.Initialize(playerAttack.crosshair);
            }
            else
            {
                Debug.LogError("¡No se pudo inicializar la linterna! Falta PlayerAttack o su Crosshair.");
            }
        }
    }

    public void Unequip()
    {
        if (currentEquip != null)
            Destroy(currentEquip);

        IsEquipped = false;
        currentEquip = null;
    }
}