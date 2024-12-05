using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
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
        _startPosition = _rect.localPosition;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(_drop);
        Active = this;

        // Convert screen position to local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rect.parent as RectTransform, // Parent RectTransform
            eventData.position,           // Pointer position in screen space
            eventData.pressEventCamera,   // Camera handling the UI
            out Vector2 localPoint        // Resulting local position
        );

        _rect.localPosition = localPoint;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop Zone status: " + _drop);
        
        if (_drop != null)
        {
            Debug.Log("Dropzone");
            _rect.DOLocalMove(_drop.DropPosition, 0.25f).SetEase(Ease.OutBack);
        }
        else
        {
            _rect.DOLocalMove(_startPosition, 0.25f).SetEase(Ease.OutBack);
        }

        Active = null;
    }
}