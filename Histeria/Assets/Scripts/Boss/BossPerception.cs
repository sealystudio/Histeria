using UnityEngine;

public class BossPerception : MonoBehaviour
{
    public Transform player;

    public bool PlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.position) <= range;
    }
}
