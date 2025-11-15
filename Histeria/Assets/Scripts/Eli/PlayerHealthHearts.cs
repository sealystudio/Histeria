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

    public GameObject deathCanvas; // ← Arrastra aquí tu Canvas de muerte

    private void Start()
    {
        if (deathCanvas != null)
            deathCanvas.SetActive(false); // ocultar al iniciar
    }

    private void Update()
    {
        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void TakeDamage(int amount = 1)
    {
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
    }

    private IEnumerator DieAnimation()
    {
        // Animación opcional
        transform.rotation = Quaternion.Euler(0, 0, 90);

        yield return new WaitForSeconds(0.2f);

        // En vez de recargar escena → mostrar canvas
        if (deathCanvas != null)
            deathCanvas.SetActive(true);

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
