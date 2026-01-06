using UnityEngine;
using BehaviourAPI.Core;

public class VacioController : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadBase = 3.0f;
    public float multiplicadorSinergia = 2.0f;
    public float rangoDeteccion = 6.0f;
    public float rangoAtaque = 1.5f;
    public float rangoSinergia = 2.0f;
    public float radioPatrulla = 5.0f;

    [Header("Combate")]
    public int dañoAtaque = 1;
    public float cooldownAtaque = 1.5f;
    private float tiempoParaSiguienteAtaque = 0f;

    [Header("Referencias")]
    public Animator anim;

    private Transform jugador;
    private PlayerHealthHearts scriptVidaJugador;

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
        if (playerObj != null)
        {
            jugador = playerObj.transform;
            scriptVidaJugador = playerObj.GetComponent<PlayerHealthHearts>();
        }
    }

    void Update()
    {
        if (jugador == null) BuscarJugador();
        if (estaBufado && CheckVacioCerca() == Status.Failure) DetenerSinergia();
    }

    // --- PERCEPCIONES ---
    public Status CheckRangoDeteccion()
    {
        if (jugador == null) return Status.Failure;
        float distancia = Vector2.Distance(transform.position, jugador.position);
        if (distancia <= rangoDeteccion) return Status.Success;
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

    // --- ACCIONES ---
    public void Perseguir()
    {
        estaPatrullando = false;
        if (jugador != null) MoverHacia(jugador.position);
        Debug.Log("Persigo");
    }

    public void Patrullar()
    {
        estaPatrullando = true;
        if (Vector2.Distance(transform.position, puntoDestinoPatrulla) < 0.2f)
        {
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioPatrulla;
            puntoDestinoPatrulla = posicionOrigen + new Vector3(puntoAleatorio.x, puntoAleatorio.y, 0);
        }
        MoverHacia(puntoDestinoPatrulla);
        Debug.Log("Patrullo");
    }

    public void VolverAOrigen()
    {
        estaPatrullando = false;
        MoverHacia(posicionOrigen);
        Debug.Log("Vuelvo");
    }

    public void Idle() { if (anim) anim.SetBool("Moviendo", false); Debug.Log("Me paro"); }
    public void AplicarSinergia() { if (!estaBufado) { velocidadActual = velocidadBase * multiplicadorSinergia; estaBufado = true; } Debug.Log("Me bufo"); }

    public void Atacar()
    {
        if (Time.time < tiempoParaSiguienteAtaque) return;

        if (scriptVidaJugador != null)
        {
            Debug.Log("Ataco");

            scriptVidaJugador.TakeDamage(dañoAtaque);

            tiempoParaSiguienteAtaque = Time.time + cooldownAtaque;
        }
    }

    private void DetenerSinergia() { velocidadActual = velocidadBase; estaBufado = false; Debug.Log("Quito bufo"); }

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