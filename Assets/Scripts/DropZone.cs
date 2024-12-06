using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [SerializeField] private Vector3 dropOffset;
    public Vector3 DropPosition => transform.position + dropOffset;

    public Color unselectedColour = Color.grey;
    public Color highlightedColour = Color.white;
    public Color selectedColour = Color.yellow;

    public bool scaleDraggables = true;
    public float draggableScale = 1f;

    private Image image;

    // UnityEvent for OnDrop functionality
     [System.Serializable] public class DroppedObjectEvent : UnityEvent<GameObject, GameObject> { }
    public DroppedObjectEvent OnCardDropped;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Draggable.Active != null)
        {
            if (scaleDraggables) Draggable.Active.transform.DOScale (Vector3.one * draggableScale, 0.2f);
            
            Draggable.Active.SetDropZone(this);
            GetComponent<Image>().DOColor(selectedColour, 0.5f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Draggable.Active != null)
        {
            Draggable.Active.SetDropZone(null);
            GetComponent<Image>().DOColor(unselectedColour, 0.5f);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag as GameObject;

        if (droppedObject != null)
        {
            OnCardDropped?.Invoke(droppedObject, this.gameObject);
        }

        Sequence sequence = DOTween.Sequence();
        image.DOKill();
        sequence.Append(image.DOColor(highlightedColour, 0.1f));
        sequence.Append(image.DOColor(unselectedColour, 1f));
        sequence.Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + dropOffset, Vector3.one * 10);
    }
}
