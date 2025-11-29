using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthHearts : MonoBehaviour
{
    public Image heart1;
    public Image heart2;
    public Image heart3;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Range(0, 3)]
    public int currentHealth = 3;

    public GameObject deathCanvas;
    public GameObject dialogoLinterna;

    private bool primerCorazon = false;

    private void Start()
    {
        if (dialogoLinterna != null)
            dialogoLinterna.SetActive(false);

        if (deathCanvas != null)
            deathCanvas.SetActive(false);
    }

    private void Update()
    {
        if (currentHealth == 0)
        {
            Die();
        }
        if (dialogoLinterna == null) return;
    }

    public void TakeDamage(int amount = 1)
    {
        if (!primerCorazon && dialogoLinterna != null)
        {
            Time.timeScale = 0f;

            // Activar canvas de diálogo
            dialogoLinterna.SetActive(true);

            // Bloquear movimiento YA aquí
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.canMove = false;
                pm.puedeDisparar = false;
            }

            DialogueText dt = dialogoLinterna.GetComponentInChildren<DialogueText>();
            if (dt != null)
            {
                dt.InitDialogue("Nivel_1_prov");
            }

            primerCorazon = true;
        }

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHearts();

        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            StartCoroutine(DamageFlash(sr));
        }
    }


    private IEnumerator DamageFlash(SpriteRenderer sr)
    {
        Color originalColor = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sr.color = originalColor;
    }

    public void Heal(int amount = 1)
    {
        currentHealth = Mathf.Min(currentHealth + amount, 3);
        UpdateHearts();
    }

    void UpdateHearts()
    {
        heart1.sprite = currentHealth >= 1 ? fullHeart : emptyHeart;
        heart2.sprite = currentHealth >= 2 ? fullHeart : emptyHeart;
        heart3.sprite = currentHealth >= 3 ? fullHeart : emptyHeart;
    }

    private void Die()
    {
        StartCoroutine(DieAnimation());
        // Detener todos los audios de la escena
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in allAudio)
        {
            a.Stop();
        }
    }

    private IEnumerator DieAnimation()
    {
        // Animación opcional
        transform.rotation = Quaternion.Euler(0, 0, 90);

        yield return new WaitForSeconds(0.2f);

        // En vez de recargar escena → mostrar canvas
        if (deathCanvas != null)
        {
            deathCanvas.SetActive(true);
            Cursor.visible = true;
        }

        // Congelar el juego
        Time.timeScale = 0f;
    }

    // BOTÓN REINTENTAR
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // BOTÓN SALIR
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

        // Para que funcione en el editor:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
