using UnityEngine;
using System.Collections;
using BehaviourAPI.Core;

public class EliCorrupta : EnemyBase
{
    [Header("Datos")]
    public EliCorruptaData data;
    private Transform eliNormal;   // referencia al jugador
    private PlayerMovement eliMovement; // referencia al script de movimiento del jugador
    private Rigidbody2D rb;
    public CrosshairController crosshair; // referencia al crosshair
    Animator anim; 


    bool canDash = true;
    bool isDashing = false;

    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;
    public float dashSpeed = 12f;

    [Header("Punch")]
    public int punchDamage = 1;
    public float punchRange = 1f;

    [Header("Ataque espejo")]
    private bool puedeDisparar;
    public float lagrimasCooldown = 0.5f;


    [Header("Area Attack")]
    public bool puedeArea = true;
    public bool isCharging = false;
    public float areaCooldown = 6f;

    public bool PuedeDispararDebug() => puedeDisparar;
    private LevelManager lm;
    private bool alreadyCounted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
        anim = GetComponent<Animator>();
        InitializeStats(data.maxHealth , 7f, this.GetComponent<Rigidbody2D>());
        puedeDisparar = true;
        moveSpeed = data.moveSpeed;
        damage = data.damage;
        detectionRange = data.detectionRange;
        attackRange = data.attackRange;
        // Buscar dinámicamente al jugador en la escena
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            eliNormal = player.transform;
            eliMovement = player.GetComponent<PlayerMovement>();
        }
    }

    void Update()
    {
        if (isDead || eliNormal == null) return;

        float distancia = DistanciaJugador();

        // Eli corrupta no se mueve hacia el jugador , solo cuando este cerca

        if (distancia <= 4.5f ) {
            // 🔹 Girar en el eje X según la posición del jugador
            if (eliNormal.position.x > transform.position.x)
            {
                // Eli está a la derecha → mirar a la derecha
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (eliNormal.position.x < transform.position.x)
            {
                // Eli está a la izquierda → mirar a la izquierda
                transform.localScale = new Vector3(-1, 1, 1);
            }


            if (eliMovement.IsMoving() && !isCharging)
            {

                this.rb.linearVelocity = -eliMovement.getMoveDirection().normalized * moveSpeed;

            }



        }
    }

    // --- Método público para disparo espejo ---
    public void DispararEspejo(Vector3 direccionOriginal) // HAY QUE QUITAR EL ARGUMENTO
    {
        if (!puedeDisparar || data.lagrimaPrefab == null || eliNormal == null || isCharging) return;

        //Vector3 direccionOriginal = (eliNormal.position - transform.position).normalized;

        // comprobar distancia al jugador
        float distancia = Vector2.Distance(transform.position, eliNormal.position);
        if (distancia > attackRange + 3) return; // demasiado lejos, no dispara

        Vector3 dirContraria = -direccionOriginal.normalized;

        GameObject tear = Instantiate(data.lagrimaPrefab, transform.position, Quaternion.identity);

        LagrimasAttack la = tear.GetComponent<LagrimasAttack>();
        if (la != null) la.Initialize(dirContraria, LagrimasAttack.Team.Corrupta);

        float angle = Mathf.Atan2(dirContraria.y, dirContraria.x) * Mathf.Rad2Deg;
        tear.transform.rotation = Quaternion.Euler(0, 0, angle) * data.lagrimaPrefab.transform.rotation;

        StartCoroutine(CooldownDisparo());
    }
    
    private float nextShootTime = 0f;

    public float PuedeDispararUS()
    {
        return Time.time >= nextShootTime ? 1f : 0f;
    }

    public void DispararEspejoUS()
    {
        if (Time.time < nextShootTime) return;

        Vector3 dir = (eliNormal.position - transform.position).normalized;
        Vector3 dirContraria = -dir;

        GameObject tear = Instantiate(data.lagrimaPrefab, transform.position, Quaternion.identity);

        LagrimasAttack la = tear.GetComponent<LagrimasAttack>();
        if (la != null)
            la.Initialize(dirContraria, LagrimasAttack.Team.Corrupta);

        nextShootTime = Time.time + lagrimasCooldown;
    }


    




  

    public void Punch()

    {
       

        Vector3 attackPos = transform.position;

        // Detecta todo lo que esté en el rango del puño
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, punchRange);


        eliNormal.parent.GetComponent<PlayerHealthHearts>()?.TakeDamage(punchDamage);
    }

      

            
        
    


    public void CargarAtaqueArea()
    {
        if (!puedeArea || isCharging)
            return;

        puedeArea = false;
        isCharging = true;

        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("LoadingArea");
    }

    public void FinishAreaAttack()
    {
        isCharging = false;
        StartCoroutine(AreaCooldown());
    }

    private IEnumerator AreaCooldown()
    {
        yield return new WaitForSeconds(areaCooldown);
        puedeArea = true;
    }


    public IEnumerator returnChargingCooldown()
    {
        
        yield return new WaitForSeconds(5f);
        isCharging = false;
    }

    private IEnumerator CooldownDisparo()
    {
        puedeDisparar = false;
        yield return new WaitForSeconds(lagrimasCooldown);
        puedeDisparar = true;
    }


    public void DamageArea()
    {
        if (eliNormal == null) return;

        
        float distancia = DistanciaJugador();
        if (distancia <= 4.5f)
        {
            PlayerHealthHearts ph = eliNormal.GetComponent<PlayerHealthHearts>();
            if (ph != null)
            {
                ph.TakeDamage();
            }
        }
    }

    protected override void OnHit()
    {
        if (animator != null)
            animator.SetTrigger("Hit");

        if (data != null && data.hitEffect != null)
            Instantiate(data.hitEffect, transform.position, Quaternion.identity);
    }


    // Para el sistema de utilidad , distancia respecto al jugador
    public float DistanciaJugador()
    {
        //if (eliNormal == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, eliNormal.position);    
    }


    public float NoEstaCargando()
    {
        return isCharging ? 0f : 1f;
    }


    public float PuedeArea()
    {
        return puedeArea ? 1f : 0f;
    }

    public void Idle()
    {
        rb.linearVelocity = Vector2.zero;
    }
    public float VidaActual()
    {
        return currentHealth;
    }

    public IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;

        anim.SetTrigger("Dash");


        rb.linearVelocity = -eliMovement.getMoveDirection() * dashSpeed;


        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    protected override void Die()
    {
        if (animator != null)
            animator.SetTrigger("Die");

        if (data != null && data.dieEffect != null)
            Instantiate(data.dieEffect, transform.position, Quaternion.identity);

        if (!alreadyCounted)
        {
            if (LevelManager.instance != null)
                LevelManager.instance.EnemyMuerto();

            if (DungeonPopulator.instance != null)
                DungeonPopulator.instance.RestarEnemigo();

            alreadyCounted = true;
        }

        base.Die();
    }

}
