using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [SerializeField] private Vector3 dropOffset;
    public GameObject dropOffsetObject;
    public Vector3 dropPosition => transform.position + dropOffset;

    public Color unselectedColour = Color.grey;
    public Color highlightedColour = Color.white;
    public Color selectedColour = Color.yellow;

    public bool scaleDraggables = true;
    public float draggableScale = 1f;

    private Image image;
    public Draggable child;

    public bool singleSlot = true;

    [System.Serializable] public class DroppedObjectEvent : UnityEvent<GameObject, GameObject> { }
    public DroppedObjectEvent OnCardDropped;

    private ArrangeTokens arrangeTokens;

    void Start()
    {
        image = GetComponent<Image>();
        arrangeTokens = GetComponentInParent<ArrangeTokens>(); // Assuming ArrangeTokens is on the parent object
    }

    public void OpenSlot()
    {
        GetComponent<Image>().raycastTarget = true;
    }

    public void CloseSlot()
    {
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Draggable.Active != null)
        {
            if (scaleDraggables)
            {
                DOTween.Kill("ScaleTween");
                Draggable.Active.transform.DOScale(Vector3.one * draggableScale, 0.2f).SetId("ScaleTween");
            }

            Draggable.Active.SetDropZone(this);
            GetComponent<Image>().DOColor(selectedColour, 0.5f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Draggable.Active != null)
        {
            Draggable.Active.SetDropZone(null);
            if (scaleDraggables) Draggable.Active.transform.DOScale(Vector3.one * Draggable.Active.scaleOnPickup, 0.2f).SetId("ScaleTween");
            GetComponent<Image>().DOColor(unselectedColour, 0.5f);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag as GameObject;

        if (droppedObject != null)
        {
            if (singleSlot) CloseSlot();
            OnCardDropped?.Invoke(droppedObject, this.gameObject);
        }

        Sequence sequence = DOTween.Sequence();
        image.DOKill();
        sequence.Append(image.DOColor(highlightedColour, 0.1f));
        sequence.Append(image.DOColor(unselectedColour, 1f));
        sequence.Play();

        droppedObject.GetComponent<Worker>().dropZone = this;

        // Check if the drop zone has the ArrangeTokens component
        if (arrangeTokens != null)
        {
            arrangeTokens.AddObject(droppedObject.GetComponent<RectTransform>());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + dropOffset, Vector3.one * 0.1f);
    }
}
