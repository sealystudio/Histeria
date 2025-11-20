using System.Collections.Generic;
using System.ComponentModel;
using BehaviourAPI.Core;
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

    public Transform other;
    public Transform otraSombra;

    Vector2 direccionHuida;

    LightTarget target;
    private bool _hasTarget;

    public Light2D globalTargetLight;
    public Vector2 globalLightPos;

    private GameObject player;
    private PlayerAttack playerAttack;

    private GameObject[] lights;

    bool _playerFlashlight;
    float detectionRange = 4f;

    public static List<SombraAbandono> todasLasSombras = new List<SombraAbandono>();

    private bool _estoyHuyendo;

    private bool isIdle = false;
    private Vector3 idleStartPos;
    private float floatAmplitude = 0.2f; // altura del flotado
    private float floatFrequency = 1f;   // velocidad del flotado


    private AudioSource audioSource;
    public AudioClip dieSound;           // sonido al recibir daño

    private LevelManager lm; 
    private bool alreadyCounted = false;

    Rigidbody2D rb;

    private void Start()
    {
        lm = FindAnyObjectByType<LevelManager>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // si no tiene audioSource, se lo añadimos
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        if (data != null)
            InitializeStats(data.maxHealth);

        player = GameObject.FindGameObjectWithTag("Player");

        playerAttack = player.GetComponent<PlayerAttack>(); 

        lights = GameObject.FindGameObjectsWithTag("Light");
    }

    private void Update()
    {

        Vector3 dir = direccionHuida;

        if (float.IsNaN(dir.x) || float.IsNaN(dir.y) ||
            float.IsInfinity(dir.x) || float.IsInfinity(dir.y))
        {
            Debug.LogWarning("Dirección corrupta detectada, regenerando…");

            dir = Random.insideUnitCircle.normalized;
            direccionHuida = dir;
        }


        transform.Translate(dir * data.moveSpeed * Time.deltaTime * 0.1f);

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerAttack = player.GetComponent<PlayerAttack>();
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

    public bool OtraSombraHuyendo()
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
        Debug.Log("Funciona JugadorCerca");
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
    public bool HaLlegadoLuz()
    {

        return Vector2.Distance(transform.position, target.position) < 0.2f;
    }
    public void SeekLight()
    {
        

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
                    target.light2D = l;
                    target.position = obj.transform.position;
                    _hasTarget = true;
                    //closestLight = l;
                    //closestPos = obj.transform.position;
                }
            }
        }

    }


    public StatusFlags MoveToLight()
    {


       
        globalTargetLight = target.light2D;
        globalLightPos = target.position;

        data.moveSpeed = 2.5f;

        Vector2 dir = (Vector2)transform.position - globalLightPos;
        direccionHuida = dir;

        while (!HaLlegadoLuz()) {
            if (JugadorCerca()) { 
                return StatusFlags.Failure;
            }
            return StatusFlags.Running;
        }
        if (HaLlegadoLuz()) {
            return StatusFlags.Success;
        }
        else { return StatusFlags.Failure; }
            
    }

    public void SwitchOffLight()
    {

        target.light2D.intensity = 0f;

    }

    public void Huir()
    {

         data.moveSpeed = 2.5f;

        Vector2 dir = transform.position - player.transform.position ;
        direccionHuida = dir;
    }

    public StatusFlags HuirSombra()
    {
        data.moveSpeed = 5f;

        if (direccionHuida == Vector2.zero)
            direccionHuida = Random.insideUnitCircle.normalized;

        bool above = transform.position.y > other.position.y;

        Vector2 perp = new Vector2(-direccionHuida.y, direccionHuida.x);

        if (!above)
            perp = -perp;

        float t = Random.Range(0.1f, 0.9f);
        Vector2 newDir = Vector2.Lerp(direccionHuida.normalized, perp.normalized, t).normalized;

        // Si por lo que sea vuelve a ser cero:
        if (newDir == Vector2.zero)
            newDir = Random.insideUnitCircle.normalized;

        direccionHuida = newDir;

        if (OtraSombraHuyendo())
            return StatusFlags.Running;

        return StatusFlags.Failure;
    }


    public StatusFlags PerseguirJugador()
    {
        data.moveSpeed = 2f;

        Vector2 dir = player.transform.position - transform.position;

        direccionHuida = dir;
        while (JugadorCerca() && PlayerHasFlashLight()) { 
            return StatusFlags.Running;
        }

        return StatusFlags.Failure;
        
            
    }


    public void Idle()
    {
        // Se detiene completamente la sombra
        data.moveSpeed = 0;
        direccionHuida = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;        // <-- Detiene la velocidad
            rb.angularVelocity = 0f;           // <-- Limpia rotación (opcional)
        }

        // Activamos el flotado (idle behavior visual)
        if (!isIdle)
        {
            idleStartPos = transform.position;
            isIdle = true;
        }

        if (animator != null)
            animator.SetTrigger("Idle");
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
            animator.SetTrigger("Die");

        if (dieSound != null)
            AudioSource.PlayClipAtPoint(dieSound, transform.position, 0.8f);

        if (data != null && data.dieEffect != null)
            Instantiate(data.dieEffect, transform.position, Quaternion.identity);

        if (!alreadyCounted && lm != null)
        {
            lm.numeroDeSombras--;
            alreadyCounted = true;
        }

        base.Die();
    }

}
