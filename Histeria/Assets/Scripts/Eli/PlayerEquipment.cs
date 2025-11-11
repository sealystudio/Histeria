using Unity.VisualScripting;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Header("Referencia al jugador")]
    public SpriteRenderer playerRenderer; // arrastra aquí el SpriteRenderer del jugador
    public Transform transformLinterna;
    private GameObject currentEquip;
    private bool lastFlipX;

    public void Equip(GameObject equipPrefab)
    {
        if (currentEquip != null)
            Destroy(currentEquip);

        
        currentEquip = Instantiate(equipPrefab, transformLinterna);

        // Guarda el estado actual del flip
        lastFlipX = playerRenderer.flipX;

        // Ajusta la orientación inicial
        UpdateFlip();
    }

    void Update()
    {
        if (currentEquip == null || playerRenderer == null)
            return;

        // Si cambia el flipX del jugador → actualiza
        if (playerRenderer.flipX != lastFlipX)
        {
            lastFlipX = playerRenderer.flipX;
            UpdateFlip();
        }
    }

    private void UpdateFlip()
    {
        // Cambiamos el flip aplicando una escala negativa en X
        Vector3 scale = currentEquip.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (playerRenderer.flipX ? -1 : 1);
        currentEquip.transform.localScale = scale;
    }
}
