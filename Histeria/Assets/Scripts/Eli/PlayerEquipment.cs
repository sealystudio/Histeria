using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Header("Referencia al jugador")]
    public SpriteRenderer playerRenderer; // arrastra aquí el SpriteRenderer del jugador
    public Transform transformLinterna; // punto donde se instancia la linterna

    private GameObject currentEquip;
    private bool lastFlipX;

    // 🔹 AÑADIDO: Referencia a PlayerAttack para coger el crosshair
    private PlayerAttack playerAttack;

    // 🔹 AÑADIDO: Awake para obtener la referencia al script de ataque
    void Awake()
    {
        // PlayerAttack debe estar en el mismo GameObject que PlayerEquipment
        playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack == null)
        {
            Debug.LogError("PlayerEquipment no pudo encontrar el script PlayerAttack en el jugador.");
        }
    }

    public void Equip(GameObject equipPrefab)
    {
        if (currentEquip != null)
            Destroy(currentEquip);

        // Instanciamos la linterna en el punto de anclaje (transformLinterna)
        currentEquip = Instantiate(equipPrefab, transformLinterna);

        // 🔹 AÑADIDO: Lógica para inicializar la linterna
        // Buscamos el script FlashlightController en el prefab que acabamos de instanciar
        FlashlightController flashlight = currentEquip.GetComponent<FlashlightController>();
        if (flashlight != null)
        {
            // Si lo encontramos, y tenemos la referencia de PlayerAttack y su cruceta...
            if (playerAttack != null && playerAttack.crosshair != null)
            {
                // ...le pasamos la cruceta a la linterna para que pueda apuntar.
                flashlight.Initialize(playerAttack.crosshair);
            }
            else
            {
                Debug.LogError("PlayerEquipment no pudo encontrar el crosshair (en PlayerAttack) para inicializar la linterna.");
            }
        }

        // Esta es la lógica de tu compañero, la mantenemos
        lastFlipX = playerRenderer.flipX;
        UpdateFlip();
    }

    // Esta es la lógica de tu compañero, la mantenemos
    void Update()
    {
        if (currentEquip == null || playerRenderer == null)
            return;

        if (playerRenderer.flipX != lastFlipX)
        {
            lastFlipX = playerRenderer.flipX;
            UpdateFlip();
        }
    }

    // Esta es la lógica de tu compañero, la mantenemos
    private void UpdateFlip()
    {
        Vector3 scale = currentEquip.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (playerRenderer.flipX ? -1 : 1);
        currentEquip.transform.localScale = scale;
    }
}