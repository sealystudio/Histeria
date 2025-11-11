
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
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Heal(1);
        }
        if(currentHealth == 0)
        {
            Die();
        }

    }
    public void TakeDamage(int amount = 1)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHearts();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
