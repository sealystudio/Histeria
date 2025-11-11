// Pon este script en el Prefab "LinternaEquipable"
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Referencias de Sprites")]
    public SpriteRenderer spriteRendererOff;
    public SpriteRenderer spriteRendererOn; 

    [Header("Configuración de Ataque")]
    public float attackRange = 8f;
    public float attackWidth = 1.5f;
    public int attackDamage = 1;

    private CrosshairController crosshair;

    public void Initialize(CrosshairController targetCrosshair)
    {
        crosshair = targetCrosshair;
    }

    void Start()
    {
        //luz apagada por defecto
        if (spriteRendererOn) spriteRendererOn.enabled = false;
        if (spriteRendererOff) spriteRendererOff.enabled = true;
    }

    void Update()
    {
        
        if (crosshair == null) return;

        //rotacion segun cruceta
        Vector3 target = crosshair.transform.position;
        target.z = 0f;
        Vector3 dir = (target - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //lo rotamos
        transform.rotation = Quaternion.Euler(0, 0, angle);


        //ataque con boton derecho
        if (Input.GetMouseButton(1))
        {
            // cambiar a luz encendida
            if (spriteRendererOn) spriteRendererOn.enabled = true;
            if (spriteRendererOff) spriteRendererOff.enabled = false;

            //ataca
            PerformLightAttack(dir);
        }
        else
        {
            //mostar luz apagada
            if (spriteRendererOn) spriteRendererOn.enabled = false;
            if (spriteRendererOff) spriteRendererOff.enabled = true;
        }
    }

    // rayo para detectar hit
    void PerformLightAttack(Vector3 direction)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position, // Origen
            attackWidth,        // Ancho del rayo
            direction,          // Dirección
            attackRange         // Distancia
        );

        foreach (var hit in hits)
        {
            SombraAbandono sombra = hit.collider.GetComponent<SombraAbandono>();
            if (sombra != null)
            {
                sombra.TakeDamageFromLight(attackDamage);
            }
        }
    }
}