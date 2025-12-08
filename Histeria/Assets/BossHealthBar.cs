using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))] // Obliga a que haya un CanvasGroup
public class BossHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public BossController boss;
    public Slider slider;
    private CanvasGroup canvasGroup;

    private bool isBossActive = false; 

    void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (boss == null) boss = FindObjectOfType<BossController>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (boss != null && slider != null)
        {
            slider.maxValue = boss.maxHP;
            slider.value = boss.currentHP;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f; // Invisible
        }
    }

    void Update()
    {
        if (boss == null || slider == null) return;

        slider.value = boss.currentHP;

        if (!isBossActive)
        {
            if (boss.perception.player != null)
            {
                float distance = Vector3.Distance(boss.transform.position, boss.perception.player.position);

                if (distance <= boss.detectionRange)
                {
                    ShowHealthBar();
                }
            }
        }

        if (boss.currentHP <= 0)
        {
            canvasGroup.alpha = 0f;
        }
    }

    void ShowHealthBar()
    {
        isBossActive = true;
        canvasGroup.alpha = 1f; 
    }
}