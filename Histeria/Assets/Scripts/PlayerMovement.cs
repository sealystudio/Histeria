using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement_Smooth_Final : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float smoothTime = 0.05f; // Suavizado rápido para movimiento fluido
    public bool showDebug = false;     // Activa o desactiva logs de debug

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private Vector2 moveInput;
    private Vector2 smoothInput;
    private Vector2 inputVelocity;

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
        
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        //suavizaar movimeinto
        smoothInput = Vector2.SmoothDamp(smoothInput, moveInput, ref inputVelocity, smoothTime);

        //valor Speed del animator
        anim.SetFloat("Speed", moveInput.magnitude);

        // Flip horizontal del sprite
        if (smoothInput.x != 0)
            sr.flipX = smoothInput.x < 0;

        if (showDebug)
        {
            Debug.Log($"RawInput: {moveInput}, SmoothInput: {smoothInput}, Rigidbody Velocity: {smoothInput * moveSpeed}");
        }
    }

    void FixedUpdate()
    {
        // movimeitno suavizado
        rb.linearVelocity = smoothInput * moveSpeed;
    }
}
