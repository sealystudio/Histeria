using System.Collections.Generic;
using System.ComponentModel;
using BehaviourAPI.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;





public class SombraAbandono : EnemyBase
{
    public SombraAbandonoData data;

    public Transform other;
    public Transform otraSombra;

    Vector2 direccionHuida;

  

    public Light2D globalTargetLight;
    public Vector2 globalLightPos;
    bool _hasTarget;

    private GameObject player;
    private PlayerAttack playerAttack;

    private GameObject[] lights;

    bool _playerFlashlight;
    float detectionRange = 0.5f;

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


        _playerFlashlight = playerAttack._hasFlashlight;

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


        transform.Translate(dir * data.moveSpeed * Time.deltaTime * 0.5f);

        if (playerAttack != null)
            _playerFlashlight = playerAttack._hasFlashlight;

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
        Debug.Log("Funciona JugadorNOCerca");
        return Vector2.Distance(transform.position, player.transform.position) > detectionRange;
    }



    public bool TengoLuz()
    {

        return _hasTarget;

    }
    public bool HaLlegadoLuz()
    {

        return Vector2.Distance(transform.position, globalLightPos) < 0.2f;
    }
    public void SeekLight()
    {
        float minDist = float.MaxValue;
        _hasTarget = false;
        globalTargetLight = null;

        foreach (GameObject obj in lights)
        {
            Light2D l = obj.GetComponent<Light2D>();

            if (l != null && l.intensity > 0f)
            {
                float dist = Vector2.Distance(transform.position, obj.transform.position);

                
                if (dist < minDist)
                {
                    minDist = dist;
                    globalTargetLight = l;
                    globalLightPos = obj.transform.position;
                    _hasTarget = true;
                }
            }
        }

        if (_hasTarget)
            Debug.Log("He encontrado una luz: " + globalTargetLight.name);
        else
            Debug.Log("No hay luces disponibles.");
    }



    public StatusFlags MoveToLight()
    {
        if (!(_hasTarget && globalTargetLight != null))
            return StatusFlags.Failure;

        data.moveSpeed = 2.5f;

        
        direccionHuida = (globalLightPos - (Vector2)transform.position).normalized;

        if (JugadorCerca())
            return StatusFlags.Failure;

        if (!HaLlegadoLuz())
            return StatusFlags.Running;

        return StatusFlags.Success;
    }



    public void SwitchOffLight()
    {
        if (globalTargetLight != null)
            globalTargetLight.intensity = 0f;

        ClearLightTarget();
    }



    void ClearLightTarget()
    {
        globalTargetLight = null;
        _hasTarget = false;
    }

    public StatusFlags Huir()
    {

         data.moveSpeed = 2.5f;

        Vector2 dir = transform.position - player.transform.position ;
        direccionHuida = dir;

        if (JugadorCerca() && PlayerHasFlashLight())
            return StatusFlags.Running;

        if (JugadorNoCerca())
            return StatusFlags.Failure;

        return StatusFlags.Success;
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

        
        direccionHuida = (player.transform.position - transform.position).normalized;

        
        if (JugadorCerca() && !PlayerHasFlashLight())
            return StatusFlags.Running;

        if(!JugadorCerca())
            return StatusFlags.Failure;


        return StatusFlags.Success;
    }




public StatusFlags Idle()
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


        if (JugadorCerca())
            return StatusFlags.Failure;



        return StatusFlags.Success;
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
