// Pon este script en el Prefab "LinternaEquipable"
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    [Header("Referencias de Sprites")]
    public SpriteRenderer spriteRendererOff;
    private Light2D luzLinterna;

    [Header("Configuración de Ataque")]
    public float attackRange = 8f;
    public float attackWidth = 1.5f;
    public int attackDamage = 1;

    private CrosshairController crosshair;

    public AudioClip onSound;
    public AudioClip offSound;
    private AudioSource audioSource;
    private PlayerAttack pA;

    public void Initialize(CrosshairController targetCrosshair)
    {
        crosshair = targetCrosshair;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        luzLinterna = GetComponent<Light2D>();

        //  SOLUCIÓN 1: Buscar y asignar el componente PlayerAttack.
        // Asumiendo que este script (la linterna) es un hijo o un componente del Jugador,
        // o si está en el jugador, lo busca en sí mismo:
        pA = GetComponentInParent<PlayerAttack>();

        // Si la linterna NO es hijo del jugador, busca al jugador por Tag:
        if (pA == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                pA = playerObject.GetComponent<PlayerAttack>();
            }
        }

        if (pA == null)
        {
            Debug.LogError("PlayerAttack no encontrado. ¿Está el componente en el jugador y tiene la etiqueta 'Player'?");
            return; // Detener la ejecución si no se encuentra el PlayerAttack
        }


        //luz apagada por defecto
        luzLinterna.enabled = false;
        if (spriteRendererOff) spriteRendererOff.enabled = true;

        //  SOLUCIÓN 2: Usar el método SetFlashlight para activar la variable en PlayerAttack.
        // Esto activará el checkbox en el Inspector y notificará a los enemigos.
        pA.SetFlashlight(true);
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
            audioSource.PlayOneShot(onSound, 1f);
            luzLinterna.enabled = true;

            //ataca
            PerformLightAttack(dir);
        }
        else
        {
            audioSource.Stop();

            //mostar luz apagada
            audioSource.PlayOneShot(offSound, 1f);
            luzLinterna.enabled = false;

        }
    }

    // rayo para detectar hit
    void PerformLightAttack(Vector3 direction)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            luzLinterna.transform.position, // Origen
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