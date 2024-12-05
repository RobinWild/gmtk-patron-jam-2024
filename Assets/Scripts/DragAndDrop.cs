using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static DragAndDrop Active;
    
    private DropZone _drop;
    private Vector3 _startPosition;
    private RectTransform _rect;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void SetDropZone(DropZone drop)
    {
        _drop = drop;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        _startPosition = _rect.localPosition;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Active = this;

        // Convert screen position to local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rect.parent as RectTransform, // Parent RectTransform
            eventData.position,           // Pointer position in screen space
            eventData.pressEventCamera,   // Camera handling the UI
            out Vector2 localPoint        // Resulting local position
        );

        // Tween to the desired position
        Vector3 targetPosition = _rect.parent.TransformPoint(localPoint); // Convert localPoint to world position
        _rect.DOMove(targetPosition, 0.2f).SetEase(Ease.OutQuad); // Smoothly move to target position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (_drop != null)
        {
            _rect.DOLocalMove(_drop.DropPosition, 0.25f).SetEase(Ease.OutBack);
        }
        else
        {
            _rect.DOLocalMove(_startPosition, 0.25f).SetEase(Ease.OutBack);
        }

        Active = null;
    }

}