using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static DraggableCard Active;
    private static List<DraggableCard> allDraggables = new List<DraggableCard>();

    public CardSlot _drop;
    private Vector3 _startPosition;
    private RectTransform _rect;

    private Canvas _canvas;
    private int _originalSortingOrder;

    public float baseScale = 0.5f;
    public float scaleOnPickup = 1.4f;
    public float scaleOnHover = 1.1f;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponent<Canvas>();

        if (_canvas == null)
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.overrideSorting = true;
        }

        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        _originalSortingOrder = _canvas.sortingOrder;

        allDraggables.Add(this);

        _rect.DOScale(Vector3.one * baseScale, 0.25f).SetEase(Ease.OutBack).SetId("ScaleTween" + GetInstanceID());
    }

    private void OnDestroy()
    {
        allDraggables.Remove(this);
    }

    public void SetDropZone(CardSlot cardSlot)
    {
        _drop = cardSlot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CheckForDropZone(eventData);
        Active = this;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        _startPosition = _rect.position;
        _canvas.overrideSorting = true;
        _originalSortingOrder = _canvas.sortingOrder;
        _canvas.sortingOrder = 100;
        DOTween.Kill("ScaleTween" + GetInstanceID());
        _rect.DOScale(Vector3.one * scaleOnPickup, 0.25f).SetEase(Ease.OutBack).SetId("ScaleTween" + GetInstanceID());

        CardSlot parentCardSlot = transform.GetComponentInParent<CardSlot>();
        if (parentCardSlot){
            parentCardSlot.OpenSlot();
        }

        foreach (var draggable in allDraggables)
        {
            if (draggable != this)
            {
                draggable.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
    }

    public void CheckForDropZone(PointerEventData eventData)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (var result in raycastResults)
        {
            CardSlot cardSlot = result.gameObject.GetComponent<CardSlot>();
            if (cardSlot != null)
            {
                _drop = cardSlot;
                break;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        Vector3 targetPosition = _rect.parent.TransformPoint(localPoint);
        _rect.DOMove(targetPosition, 0.2f).SetEase(Ease.OutQuad);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (_drop == null)
        {
            ResetPosition();
        }

        _canvas.sortingOrder = _originalSortingOrder;
        Active = null;
        DOTween.Kill("ScaleTween" + GetInstanceID());
        _rect.DOScale(Vector3.one * baseScale, 0.25f).SetEase(Ease.OutBack).SetId("ScaleTween" + GetInstanceID());

        foreach (var draggable in allDraggables)
        {
            if (draggable != this)
            {
                draggable.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DOTween.Kill("ScaleTween" + GetInstanceID());
        _rect.DOScale(Vector3.one * scaleOnHover, 0.25f).SetEase(Ease.OutBack).SetId("ScaleTween" + GetInstanceID());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Active != this)
        {
            DOTween.Kill("ScaleTween" + GetInstanceID());
            _rect.DOScale(Vector3.one * baseScale, 0.25f).SetEase(Ease.OutBack).SetId("ScaleTween" + GetInstanceID());
        }
    }

    public void ResetPosition()
    {
        _rect.DOMove(_startPosition, 0.25f).SetEase(Ease.OutBack);
    }
}
