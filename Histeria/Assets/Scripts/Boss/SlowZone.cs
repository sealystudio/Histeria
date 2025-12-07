using UnityEngine;

public class SlowZone : MonoBehaviour
{
    public float slowAmount = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerMovement>().moveSpeed *= slowAmount;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerMovement>().moveSpeed /= slowAmount;
    }
}
