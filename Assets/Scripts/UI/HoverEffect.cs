using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;
using UnityEngine.Events;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;

    public void OnPointerEnter(PointerEventData eventData) {
        onHoverEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData) {
        onHoverExit.Invoke();
    }
}