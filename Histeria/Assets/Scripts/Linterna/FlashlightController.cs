
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Referencias de Sprites")]
    public SpriteRenderer spriteRendererOff; // Arrastra el sprite de linterna apagada (hijo)
    public SpriteRenderer spriteRendererOn;  // Arrastra el sprite de linterna CON LUZ (hijo)

    [Header("Configuración de Ataque")]
    public float attackRange = 8f;        // Qué tan lejos llega la luz
    public float attackWidth = 1.5f;      // Qué tan ancha es la luz
    public int attackDamage = 1;

    // Referencia interna a la cruceta (se la pasa PlayerEquipment)
    private CrosshairController crosshair;

    /// <summary>
    /// El script PlayerEquipment llamará a esto cuando se equipe.
    /// </summary>
    public void Initialize(CrosshairController targetCrosshair)
    {
        crosshair = targetCrosshair;
    }

    void Start()
    {
        // Asegurarse de empezar con la luz apagada
        if (spriteRendererOn) spriteRendererOn.enabled = false;
        if (spriteRendererOff) spriteRendererOff.enabled = true;
    }

    void Update()
    {
        // Si no ha sido inicializada (aún no tiene la cruceta), no hacer nada
        if (crosshair == null) return;

        // --- 1. Rotación ---
        Vector3 target = crosshair.transform.position;
        target.z = 0f;
        Vector3 dir = (target - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Rotamos este objeto (el padre "LinternaEquipable")
        transform.rotation = Quaternion.Euler(0, 0, angle);


        // --- 2. Input y Ataque (Botón derecho del mouse) ---
        if (Input.GetMouseButton(1))
        {
            // Muestra el sprite de LUZ ENCENDIDA
            if (spriteRendererOn) spriteRendererOn.enabled = true;
            if (spriteRendererOff) spriteRendererOff.enabled = false;

            // Ataca
            PerformLightAttack(dir);
        }
        else
        {
            // Muestra el sprite de LUZ APAGADA
            if (spriteRendererOn) spriteRendererOn.enabled = false;
            if (spriteRendererOff) spriteRendererOff.enabled = true;
        }
    }

    /// <summary>
    /// Lanza un rayo de luz (usando un CircleCast) y daña a los enemigos
    /// </summary>
    void PerformLightAttack(Vector3 direction)
    {
        // Usamos un CircleCast que es como un rayo "ancho"
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position, // Origen (el pivote de la linterna)
            attackWidth,        // Radio/ancho del rayo
            direction,          // Dirección en la que apunta
            attackRange         // Distancia
        );

        foreach (var hit in hits)
        {
            SombraAbandono sombra = hit.collider.GetComponent<SombraAbandono>();
            if (sombra != null)
            {
                // La linterna hace daño de luz
                sombra.TakeDamageFromLight(attackDamage);
            }
        }
    }
}