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

    private Light2D[] lights;

    bool _playerFlashlight;
    float detectionRange = 10f;

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

        lights = FindObjectsOfType<Light2D>();
        Debug.Log("Luces encontradas: " + lights.Length);
    }

    private void Update()
    {
        if (player == null || playerAttack == null) return;

        // Obtenemos la dirección del crosshair
        CrosshairController cross = player.GetComponentInChildren<CrosshairController>();
        Vector2 dirJugadorACross = cross != null ? cross.dir.normalized : Vector2.right;

        Vector2 dirJugadorASombra = (Vector2)(transform.position - player.transform.position);
        float distancia = dirJugadorASombra.magnitude;

        // Si el crosshair apunta a la sombra y está dentro del rango
        bool apuntado = Vector2.Angle(dirJugadorACross, dirJugadorASombra) < 30f && distancia < detectionRange;

        if (apuntado)
        {
            // Huir del jugador
            _estoyHuyendo = true;
            direccionHuida = dirJugadorASombra.normalized;
        }
        else
        {
            _estoyHuyendo = false;

            // Si no está apuntado, perseguir jugador si está cerca
            if (JugadorCerca())
                direccionHuida = (player.transform.position - transform.position).normalized;
            else
                IdleFloating();
        }

        // Aplicar movimiento con Rigidbody2D
        if (rb != null)
            rb.velocity = direccionHuida * data.moveSpeed;
    }


    // Función para flotado idle visual
    private void IdleFloating()
    {
        if (!isIdle)
        {
            idleStartPos = transform.position;
            isIdle = true;
        }

        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        Vector3 newPos = idleStartPos + new Vector3(0, yOffset, 0);
        transform.position = newPos;

        if (animator != null)
            animator.SetTrigger("GoIdle");
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

        return Vector2.Distance(transform.position, player.transform.position) < detectionRange;
    }

    public bool JugadorNoCerca()
    {

        return Vector2.Distance(transform.position, player.transform.position) > detectionRange;
    }



    public bool TengoLuz()
    {

        return _hasTarget;

    }
    public bool HaLlegadoLuz()
    {

        return Vector2.Distance(transform.position, globalLightPos) < 1f;
    }
    public StatusFlags SeekLight()
    {
        lights = FindObjectsOfType<Light2D>();

        float minDist = Mathf.Infinity;
        Light2D bestLight = null;

        foreach (Light2D l in lights)
        {
            if (l == null || l.intensity <= 0f)
                continue;

            float dist = Vector2.Distance(transform.position, l.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                bestLight = l;
            }
        }

        if (bestLight != null)
        {
            Debug.Log("Luz objetivo seleccionada: " + bestLight.gameObject.name);
            globalTargetLight = bestLight;
            globalLightPos = bestLight.transform.position;
            _hasTarget = true;
            return StatusFlags.Success;
        }

        _hasTarget = false;
        return StatusFlags.Failure;
    }






    public StatusFlags MoveToLight()
    {
        if (!(_hasTarget && globalTargetLight != null))
            return StatusFlags.Failure;

        data.moveSpeed = 2.5f;
        direccionHuida = (globalLightPos - (Vector2)transform.position).normalized;

        if (!HaLlegadoLuz())
            return StatusFlags.Running;

        // Ha llegado
        return StatusFlags.Success;
    }




    public StatusFlags SwitchOffLight()
    {
        if (globalTargetLight != null)
        {
            // 1. Establece la intensidad a cero (opcional, pero buena práctica)
            globalTargetLight.intensity = 0f;

            // 2. DESACTIVA EL COMPONENTE Light2D para forzar que permanezca apagado.
            globalTargetLight.enabled = false;

            // Opcional: Si quieres desactivar el GameObject completo:
            // globalTargetLight.gameObject.SetActive(false); 
        }
        
        ClearLightTarget();
        return StatusFlags.Success;
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
            animator.SetTrigger("GoIdle");


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
            LevelManager.instance.EnemyMuerto();
            DungeonPopulator dp = GameObject.FindGameObjectWithTag("Rooms").GetComponent<DungeonPopulator>();
            dp.enemyNumber--;
            alreadyCounted = true;
        }

        base.Die();
    }

}
