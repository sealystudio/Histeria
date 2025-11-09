using UnityEngine;

//Con esta clase que hereda de scriptable object, podemos crear un asset de cada enemigo sin cambiar stats o código, es como si fuera el prefab
//Esto no define comportamiento, solo “cómo es” el enemigo


[CreateAssetMenu(fileName = "SombraAbandonoData", menuName = "DatosEnemigos/SombraAbandonoData")]
public class SombraAbandonoData : ScriptableObject
{
    public int maxHealth = 3;
    public float moveSpeed = 1.5f;
    public int damage = 1;
    public GameObject hitEffect;
    public GameObject dieEffect;
}
