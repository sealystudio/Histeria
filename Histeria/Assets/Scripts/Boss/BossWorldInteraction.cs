using UnityEngine;

public class BossWorldInteraction : MonoBehaviour
{
    public GameObject slowZonePrefab;
    private float dropCooldown = 5f;
    private float lastDrop = 0f;

    public void DropHazard()
    {
        if (Time.time - lastDrop > dropCooldown)
        {
            Instantiate(slowZonePrefab, transform.position, Quaternion.identity);
            Debug.Log("Marca que ralentiza al jugador colocada.");
            lastDrop = Time.time;
        }
    }
}
