using UnityEngine;

public class BossWorldInteraction : MonoBehaviour
{
    public GameObject slowZonePrefab;

    // Ya no decidimos aquí el cooldown, solo ejecutamos
    public void DropHazard()
    {
        Instantiate(slowZonePrefab, transform.position, Quaternion.identity);
        Debug.Log("IA: Marca ralentizante colocada.");
    }
}