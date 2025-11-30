using UnityEngine;

[CreateAssetMenu(fileName = "EliCorruptaData", menuName = "DatosEnemigos/EliCorruptaData")]
public class EliCorruptaData : ScriptableObject
{
    public int maxHealth = 3;
    public float moveSpeed = 2f;
    public int damage = 1;
    public float detectionRange = 10f;
    public float attackRange = 3f;

    public GameObject lagrimaPrefab;
    public GameObject hitEffect;
    public GameObject dieEffect;
}
