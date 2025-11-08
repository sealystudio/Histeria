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

    public void Punch()
    {
        if (crosshair == null) return;

        Vector3 attackPos = crosshair.transform.position;

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
        {
            la.Initialize(dir);
        }

        // opcional: rotar el sprite según la dirección
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        tear.transform.rotation = Quaternion.Euler(0, 0, angle);
    }



    public void MegaPuño()
    {

    }

}

