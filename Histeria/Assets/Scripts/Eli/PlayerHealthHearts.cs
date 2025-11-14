
using System;
using System.Collections;
using UnityEditor.SearchService;
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

    private void Update()
    {
        
        if(currentHealth == 0)
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

        // Poner rojo
        sr.color = Color.red;

        // Esperar un pequeño tiempo para que se vea el efecto
        yield return new WaitForSeconds(0.2f);

        // Volver al color original
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
        // Lanzar animación de pulso para los corazones afectados
        if (currentHealth >= 1) StartCoroutine(PulseHeart(heart1.transform));
        if (currentHealth >= 2) StartCoroutine(PulseHeart(heart2.transform));
        if (currentHealth >= 3) StartCoroutine(PulseHeart(heart3.transform));
    }

    private IEnumerator PulseHeart(Transform heart)
    {
        Vector3 originalScale = heart.localScale;
        Vector3 targetScale = originalScale * 1.2f; // crecer 20%
        float duration = 0.1f; // tiempo para crecer

        // Crecer
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            heart.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }

        // Volver a tamaño normal
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            heart.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            yield return null;
        }

        heart.localScale = originalScale; // asegurar valor final
    }

    private void Die()
    {
        Transform t = gameObject.GetComponent<Transform>();
        if (t != null)
        {
            StartCoroutine(DieAnimation(t));
        }

    }

    private IEnumerator DieAnimation(Transform t)
    {
       
        t.rotation = Quaternion.AngleAxis(90.0f, new Vector3(0.0f,0.0f,90.0f));

        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
