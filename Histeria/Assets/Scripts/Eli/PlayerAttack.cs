
using System;
using UnityEngine;

using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    [Header("UI")]
    public CrosshairController crosshair; // referencia al crosshair


    [Header("Punch")]
    public int punchDamage = 1;
    public float punchRange = 1f;

    [Header("Lagrimas")]
    public GameObject lagrima;

    [Header("Audio Disparo")]
    public AudioClip tearSound; 
    [Range(0f, 1f)] public float tearVolume = 0.5f;
    private AudioSource audioSource;

    [Header("Tengo Linterna?")]
    public bool _hasFlashlight;
    public  event Action<bool> OnFlashlightChanged;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void SetFlashlight(bool value)
    {
        if (_hasFlashlight == value) return; // No notificar si no cambió
        _hasFlashlight = value;
        OnFlashlightChanged?.Invoke(_hasFlashlight); // Notificar a todos los enemigos
    }

    public bool PlayerHasFlashLight()
    {
        return _hasFlashlight;
    }


    public void Punch()
    {
        if (crosshair == null) return;

        Vector3 attackPos = transform.position;

        // Detecta todo lo que esté en el rango del puño
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, punchRange);

        foreach (var hit in hits)
        {
            SombraAbandono sombra = hit.GetComponent<SombraAbandono>();
            if (sombra != null)
            {
                // sombra.TakeDamageFromLight(1); 
            }

            BossController boss = hit.GetComponent<BossController>();

            if (boss == null) boss = hit.GetComponentInParent<BossController>();

            if (boss != null)
            {
                boss.TakeDamage(punchDamage);
            }

            MinionAI minion = hit.GetComponent<MinionAI>();
            if (minion != null)
            {
                minion.TakeDamage(punchDamage);
            }
        }
    }


    public void DispararLagrimas()
    {
        if (crosshair == null || lagrima == null) return;

        Vector3 target = crosshair.transform.position;
        target.z = 0f;
        Vector3 dir = (target - transform.position).normalized;


        GameObject tear = Instantiate(lagrima, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity);

        if (audioSource != null && tearSound != null)
        {
            
            audioSource.pitch = UnityEngine.Random.Range(0.85f, 1.15f);

            audioSource.PlayOneShot(tearSound, tearVolume);
        }

        Debug.Log("LÃ¡grima instanciada en " + transform.position);


        LagrimasAttack la = tear.GetComponent<LagrimasAttack>();
        if (la != null) la.Initialize(dir, LagrimasAttack.Team.Player);


        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        tear.transform.rotation = Quaternion.Euler(0, 0, angle) * lagrima.transform.rotation;

        // Notificar a las Eli corruptas
        Debug.Log($"Notificando clones. Dir={dir}");
        EliCorrupta[] clones = UnityEngine.Object.FindObjectsByType<EliCorrupta>(FindObjectsSortMode.None);
        foreach (var clone in clones)
        {
            Debug.Log($"Clone: {clone.name} puedeDisparar={clone.PuedeDispararDebug()}");
            clone.DispararEspejo(dir);
        }


    }

   
}