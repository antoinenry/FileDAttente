using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIAccurateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent Down;
    public UnityEvent Up;

    public void OnPointerDown(PointerEventData eventData)
    {
        Down.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Up.Invoke();
    }

    private void OnDestroy()
    {
        Down.RemoveAllListeners();
        Up.RemoveAllListeners();
    }
}
