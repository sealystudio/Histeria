using UnityEngine;

public class MobileInputSimulator : MonoBehaviour
{
    void Update()
    {
        // Simula el joystick izquierdo (movimiento)
        MobileInputBridge.MoveJoystick = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Simula el joystick derecho (apuntar)
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        MobileInputBridge.LookJoystick = mouseDelta;

        // Simula los botones
        MobileInputBridge.MeleePressed = Input.GetMouseButton(0);  // clic izquierdo
        MobileInputBridge.RangedPressed = Input.GetMouseButton(1); // clic derecho
        MobileInputBridge.InteractPressed = Input.GetKey(KeyCode.F); // F = recoger
        MobileInputBridge.InventoryPressed = Input.GetKey(KeyCode.E); // E = abrir/cerrar inventario
        MobileInputBridge.DashPressed = Input.GetKey(KeyCode.Space); // Space = dash
    }
}
