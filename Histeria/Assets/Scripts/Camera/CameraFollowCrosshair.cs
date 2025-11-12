using UnityEngine;

public class CameraFollowCrosshair : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Transform crosshair;

    [Header("Ajustes")]
    public float followSmoothTime = 0.15f; // suavizado
    public float offsetFactor = 0.3f;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (player == null || crosshair == null) return;

        Vector3 offset = (crosshair.position - player.position) * offsetFactor;

        Vector3 targetPos = player.position + offset;
        targetPos.z = transform.position.z;

        //suavizado
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followSmoothTime);
    }
}
