using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickEvents : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Action pointerDown;
    public Action<PointerEventData> pointerDrag; 
    public Action pointerUp; 
    
    public void OnPointerDown(PointerEventData eventData)
    {
        //pointerDown.Invoke();
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        pointerDrag.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerUp.Invoke();
    }
}
