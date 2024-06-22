using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;


public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public UnityEvent onClick;

    RectTransform rtBody;

    void Start()
    {
        rtBody = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rtBody.DOScale(0.9f, 0.1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rtBody.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
    }
}