using UnityEngine;
using BehaviourAPI.Core;

public class VacioController : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadBase = 3.0f;
    public float multiplicadorSinergia = 2.0f;
    public float rangoDeteccion = 4.0f;
    public float rangoAtaque = 1.5f;
    public float rangoSinergia = 1.0f;
    public float radioPatrulla = 3.0f;

    [Header("Referencias")]
    public Animator anim;

    private Transform jugador;
    private Vector3 posicionOrigen;
    private Vector3 puntoDestinoPatrulla;
    private bool estaPatrullando = false;
    private bool estaBufado = false;
    private float velocidadActual;

    void Awake()
    {
        posicionOrigen = transform.position;
        puntoDestinoPatrulla = posicionOrigen;
        velocidadActual = velocidadBase;
        BuscarJugador();
    }

    void BuscarJugador()
    {
        if (jugador != null) return;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
    }

    void Update()
    {
        if (jugador == null) BuscarJugador();
        if (estaBufado && CheckVacioCerca() == Status.Failure) DetenerSinergia();
        Debug.Log("Velocidad: " + velocidadActual);
    }

    // --- PERCEPCIONES (Status) ---
    // Estas SÍ se quedan como Status porque son preguntas (Check...)

    public Status CheckRangoDeteccion()
    {
        if (jugador == null) return Status.Failure;
        float distancia = Vector2.Distance(transform.position, jugador.position);

        // SOLO imprime si es Success
        if (distancia <= rangoDeteccion)
        {
            //Debug.Log($"¡TE VEO DE VERDAD! Distancia: {distancia}");
            return Status.Success;
        }
        return Status.Failure;
    }

    public Status CheckJugadorMueve()
    {
        bool mueve = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        return mueve ? Status.Success : Status.Failure;
    }

    public Status CheckRangoAtaque()
    {
        if (jugador == null) return Status.Failure;
        return Vector2.Distance(transform.position, jugador.position) <= rangoAtaque ? Status.Success : Status.Failure;
    }

    public Status CheckEstaEnOrigen() => (Vector2.Distance(transform.position, posicionOrigen) < 0.2f) ? Status.Success : Status.Failure;

    public Status CheckPuedePatrullar() => (CheckEstaEnOrigen() == Status.Success || estaPatrullando) ? Status.Success : Status.Failure;

    public Status CheckVacioCerca()
    {
        Collider2D[] cercanos = Physics2D.OverlapCircleAll(transform.position, rangoSinergia);
        foreach (var col in cercanos)
        {
            if (col.gameObject != this.gameObject && col.GetComponent<VacioController>()) return Status.Success;
        }
        return Status.Failure;
    }

    // --- ACCIONES (AHORA SON VOID / SIMPLE ACTIONS) ---
    // Ya no devuelven nada, solo hacen "un poquito" de trabajo cada frame.

    public void Perseguir()
    {
        estaPatrullando = false;
        if (jugador != null) MoverHacia(jugador.position);
        Debug.Log("¡Perseguir! " + velocidadActual);
    }

    public void Patrullar()
    {
        estaPatrullando = true;

        // Si llegamos al destino, calculamos otro
        if (Vector2.Distance(transform.position, puntoDestinoPatrulla) < 0.2f)
        {
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioPatrulla;
            puntoDestinoPatrulla = posicionOrigen + new Vector3(puntoAleatorio.x, puntoAleatorio.y, 0);
        }
        MoverHacia(puntoDestinoPatrulla);
        Debug.Log("¡Patrulla! " + velocidadActual);
    }

    public void VolverAOrigen()
    {
        estaPatrullando = false;
        MoverHacia(posicionOrigen);
        Debug.Log("Vuelvo:");
    }

    public void Idle() { if (anim) anim.SetBool("Moviendo", false); }
    public void AplicarSinergia() { if (!estaBufado) { velocidadActual = velocidadBase * multiplicadorSinergia; estaBufado = true; Debug.Log("Me bufo: " + velocidadActual); } }
    public void Atacar() => Debug.Log("¡Ataque!");

    // --- MÉTODOS PRIVADOS ---
    private void DetenerSinergia() { velocidadActual = velocidadBase; estaBufado = false; }

    private void MoverHacia(Vector3 objetivo)
    {
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidadActual * Time.deltaTime);
        Vector3 direccion = (objetivo - transform.position).normalized;

        if (direccion.x > 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Abs(escala.x);
            transform.localScale = escala;
        }
        else if (direccion.x < 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = -Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        if (anim) anim.SetBool("Moviendo", true);
    }
}