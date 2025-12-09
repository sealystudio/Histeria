using UnityEngine;

public class MobileJoystickReader : MonoBehaviour
{
    public SimpleJoystick joystickLeft;
    public SimpleJoystick joystickRight;

    void Update()
    {
        if (Application.isMobilePlatform)
        {
            MobileInputBridge.MoveJoystick = joystickLeft.GetInput();
            MobileInputBridge.LookJoystick = joystickRight.GetInput();
        }
    }
}
