using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static Draggable Active;
    private static List<Draggable> allDraggables = new List<Draggable>(); // Tracks all draggable objects

    public DropZone _drop;
    private Vector3 _startPosition;
    private RectTransform _rect;

    public GraphicRaycaster nonDropzoneRaycaster;
    private Canvas _canvas;
    private int _originalSortingOrder;

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

        // Add this object to the global list of draggables
        allDraggables.Add(this);
    }

    private void OnDestroy()
    {
        // Remove this object from the global list when destroyed
        allDraggables.Remove(this);
    }

    public void SetDropZone(DropZone drop)
    {
        _drop = drop;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Active = this;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        _startPosition = _rect.position;
        _canvas.overrideSorting = true;
        _originalSortingOrder = _canvas.sortingOrder;
        _canvas.sortingOrder = 100;
        _rect.transform.DORotate(Vector3.zero, 0.2f);

        // Disable raycast blocking for all other draggables
        foreach (var draggable in allDraggables)
        {
            if (draggable != this)
            {
                draggable.GetComponent<CanvasGroup>().blocksRaycasts = false;
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

        _canvas.overrideSorting = false;
        _canvas.sortingOrder = _originalSortingOrder;
        Active = null;
        _rect.DOScale(Vector3.one * 1f, 0.25f).SetEase(Ease.OutBack);

        // Re-enable raycast blocking for all other draggables
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
        _rect.DOScale(Vector3.one * 1.1f, 0.25f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Active != this)
        {
            _rect.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        }
    }

    public void ResetPosition()
    {
        _rect.DOMove(_startPosition, 0.25f).SetEase(Ease.OutBack);
    }
}
