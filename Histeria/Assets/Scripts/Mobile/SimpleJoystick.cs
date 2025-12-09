using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform background;
    public RectTransform handle;

    public float joystickRadius = 90f;
    public Vector2 input;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        pos = Vector2.ClampMagnitude(pos, joystickRadius);
        handle.anchoredPosition = pos;

        input = pos / joystickRadius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    public Vector2 GetInput()
    {
        return input;
    }
}
