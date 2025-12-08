using UnityEngine;

public class PlayerInputBridge : MonoBehaviour
{
    public static Vector2 MoveInput;
    public static Vector2 AimInput;

    public static bool MeleePressed;
    public static bool RangedPressed;
    public static bool PickupPressed;
    public static bool InventoryPressed;

    public SimpleJoystick moveJoystick;
    public SimpleJoystick aimJoystick;

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        MoveInput = moveJoystick.GetInput();
        AimInput = aimJoystick.GetInput();
#endif
    }

    public void OnMelee() => MeleePressed = true;
    public void OnRanged() => RangedPressed = true;
    public void OnPickup() => PickupPressed = true;
    public void OnInventory() => InventoryPressed = true;

    void LateUpdate()
    {
        MeleePressed = false;
        RangedPressed = false;
        PickupPressed = false;
        InventoryPressed = false;
    }
}
