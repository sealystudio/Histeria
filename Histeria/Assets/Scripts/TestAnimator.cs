using UnityEngine;

public class TestAnimator : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        // Busca el animator automáticamente si no lo has asignado
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // PRUEBA DE ANDAR (Mantén pulsada la tecla W)
        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("IsWalking", true);
            Debug.Log("Activando Andar (IsWalking = true)");
        }
        else
        {
            animator.SetBool("IsWalking", false);
            // No ponemos log aquí para no llenar la consola
        }

        // PRUEBA DE ATAQUE (Pulsa la Barra Espaciadora)
        if (Input.GetKeyDown(KeyCode.G))
        {
            animator.SetTrigger("Attack");
            Debug.Log("Disparando Ataque (Trigger Attack)");
        }
    }
}