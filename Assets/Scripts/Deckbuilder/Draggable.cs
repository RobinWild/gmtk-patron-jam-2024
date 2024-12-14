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

        // Add this object to the global list of draggables
        allDraggables.Add(this);

        _rect.DOScale(Vector3.one * baseScale, 0.25f).SetEase(Ease.OutBack).SetId("ScaleTween" + GetInstanceID());
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
        CheckForDropZone(eventData);
        Active = this;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        _startPosition = _rect.position;
        _canvas.overrideSorting = true;
        _originalSortingOrder = _canvas.sortingOrder;
        _canvas.sortingOrder = 100;
        DOTween.Kill("ScaleTween" + GetInstanceID());
        _rect.DOScale(Vector3.one * scaleOnPickup, 0.25f).SetEase(Ease.OutBack).SetId("ScaleTween" + GetInstanceID());

        // Disable raycast blocking for all other draggables
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
            // Check if the raycast hit a DropZone object
            DropZone dropZone = result.gameObject.GetComponent<DropZone>();
            if (dropZone != null)
            {
                // Assign the first valid DropZone to the _drop variable
                _drop = dropZone;
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
