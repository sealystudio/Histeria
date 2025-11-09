using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealthHearts : MonoBehaviour
{
    [Header("Vida")]
    public int maxHearts = 5;
    public int currentHealth; // Cada corazón = 2 puntos

    [Header("Referencias UI")]
    public Image heartPrefab;
    public Sprite fullHeart, halfHeart, emptyHeart;

    private List<Image> hearts = new List<Image>();

    void Start()
    {
        currentHealth = maxHearts * 2;
        CreateHearts();
        UpdateHearts();
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(1);

        if (Input.GetKeyDown(KeyCode.J))
            Heal(1);
    }


    void CreateHearts()
    {
        // Borra hijos anteriores (por si acaso)
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            // Instancia el prefab como hijo del panel vacío
            Image heart = Instantiate(heartPrefab, transform);
            heart.transform.localScale = Vector3.one; // asegurar escala correcta
            hearts.Add(heart);
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Max(currentHealth - dmg, 0);
        UpdateHearts();
        StartCoroutine(AnimateHearts());
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHearts * 2);
        UpdateHearts();
        StartCoroutine(AnimateHearts());
    }

    void UpdateHearts()
    {
        int tempHealth = currentHealth;

        foreach (Image heart in hearts)
        {
            if (tempHealth >= 2)
            {
                heart.sprite = fullHeart;
                tempHealth -= 2;
            }
            else if (tempHealth == 1)
            {
                heart.sprite = halfHeart;
                tempHealth -= 1;
            }
            else
            {
                heart.sprite = emptyHeart;
            }
        }
    }

    private Dictionary<RectTransform, Coroutine> activeCoroutines = new Dictionary<RectTransform, Coroutine>();

    IEnumerator AnimateHearts()
    {
        foreach (Image heart in hearts)
        {
            RectTransform rt = heart.rectTransform;

            // Si ya hay una animación en curso, la para
            if (activeCoroutines.ContainsKey(rt))
            {
                StopCoroutine(activeCoroutines[rt]);
                rt.localScale = Vector3.one;
                activeCoroutines.Remove(rt);
            }

            Coroutine c = StartCoroutine(ScaleHeart(rt));
            activeCoroutines.Add(rt, c);
        }

        yield return null;
    }


    IEnumerator ScaleHeart(RectTransform heartTransform)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * 1.05f;
        float duration = 0.1f;

        float time = 0;
        while (time < duration)
        {
            heartTransform.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        heartTransform.localScale = targetScale;

        time = 0;
        while (time < duration)
        {
            heartTransform.localScale = Vector3.Lerp(targetScale, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        heartTransform.localScale = originalScale;

        // Quitar de diccionario
        activeCoroutines.Remove(heartTransform);
    }

}
