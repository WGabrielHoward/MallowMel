using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform background;  // assign JoystickBG
    public RectTransform handle;      // assign Handle
    public float handleLimit = 25f;  // max distance handle can move

    private Vector2 input = Vector2.zero;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out pos);

        pos = Vector2.ClampMagnitude(pos, handleLimit);
        handle.anchoredPosition = pos;

        input = pos / handleLimit; // normalize input (-1 to 1)
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
        input = Vector2.zero;
    }

    public Vector2 Direction => input; // expose for PlayerController
}
