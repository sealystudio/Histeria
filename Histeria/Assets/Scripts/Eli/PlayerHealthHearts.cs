using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class PlayerHealthHearts : MonoBehaviour
{
    [Header("Vida")]
    public int maxHearts = 3;
    public int currentHealth; // Cada corazón = 2 puntos

    [Header("Referencias UI")]
    public Image heartPrefab;
    public Sprite fullHeart, halfHeart, emptyHeart;

    private readonly List<Image> hearts = new List<Image>();

    void OnEnable()
    {
        if (heartPrefab == null) return;

        // Si no se está jugando, aseguramos la vista de edición
        if (!Application.isPlaying)
        {
            currentHealth = maxHearts * 2;
            RefreshHeartsInEditor();
        }
    }

    void OnValidate()
    {
        if (heartPrefab == null) return;

        // Clamp de valores y vista previa en editor
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHearts * 2);
        RefreshHeartsInEditor();
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            currentHealth = maxHearts * 2;
            CreateHearts();
            UpdateHearts();
        }
    }

    // --------------------
    // FUNCIONES PRINCIPALES
    // --------------------

    private void RefreshHeartsInEditor()
    {
        // Borra los corazones antiguos solo si hay duplicados o falta algo
        if (transform.childCount != maxHearts)
        {
            foreach (Transform child in transform)
                DestroyImmediate(child.gameObject);
            hearts.Clear();

            for (int i = 0; i < maxHearts; i++)
            {
                Image heart = Instantiate(heartPrefab, transform);
                heart.transform.localScale = Vector3.one;
                hearts.Add(heart);
            }
        }

        UpdateHearts();
    }

    void CreateHearts()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            Image heart = Instantiate(heartPrefab, transform);
            heart.transform.localScale = Vector3.one;
            hearts.Add(heart);
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Max(currentHealth - dmg, 0);
        UpdateHearts();
        if (Application.isPlaying)
            StartCoroutine(AnimateHearts());
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHearts * 2);
        UpdateHearts();
        if (Application.isPlaying)
            StartCoroutine(AnimateHearts());
    }

    void UpdateHearts()
    {
        if (hearts.Count == 0) return;

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



    // --------------------
    // ANIMACIÓN (solo en juego)
    // --------------------

    private Dictionary<RectTransform, Coroutine> activeCoroutines = new Dictionary<RectTransform, Coroutine>();

    IEnumerator AnimateHearts()
    {
        foreach (Image heart in hearts)
        {
            RectTransform rt = heart.rectTransform;

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
        Vector3 targetScale = originalScale * 1.2f;
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

        activeCoroutines.Remove(heartTransform);
    }
}
