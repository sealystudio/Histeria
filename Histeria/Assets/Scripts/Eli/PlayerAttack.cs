using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("UI")]
    public CrosshairController crosshair; // referencia al crosshair
    

    [Header("Punch")]
    public int punchDamage = 1;
    public float punchRange = 1f;

    [Header("Linterna")]
    public bool tieneLinterna = false;
    public GameObject flashPrefab; // Prefab del triángulo
    public Transform linternaPoint; // punto de salida de la linterna



    [Header("Lagrimas")]
    public GameObject lagrima;



    public void Punch()
    {
        if (crosshair == null) return;

        Vector3 attackPos = transform.position;
       

        // Detecta enemigos cercanos en rango
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, punchRange);
        foreach (var hit in hits)
        {
            SombraAbandono sombra = hit.GetComponent<SombraAbandono>();
            if (sombra != null)
            {
                sombra.TakeDamageFromLight(1); // placeholder daño
            }
        }
    }

    public void DispararLinterna()
    {
        if (!tieneLinterna || flashPrefab == null || linternaPoint == null) return;

        // Punto fijo de salida del flash (por ejemplo la mano)
        Vector3 spawnPos = linternaPoint.position;

        // Rotación hacia el cursor
        Vector3 target = crosshair.transform.position;
        // Instanciamos el flash en ese punto fijo
        GameObject flash = Instantiate(flashPrefab, target.normalized, Quaternion.identity);

        Vector3 dir = (target - spawnPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        flash.transform.rotation = Quaternion.Euler(0, 0, angle+90);

        // Inicializar script del flash
        FlashLightAttack f = flash.GetComponent<FlashLightAttack>();
        if (f != null)
            f.Initialize();
    }




    public void DispararLagrimas()
    {
        if (crosshair == null || lagrima == null) return;

        Vector3 target = crosshair.transform.position;
        target.z = 0f;
        Vector3 dir = (target - transform.position).normalized;



        // instanciamos la lágrima en la posición de Eli
        GameObject tear = Instantiate(lagrima, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity);

        Debug.Log("Lágrima instanciada en " + transform.position);


        // inicializamos el script de la lágrima con la dirección
        LagrimasAttack la = tear.GetComponent<LagrimasAttack>();
        if (la != null)
            la.Initialize(dir);
        

        // opcional: rotar el sprite según la dirección
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        tear.transform.rotation = Quaternion.Euler(0, 0, angle) * lagrima.transform.rotation;

    }


    public void MegaPuño()
    {

    }

}

