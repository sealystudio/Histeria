using UnityEngine;

public class SombraAbandono : EnemyBase
{
    public SombraAbandonoData data;

    private void Start()
    {
        if (data != null)
            InitializeStats(data.maxHealth);

        
    }

    private void Update()
    {
        if (data == null) return;
        transform.Translate(Vector3.up * Mathf.Sin(Time.time * data.moveSpeed) * Time.deltaTime);
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
