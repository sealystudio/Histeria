using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;


public struct LightTarget
{
    public Light2D light2D;
    public Vector2 position;


    public LightTarget(Light2D light2D, Vector2 position)
    {
        this.light2D = light2D;
        this.position = position;
    }

}


public class SombraAbandono : EnemyBase
{
    public SombraAbandonoData data;



    Vector2 direccionHuida;

    LightTarget? target;
    private bool _hasTarget;

    public Light2D globalTargetLight;
    public Vector2 globalLightPos;

    private GameObject player;
    private PlayerAttack playerAttack;

    private GameObject[] lights;

    bool _playerFlashlight;
    float detectionRange = 1f;

    public static List<SombraAbandono> todasLasSombras = new List<SombraAbandono>();

    private bool _estoyHuyendo;

    private bool isIdle = false;
    private Vector3 idleStartPos;
    private float floatAmplitude = 0.2f; // altura del flotado
    private float floatFrequency = 1f;   // velocidad del flotado

    private void Start()
    {
        if (data != null)
            InitializeStats(data.maxHealth);

        player = GameObject.FindGameObjectWithTag("player");

        playerAttack = player.GetComponent<PlayerAttack>(); 

        lights = GameObject.FindGameObjectsWithTag("Light");
    }

    private void Update()
    {
        if (data == null || isDead) return; // no flotamos si está muerto

        // Flotado si está en idle
        if (isIdle)
        {
            float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = idleStartPos + new Vector3(0f, yOffset, 0f);
        }
        else if (direccionHuida != Vector2.zero)
        {
            transform.Translate(direccionHuida * data.moveSpeed * Time.deltaTime, Space.World);
            //transform.Translate(Vector3.up * Mathf.Sin(Time.time * data.moveSpeed) * Time.deltaTime);
        }

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

    private void OnEnable()
    {
        playerAttack.OnFlashlightChanged += HandleFlashlightChanged;
    }

    private void OnDisable()
    {
        playerAttack.OnFlashlightChanged -= HandleFlashlightChanged;
    }

    private void HandleFlashlightChanged(bool hasFlashlight)
    {
        _playerFlashlight = hasFlashlight;

    }
    public bool PlayerHasFlashLight()
    {

        return _playerFlashlight;
    }

    public bool PlayerHasNoFlashLight()
    {

        return !_playerFlashlight;
    }

    public bool EstoyHuyendo()
    {

        return _estoyHuyendo;

    }

    public bool OtraSombraHuyendo(GameObject otraSombra)
    {
        SombraAbandono sombraHuye;
        if (otraSombra == null) 
            return false;

        if (otraSombra.GetComponent<SombraAbandono>() != null) { 
            
            return false;

        }
        else
        {

          sombraHuye = otraSombra.GetComponentInParent<SombraAbandono>();

        }


        if(Vector2.Distance(transform.position , otraSombra.transform.position) < detectionRange) {


         return sombraHuye.EstoyHuyendo();

        }
        else
        {

            return false;
        }


        

    }

    public bool JugadorCerca()
    {
        return Vector2.Distance(transform.position, player.transform.position) < detectionRange;
    }

    public bool JugadorNoCerca()
    {
        return !(Vector2.Distance(transform.position, player.transform.position) < detectionRange);
    }



    public bool TengoLuz()
    {

        return _hasTarget;

    }
    public bool HaLlegadoLuz(LightTarget? destino)
    {

        return Vector2.Distance(transform.position, destino.Value.position) < 0.2f;
    }
    LightTarget? SeekLight()
    {
        
        Light2D closestLight = null;
        Vector2 closestPos = Vector2.zero;
        float minDist = 0;

        foreach (GameObject obj in lights)
        {
            Light2D l = obj.GetComponent<Light2D>();
            if (l != null  && l.intensity > 0f)
            {
                float dist = Vector2.Distance(transform.position, obj.transform.position);
                if (dist < minDist && dist <= detectionRange)
                {
                    minDist = dist;
                    closestLight = l;
                    closestPos = obj.transform.position;
                }
            }
        }

        if (closestLight != null) 
            return new LightTarget(closestLight, closestPos);

        return null;
        
    }


    public void MoveToLight()
    {

        target = SeekLight();

        if (!target.HasValue)
        return;

       
        globalTargetLight = target.Value.light2D;
        globalLightPos = target.Value.position;

        data.moveSpeed = 2.5f;

        Vector2 dir = (Vector2)transform.position - globalLightPos;
        direccionHuida = dir;
    }

    public void SwitchOffLight()
    {

        if (!target.HasValue)
            return;


        LightTarget light = target.Value;


        light.light2D.intensity = 0f;

    }

    public void Huir()
    {

         data.moveSpeed = 2.5f;

        Vector2 dir = transform.position - player.transform.position ;
        direccionHuida = dir;
    }

    public void HuirSombra(GameObject other)
    {
        data.moveSpeed = 5f;

        
        bool above = transform.position.y > other.transform.position.y;

        
        Vector2 perp = new Vector2(-direccionHuida.y, direccionHuida.x);

       
        if (!above)
            perp = -perp;

        
        float t = Random.Range(0.1f, 0.9f);
        Vector2 newDir = Vector2.Lerp(direccionHuida.normalized, perp.normalized, t).normalized;

        direccionHuida = newDir;
    }


    public void PerseguirJugador()
    {

        data.moveSpeed = 5f;

        Vector2 dir =  player.transform.position - transform.position ;

        direccionHuida = dir;

    }


    public void Idle()
    {
        data.moveSpeed = 0;  // Se para la sombra
        direccionHuida = Vector2.zero;

        // Activamos el flotado
        if (!isIdle)
        {
            idleStartPos = transform.position; // guardamos posición base
            isIdle = true;
        }

        if (animator != null)
            animator.SetTrigger("Idle"); // si quieres usar animación de sprite estático
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
        if (animator != null)
            animator.SetTrigger("Die"); // dispara la animación de muerte

        if (data != null && data.dieEffect != null)
            Instantiate(data.dieEffect, transform.position, Quaternion.identity);

        base.Die();
    }
}
