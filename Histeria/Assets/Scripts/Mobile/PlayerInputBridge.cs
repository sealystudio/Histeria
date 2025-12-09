using UnityEngine;

public class MobileInputBridge : MonoBehaviour
{
    public static Vector2 MoveJoystick;
    public static Vector2 LookJoystick;

    public static bool MeleePressed;
    public static bool RangedPressed;
    public static bool DashPressed;
    public static bool InteractPressed;
    public static bool InventoryPressed;

    public static void ResetButtons()
    {
        MeleePressed = false;
        RangedPressed = false;
        DashPressed = false;
        InteractPressed = false;
        InventoryPressed = false;
    }

    public void Melee() => MeleePressed = true;
    public void Ranged() => RangedPressed = true;
    public void Dash() => DashPressed = true;
    public void Interact() => InteractPressed = true;
    public void Inventory() => InventoryPressed = true;
}
