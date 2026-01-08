using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using BehaviourAPI.Core;

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
    

    public static List<SombraAbandono> todasLasSombras = new List<SombraAbandono>();

    private bool _estoyHuyendo;

    private bool isIdle = false;
    private Vector3 idleStartPos;
    private float floatAmplitude = 0.2f;
    private float floatFrequency = 1f;

    private AudioSource audioSource;
    public AudioClip dieSound;

    private LevelManager lm;
    private bool alreadyCounted = false;

    Rigidbody2D rb;

    private void Start()
    {
        lm = FindAnyObjectByType<LevelManager>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        if (data != null)
            InitializeStats(data.maxHealth , 10f , this.GetComponent<Rigidbody2D>());

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
            _playerFlashlight = playerAttack._hasFlashlight;
        }

        // CORRECCIÓN: Inicializamos idleStartPos al principio para evitar saltos al 0,0
        idleStartPos = transform.position;

        lights = FindObjectsOfType<Light2D>();
    }

    private void Update()
    {
        if (player == null || playerAttack == null)
            return;

        if (Time.timeScale == 0f)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        CrosshairController cross = player.GetComponentInChildren<CrosshairController>();
        Vector2 dirJugadorACross = cross != null ? cross.dir.normalized : Vector2.right;

        Vector2 dirJugadorASombra = (Vector2)(transform.position - player.transform.position);
        float distancia = dirJugadorASombra.magnitude;

        bool apuntado = Vector2.Angle(dirJugadorACross, dirJugadorASombra) < 30f && distancia < detectionRange;

        if (apuntado)
        {
            _estoyHuyendo = true;
            isIdle = false; 
            direccionHuida = dirJugadorASombra.normalized;
        }
        else
        {
            _estoyHuyendo = false;

            if (JugadorCerca())
            {
                isIdle = false;
                direccionHuida = (player.transform.position - transform.position).normalized;
            }
            else
            {

                if (!isIdle)
                {

                    idleStartPos = transform.position;
                    isIdle = true;

                    if (rb != null) rb.linearVelocity = Vector2.zero;
                }

                direccionHuida = Vector2.zero;
            }
        }

        if (rb != null && !isIdle)
            rb.linearVelocity = direccionHuida * data.moveSpeed;
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f) return;


        if (isIdle)
        {
            IdleFloatingPhysics();
        }
    }

    private void IdleFloatingPhysics()
    {

        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        Vector3 newPos = idleStartPos + new Vector3(0, yOffset, 0);

        rb.MovePosition(newPos);

        if (animator != null)
            animator.SetTrigger("GoIdle");
    }


    public void TakeDamageFromLight(int amount)
    {
        TakeDamage(amount, Vector2.zero);
        if (data != null && data.hitEffect != null)
        {
            GameObject effect = Instantiate(data.hitEffect, transform.position, Quaternion.identity);
            effect.transform.position = transform.position;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerAttack = player.GetComponent<PlayerAttack>();
    }

    public bool PlayerHasFlashLight() => _playerFlashlight;
    public bool PlayerHasNoFlashLight() => !_playerFlashlight;
    public bool EstoyHuyendo() => _estoyHuyendo;

    public bool OtraSombraHuyendo()
    {
        if (otraSombra == null) return false;
        SombraAbandono sombraHuye = otraSombra.GetComponent<SombraAbandono>();
        if (sombraHuye == null) sombraHuye = otraSombra.GetComponentInParent<SombraAbandono>();

        if (Vector2.Distance(transform.position, otraSombra.transform.position) < detectionRange)
            return sombraHuye != null && sombraHuye.EstoyHuyendo();

        return false;
    }

    public bool JugadorCerca() => Vector2.Distance(transform.position, player.transform.position) < detectionRange;
    public bool JugadorNoCerca() => Vector2.Distance(transform.position, player.transform.position) > detectionRange;
    public bool TengoLuz() => _hasTarget;
    public bool HaLlegadoLuz() => Vector2.Distance(transform.position, globalLightPos) < 1f;

    
    public StatusFlags SeekLight()
    {
        lights = FindObjectsOfType<Light2D>();
        float minDist = Mathf.Infinity;
        Light2D bestLight = null;

        foreach (Light2D l in lights)
        {
            if (l == null || l.intensity <= 0f) continue;
            float dist = Vector2.Distance(transform.position, l.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                bestLight = l;
            }
        }

        if (bestLight != null)
        {
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
        if (!(_hasTarget && globalTargetLight != null)) return StatusFlags.Failure;
        data.moveSpeed = 2.5f;
        direccionHuida = (globalLightPos - (Vector2)transform.position).normalized;
        if (!HaLlegadoLuz()) return StatusFlags.Running;
        return StatusFlags.Success;
    }

    public StatusFlags SwitchOffLight()
    {
        if (globalTargetLight != null)
        {
            globalTargetLight.intensity = 0f;
            globalTargetLight.enabled = false;
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
        direccionHuida = (transform.position - player.transform.position); // Corrección vector
        if (JugadorCerca() && PlayerHasFlashLight()) return StatusFlags.Running;
        if (JugadorNoCerca()) return StatusFlags.Failure;
        return StatusFlags.Success;
    }

    public StatusFlags HuirSombra()
    {
        data.moveSpeed = 5f;
        if (direccionHuida == Vector2.zero) direccionHuida = Random.insideUnitCircle.normalized;

        bool above = transform.position.y > other.position.y;
        Vector2 perp = new Vector2(-direccionHuida.y, direccionHuida.x);
        if (!above) perp = -perp;

        float t = Random.Range(0.1f, 0.9f);
        Vector2 newDir = Vector2.Lerp(direccionHuida.normalized, perp.normalized, t).normalized;
        if (newDir == Vector2.zero) newDir = Random.insideUnitCircle.normalized;
        direccionHuida = newDir;

        if (OtraSombraHuyendo()) return StatusFlags.Running;
        return StatusFlags.Failure;
    }

    public StatusFlags PerseguirJugador()
    {
        data.moveSpeed = 2f;
        direccionHuida = (player.transform.position - transform.position).normalized;
        if (JugadorCerca() && !PlayerHasFlashLight()) return StatusFlags.Running;
        if (!JugadorCerca()) return StatusFlags.Failure;
        return StatusFlags.Success;
    }

    public StatusFlags Idle()
    {
        data.moveSpeed = 0;
        direccionHuida = Vector2.zero;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (!isIdle)
        {
            idleStartPos = transform.position;
            isIdle = true;
        }

        if (animator != null) animator.SetTrigger("GoIdle");
        if (JugadorCerca()) return StatusFlags.Failure;

        return StatusFlags.Success;
    }
   
    protected override void OnHit()
    {
        if (animator != null) animator.SetTrigger("Hit");
        if (data != null && data.hitEffect != null) Instantiate(data.hitEffect, transform.position, Quaternion.identity);
    }

    protected override void Die()
    {
        if (animator != null) animator.SetTrigger("Die");
        if (dieSound != null) AudioSource.PlayClipAtPoint(dieSound, transform.position, 0.8f);
        if (data != null && data.dieEffect != null) Instantiate(data.dieEffect, transform.position, Quaternion.identity);

        if (!alreadyCounted && lm != null)
        {
            LevelManager.instance.EnemyMuerto();
            // Buscar el DungeonPopulator de forma más segura
            GameObject rooms = GameObject.FindGameObjectWithTag("Rooms");
            if (rooms != null)
            {
                DungeonPopulator dp = rooms.GetComponent<DungeonPopulator>();
                if (dp != null) dp.RestarEnemigo(); // Usar el método que creamos antes
            }
            alreadyCounted = true;
        }
        base.Die();
    }
}