using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [Header("Configuración")]
    public Transform player;

    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;
    public float zOffset = -17f;
    public Vector2 offsetXY;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = new Vector3(
            player.position.x + offsetXY.x,
            player.position.y + offsetXY.y,
            zOffset
        );

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}