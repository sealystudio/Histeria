using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("Ajustes")]
    public float distanceLimit = 5f;
    public Transform player;

    [Header("Vectores")]
    public Vector3 dir;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 aimInput;

#if UNITY_ANDROID || UNITY_IOS
            // Entrada del joystick derecho móvil
            aimInput = PlayerInputBridge.AimInput;
#else
        // Entrada del mouse en PC
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mousePos3D.x, mousePos3D.y);
        aimInput = mousePos - (Vector2)player.position;
#endif

        // Convertimos a Vector3 para la posición del crosshair
        dir = new Vector3(aimInput.x, aimInput.y, 0f);

        // Limitar la distancia máxima desde el jugador
        if (dir.magnitude > distanceLimit)
            dir = dir.normalized * distanceLimit;

        // Posicionamos el crosshair
        transform.position = player.position + dir;
    }
}
