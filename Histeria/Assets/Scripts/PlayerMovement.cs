using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement_WithDash : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float smoothTime = 0.05f;

    [Header("Dash")]
    public float dashSpeed = 12f;       // velocidad del dash
    public float dashDuration = 0.15f;  // cuánto dura el dash
    public float dashCooldown = 0.5f;   // tiempo antes de poder volver a usarlo

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
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
            sr.flipX = smoothInput.x < 0;

        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.Space) && canDash && moveInput.magnitude > 0.1f)
        {
            StartCoroutine(DoDash());
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

        anim.SetTrigger("Dash"); // Lanza la animación de dash

        rb.linearVelocity = lastMoveDir * dashSpeed;

        if (showDebug)
            Debug.Log($"DASH → Dir: {lastMoveDir}, Velocidad: {rb.linearVelocity}");

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        // corta el movimiento al terminar el dash
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
