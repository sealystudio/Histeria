using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class SombraAbandono : EnemyBase
{
    public SombraAbandonoData data;



    Vector2 direccionHuida;





    private void Start()
    {
        if (data != null)
            InitializeStats(data.maxHealth);

        
    }

    private void Update()
    {
        if (data == null) return;


        if (direccionHuida == Vector2.zero)
            return;

        transform.Translate(direccionHuida * data.moveSpeed * Time.deltaTime, Space.World);

        //transform.Translate(Vector3.up * Mathf.Sin(Time.time * data.moveSpeed) * Time.deltaTime);
    }

    //solo se le puede matar con la linterna, por eso dara igual que le pegues

    public void TakeDamageFromLight(int amount)
    {
        TakeDamage(amount, Vector2.zero);

        if (data != null && data.hitEffect != null)
        {
            GameObject effect = Instantiate(data.hitEffect, transform.position, Quaternion.identity);
            effect.transform.position = transform.position; // fuerza que esté en el enemigo
        }

    }

    //Para FSM y BT



    public void Huir(GameObject player)
    {

         data.moveSpeed = 2.5f;

        Vector2 dir = player.transform.position - transform.position;
        direccionHuida = dir;
    }

    public void HuirSombra(GameObject other)
    {
        data.moveSpeed = 2.5f;

        
        bool above = transform.position.y > other.transform.position.y;

        
        Vector2 perp = new Vector2(-direccionHuida.y, direccionHuida.x);

       
        if (!above)
            perp = -perp;

        
        float t = Random.Range(0.1f, 0.9f);
        Vector2 newDir = Vector2.Lerp(direccionHuida.normalized, perp.normalized, t).normalized;

        direccionHuida = newDir;
    }


    public void PerseguirJugador(GameObject player)
    {

        data.moveSpeed = 2.5f;

        Vector2 dir = transform.position - player.transform.position ;

        direccionHuida = dir;

    }


    public void Idle()
    {

     data.moveSpeed = 0;  //Se para la sombra

        Vector2 dir = Vector2.zero;

        direccionHuida = dir;
    }

    protected override void OnHit()
    {
        if (animator != null)
            animator.SetTrigger("Hit");

        if (data != null && data.hitEffect != null)
            Instantiate(data.hitEffect, transform.position, Quaternion.identity);
    }

    protected override void Die()
    {
        if (data != null && data.dieEffect != null)
            Instantiate(data.dieEffect, transform.position, Quaternion.identity);

        base.Die();
    }
}
