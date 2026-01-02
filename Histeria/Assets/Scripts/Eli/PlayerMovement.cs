using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // <--- MANTENIDO: IMPRESCINDIBLE PARA CORREGIR EL CLICK
using Scene = UnityEngine.SceneManagement.Scene;

// NOTA: He quitado "using static UnityEditor.Progress" de la otra rama
// porque suele dar errores al compilar el juego final (Build).

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float smoothTime = 0.05f;
    public CrosshairController crosshair;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;

    [Header("VFX Dash")]
    public GameObject dashSmokePrefab;
    public Vector2 dashSmokeOffset = new Vector2(0, -0.4f);

    public bool enableDashWind = true;
    public GameObject dashWindPrefab;
    public Vector2 dashWindOffset = new Vector2(-0.2f, 0f);
    public float dashWindScale = 0.06f;

    [Header("Ataque")]
    //Punch
    public float punchDuration = 0.3f;
    public float punchCooldown = 0.5f;

    //Lagrimas
    [SerializeField] private PlayerAttack attack;
    private float lagrimasCooldown = 0.5f;
    public bool puedeDisparar = false;
    private bool estaDisparando = false;
    public float lifeTime = 0.2f;

    [Header("Debug")]
    public bool showDebug = false;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private Vector2 moveInput;
    private Vector2 smoothInput;
    private Vector2 inputVelocity;
    private Vector2 lastMoveDir;

    private bool isDashing = false;
    private bool canDash = true;
    private bool isPunching = false;
    public bool canPunch = true;
    public bool canMove = true;
    //public bool isMoving = false;

    [Header("Sonido")]
    [Tooltip("Añade aquí todos los clips de audio de pasos que tengas")]
    public AudioClip[] footstepSounds;
    [Tooltip("Sonidos de pasos para el Nivel 3 (Hierba)")]
    public AudioClip[] grassFootstepSounds;

    [Tooltip("Qué tan fuerte deben sonar los pasos (0.0 = silencio, 1.0 = volumen máximo)")]
    [Range(0f, 1f)]
    public float footstepVolume = 0.2f;

    [Tooltip("Clip de audio para el Dash")]
    public AudioClip dashSound;
    [Tooltip("Volumen del sonido de Dash (0.0 a 1.0)")]
    [Range(0f, 1f)]
    public float dashVolume = 0.7f;

    private PlayerEquipment playerEquipment;
    public static bool IsPaused = false;

    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        attack = GetComponent<PlayerAttack>();

        playerEquipment = GetComponent<PlayerEquipment>();
    }

#if UNITY_ANDROID || UNITY_IOS
    // He priorizado la lógica de MobileInputBridge de tu HEAD, 
    // pero si necesitas la variable antigua descoméntala:
    // public MobileJoystickReader mobileInput; 
#endif

    void Update()
    {
        if (IsPaused) return;

        // --- DEBUG DE SERGIO (MANTENIDO POR UTILIDAD) ---
        Debug.DrawRay(transform.position, Vector2.right * 4.5f, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * 4.5f, Color.red);
        Debug.DrawRay(transform.position, Vector2.up * 4.5f, Color.red);
        Debug.DrawRay(transform.position, Vector2.left * 4.5f, Color.red);
        Debug.DrawRay(transform.position, Vector2.right * 3, Color.red);
        // -----------------------------------------------

        if (isDashing || !canMove) return;

        // 1. REINICIAMOS LAS VARIABLES TEMPORALES CADA FRAME
        Vector2 inputMovil = Vector2.zero;
        Vector2 inputTeclado = Vector2.zero;

        // ========================================================================
        //                          LÓGICA MÓVIL
        // ========================================================================
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        // Usamos la lógica de HEAD que parece más completa con MobileInputBridge
        inputMovil = MobileInputBridge.MoveJoystick;

        // Botones Móviles
        if (MobileInputBridge.DashPressed)
        {
            MobileInputBridge.DashPressed = false;
            if (canDash && inputMovil.magnitude > 0.1f)
            {
                if (dashSound != null) audioSource.PlayOneShot(dashSound, dashVolume);
                StartCoroutine(DoDash());
            }
        }
        if (MobileInputBridge.MeleePressed)
        {
            MobileInputBridge.MeleePressed = false;
            if (canPunch && (playerEquipment == null || !playerEquipment.IsEquipped)) StartCoroutine(DoPunch());
        }
        if (MobileInputBridge.RangedPressed)
        {
            MobileInputBridge.RangedPressed = false;
            if (puedeDisparar && (playerEquipment == null || !playerEquipment.IsEquipped)) StartCoroutine(DoLagrimas());
        }
#endif

        // ========================================================================
        //                          LÓGICA PC (TECLADO)
        // ========================================================================
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        inputTeclado = new Vector2(h, v).normalized;

        // ========================================================================
        //                          MEZCLA DE INPUTS
        // ========================================================================

        // Si el joystick móvil se está usando, tiene prioridad. Si no, usa el teclado.
        if (inputMovil.magnitude > 0.1f)
        {
            moveInput = inputMovil;
            
        }
        else
        {
            moveInput = inputTeclado;
        }

        // ========================================================================
        //                          ATAQUES CON RATÓN
        // ========================================================================
        // Detectar si el ratón está tocando botones (UI) <--- ESTA ES LA CLAVE DEL CONFLICTO
        bool tocandoUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        // Solo atacamos con ratón si NO estamos en móvil real y NO estamos tocando UI
#if !UNITY_ANDROID && !UNITY_IOS
        if (!tocandoUI)
        {
            // DASH (Espacio)
            if (Input.GetKeyDown(KeyCode.Space) && canDash && moveInput.magnitude > 0.1f)
            {
                if (dashSound != null) audioSource.PlayOneShot(dashSound, dashVolume);
                StartCoroutine(DoDash());
                EliCorrupta[] clones = UnityEngine.Object.FindObjectsByType<EliCorrupta>(FindObjectsSortMode.None);
                foreach (var clone in clones)
                {
                    clone.StartCoroutine(DoDash());
                }

            }
            // PUÑO (Click Izq)
            if (Input.GetMouseButtonDown(0) && canPunch && (playerEquipment == null || !playerEquipment.IsEquipped))
            {
                StartCoroutine(DoPunch());
            }
            // DISPARO (Click Der)
            if (Input.GetMouseButtonDown(1) && puedeDisparar && (playerEquipment == null || !playerEquipment.IsEquipped))
            {
                StartCoroutine(DoLagrimas());
            }
        }
#endif

        // ========================================================================
        //                         PROCESAMIENTO FÍSICO
        // ========================================================================
        smoothInput = Vector2.SmoothDamp(smoothInput, moveInput, ref inputVelocity, smoothTime);
        anim.SetFloat("Speed", moveInput.magnitude);

        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDir = moveInput;

        if (smoothInput.x != 0 && crosshair != null)
            sr.flipX = crosshair.dir.x < 0;

        if (showDebug)
        {
            Debug.Log($"RawInput: {moveInput}, SmoothInput: {smoothInput}, Rigidbody Velocity: {smoothInput * moveSpeed}");
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name);
        if (scene.name == "Nivel2")
        {
            puedeDisparar = true;
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = smoothInput * moveSpeed;
        }
    }

    public bool IsMoving()
    {
        return (rb.linearVelocity.magnitude > 0.1f);
    }

    public Vector2 getMoveDirection()
    {
        return lastMoveDir;
    }

    IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;

        anim.SetTrigger("Dash");

        // spawnear humo en el dash
        if (dashSmokePrefab != null)
        {
            Vector3 spawnPos = transform.position + (Vector3)dashSmokeOffset;
            GameObject smoke = Instantiate(dashSmokePrefab, spawnPos, Quaternion.identity);
            smoke.transform.localScale = Vector3.one * 0.5f;
            Destroy(smoke, 0.5f);
        }

        // el viento 
        if (dashWindPrefab != null && enableDashWind)
        {
            Vector3 moveDir3D = new Vector3(lastMoveDir.x, lastMoveDir.y, 0f);
            Vector3 windOffset = (-moveDir3D * 0.3f) + new Vector3(dashWindOffset.x, dashWindOffset.y, 0f);
            Vector3 spawnPos = transform.position + windOffset;

            GameObject wind = Instantiate(dashWindPrefab, spawnPos, Quaternion.identity);
            wind.transform.localScale = new Vector3(
                 wind.transform.localScale.x,
                -Mathf.Abs(wind.transform.localScale.y),  // invertir el eje Y
                wind.transform.localScale.z
            );
            wind.transform.localPosition += new Vector3(-lastMoveDir.x * 0.8f, 0, 0);

            wind.transform.localScale = Vector3.one * dashWindScale;

            if (lastMoveDir.x < 0)
                wind.transform.localScale = new Vector3(-dashWindScale, dashWindScale, 1);

            wind.transform.SetParent(transform);

            Destroy(wind, 0.6f);
        }

        rb.linearVelocity = lastMoveDir * dashSpeed;

        if (showDebug)
            Debug.Log($"DASH → Dir: {lastMoveDir}, Velocidad: {rb.linearVelocity}");

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator DoPunch()
    {
        canPunch = false;
        isPunching = true;

        anim.SetTrigger("Punch");

        // Detener movimiento
        rb.linearVelocity = Vector2.zero;
        moveInput = Vector2.zero;
        smoothInput = Vector2.zero;

        yield return new WaitForSeconds(punchDuration);

        isPunching = false;

        yield return new WaitForSeconds(punchCooldown);
        canPunch = true;

        attack.Punch();
    }

    IEnumerator DoLagrimas()
    {
        puedeDisparar = false;
        estaDisparando = true;

        attack.DispararLagrimas();

        yield return new WaitForSeconds(0.1f);
        estaDisparando = false;

        yield return new WaitForSeconds(lagrimasCooldown);
        puedeDisparar = true;
    }

    public void PlayFootstepSound()
    {
        AudioClip[] soundPool = footstepSounds;
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "NIvel3" && grassFootstepSounds != null && grassFootstepSounds.Length > 0)
        {
            soundPool = grassFootstepSounds;
        }

        if (soundPool == null || soundPool.Length == 0)
        {
            return;
        }

        int randIndex = UnityEngine.Random.Range(0, soundPool.Length);
        AudioClip clipToPlay = soundPool[randIndex];

        // Volumen
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay, footstepVolume);
        }
    }
}