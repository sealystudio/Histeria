
using System;
using UnityEngine;

using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    [Header("UI")]
    public CrosshairController crosshair; // referencia al crosshair


    [Header("Punch")]
    public int punchDamage = 1;
    public float punchRange = 1f;

    [Header("Lagrimas")]
    public GameObject lagrima;

    [Header("Tengo Linterna?")]
    public bool _hasFlashlight;
    public  event Action<bool> OnFlashlightChanged;

    public void SetFlashlight(bool value)
    {
        if (_hasFlashlight == value) return; // No notificar si no cambió
        _hasFlashlight = value;
        OnFlashlightChanged?.Invoke(_hasFlashlight); // Notificar a todos los enemigos
    }

    public bool PlayerHasFlashLight()
    {
        return _hasFlashlight;
    }


    public void Punch()
    {
        if (crosshair == null) return;

        Vector3 attackPos = transform.position;

        // Detecta todo lo que esté en el rango del puño
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, punchRange);

        foreach (var hit in hits)
        {
            // 1. Comprobación de Sombra (lo que ya tenías)
            SombraAbandono sombra = hit.GetComponent<SombraAbandono>();
            if (sombra != null)
            {
                // sombra.TakeDamageFromLight(1); 
            }

            // 2. NUEVO: Comprobación del Boss
            BossController boss = hit.GetComponent<BossController>();

            // Si el script está en el objeto padre pero golpeamos el collider de un hijo,
            // probamos a buscar en el padre por si acaso:
            if (boss == null) boss = hit.GetComponentInParent<BossController>();

            if (boss != null)
            {
                boss.TakeDamage(punchDamage); // Le restamos vida
                Debug.Log("¡PUM! Puñetazo al Boss. Vida restante: " + boss.currentHP);
            }

            MinionAI minion = hit.GetComponent<MinionAI>();
            if (minion != null)
            {
                minion.TakeDamage(punchDamage);
            }
        }
    }


    public void DispararLagrimas()
    {
        if (crosshair == null || lagrima == null) return;

        Vector3 target = crosshair.transform.position;
        target.z = 0f;
        Vector3 dir = (target - transform.position).normalized;



        // instanciamos la lÃ¡grima en la posiciÃ³n de Eli
        GameObject tear = Instantiate(lagrima, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity);

        Debug.Log("LÃ¡grima instanciada en " + transform.position);


        // inicializamos el script de la lÃ¡grima con la direcciÃ³n
        LagrimasAttack la = tear.GetComponent<LagrimasAttack>();
        if (la != null) la.Initialize(dir, LagrimasAttack.Team.Player);



        // opcional: rotar el sprite segÃºn la direcciÃ³n
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        tear.transform.rotation = Quaternion.Euler(0, 0, angle) * lagrima.transform.rotation;

        // Notificar a las Eli corruptas
        Debug.Log($"Notificando clones. Dir={dir}");
        EliCorrupta[] clones = UnityEngine.Object.FindObjectsByType<EliCorrupta>(FindObjectsSortMode.None);
        foreach (var clone in clones)
        {
            Debug.Log($"Clone: {clone.name} puedeDisparar={clone.PuedeDispararDebug()}");
            clone.DispararEspejo(dir);
        }


    }

    /*
    public void MegaPuÃ±o()
    {

    }
    */
}