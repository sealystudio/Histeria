using UnityEngine;
using System.Collections;

public enum BossPhase { Oleada, Histeria, PreAutoDestruccion, Autodestruccion }

public class BossController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP = 1000;
    public float currentHP;

    [Header("Fases")]
    public BossPhase phase;

    [Header("Referencias")]
    public BossActions actions;
    public BossWorldInteraction world;
    public BossPerception perception;

    private float autoDestructionTimer = 15f;
    private float timeWithoutDamage = 0f;

    void Start()
    {
        currentHP = maxHP;
        phase = BossPhase.Oleada;
    }

    void Update()
    {
        EvaluatePhase();
        RunPhaseBehaviour();
    }

    public void TakeDamage(float dmg)
    {
        if (phase == BossPhase.PreAutoDestruccion)
        {
            Debug.Log("Es invencible durante cuenta regresiva.");
            return;
        }

        currentHP -= dmg;
        timeWithoutDamage = 0f; // Reinicia contador de autodestrucción
    }

    void EvaluatePhase()
    {
        float hpPercent = currentHP / maxHP * 100;

        if (hpPercent > 50 || hpPercent < 10)
            phase = BossPhase.Oleada;

        if (hpPercent <= 50 && hpPercent >= 10)
            phase = BossPhase.Histeria;

        if (hpPercent < 10)
            phase = BossPhase.PreAutoDestruccion;
    }

    void RunPhaseBehaviour()
    {
        switch (phase)
        {
            case BossPhase.Oleada:
                actions.BasicAttack();
                actions.TrySpawnMinionEvery10Sec();
                break;

            case BossPhase.Histeria:
                actions.SpecialAttack();
                world.DropHazard();
                break;

            case BossPhase.PreAutoDestruccion:
                HandleAutoDestructionCountdown();
                break;

            case BossPhase.Autodestruccion:
                actions.Explode();
                break;
        }
    }

    void HandleAutoDestructionCountdown()
    {
        timeWithoutDamage += Time.deltaTime;

        if (timeWithoutDamage >= autoDestructionTimer)
        {
            phase = BossPhase.Autodestruccion;
        }
    }
}
