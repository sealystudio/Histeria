using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement_WithDash : MonoBehaviour
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

    public bool enableDashWind = true; // true = VFX activado, false = desactivado
    public GameObject dashWindPrefab;
    public Vector2 dashWindOffset = new Vector2(-0.2f, 0f); // detrás
    public float dashWindScale = 0.06f;



    [Header("Ataque")]
    //Punch
    public float punchDuration = 0.3f; 
    public float punchCooldown = 0.5f;

    //Lagrimas
    [SerializeField] private PlayerAttack attack;
    private float lagrimasCooldown = 0.5f;
    private bool puedeDisparar = true;
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
    private Vector2 lastMoveDir; // para recordar hacia dónde se movía

    private bool isDashing = false;
    private bool canDash = true;
    private bool isPunching = false;
    private bool canPunch = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        attack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        // --- No permite controlar movimiento mientras dashea ---
        if (isDashing) return;

        // --- Movimiento normal ---
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        smoothInput = Vector2.SmoothDamp(smoothInput, moveInput, ref inputVelocity, smoothTime);

        // --- Animación ---
        anim.SetFloat("Speed", moveInput.magnitude);

        // --- Dirección de movimiento ---
        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDir = moveInput; // actualiza la dirección mientras se mueve

        // --- Flip sprite ---
        if (smoothInput.x != 0)
            sr.flipX = crosshair.dir.x < 0;
        
           

        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.Space) && canDash && moveInput.magnitude > 0.1f)
        {
            StartCoroutine(DoDash());
        }

        // --- Ataques ---
        //puño
        if (Input.GetMouseButtonDown(0) && canPunch)
        {
            StartCoroutine(DoPunch());
        }

        //lagrimas
        if (Input.GetMouseButtonDown(1) && puedeDisparar) 

        {
            canPunch = false;
            StartCoroutine(DoLinterna());
        }


        if (showDebug)
        {
            Debug.Log($"RawInput: {moveInput}, SmoothInput: {smoothInput}, Rigidbody Velocity: {smoothInput * moveSpeed}");
        }
    }

    

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = smoothInput * moveSpeed;
        }
    }

    IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;

        anim.SetTrigger("Dash");

        // --- Humo en los pies ---
        if (dashSmokePrefab != null)
        {
            Vector3 spawnPos = transform.position + (Vector3)dashSmokeOffset;
            GameObject smoke = Instantiate(dashSmokePrefab, spawnPos, Quaternion.identity);
            smoke.transform.localScale = Vector3.one * 0.5f; // porque el humo es 32x32
            Destroy(smoke, 0.5f);
        }

        // --- Viento detrás del personaje ---
        if (dashWindPrefab != null && enableDashWind)
        {
            

            // Convertimos todo a Vector3
            Vector3 moveDir3D = new Vector3(lastMoveDir.x, lastMoveDir.y, 0f);
            Vector3 windOffset = (-moveDir3D * 0.3f) + new Vector3(dashWindOffset.x, dashWindOffset.y, 0f);
            Vector3 spawnPos = transform.position + windOffset;

            GameObject wind = Instantiate(dashWindPrefab, spawnPos, Quaternion.identity);
            wind.transform.localScale = new Vector3(
                  wind.transform.localScale.x,
                 -Mathf.Abs(wind.transform.localScale.y),  // invertir el eje Y
                 wind.transform.localScale.z
            );
            wind.transform.localPosition += new Vector3(-lastMoveDir.x * 0.8f, 0, 0); // para que este un poco detrás

            // Escalado correcto (512x512 → 16x16)
            wind.transform.localScale = Vector3.one * dashWindScale;

            // Orientar según dirección
            if (lastMoveDir.x < 0)
                wind.transform.localScale = new Vector3(-dashWindScale, dashWindScale, 1);

            wind.transform.SetParent(transform);

            Destroy(wind, 0.6f);
        }

        // --- Movimiento del dash ---
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

    IEnumerator DoLinterna()
    {
        if (!attack.tieneLinterna) yield break; // No hace nada si no tiene linterna

        puedeDisparar = false;
        estaDisparando = true;

        attack.DispararLinterna(); // Solo flash de linterna

        yield return new WaitForSeconds(0.1f);
        estaDisparando = false;

        yield return new WaitForSeconds(lagrimasCooldown);
        puedeDisparar = true;
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


}
