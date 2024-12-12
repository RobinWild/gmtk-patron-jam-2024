using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public GameObject dropOffsetObject;
    public Vector3 dropPosition => dropOffsetObject.transform.position;

    public bool scaleDraggables = true;
    public float draggableScale = 1f;

    public Draggable child;

    public bool singleSlot = true;

    [System.Serializable] public class DroppedObjectEvent : UnityEvent<GameObject, GameObject> { }
    public DroppedObjectEvent OnCardDropped;

    private ArrangeTokens arrangeTokens;

    void Start()
    {
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
                Draggable.Active.transform.DOScale(Vector3.one * draggableScale, 0.1f).SetId("ScaleTween");
            }

            Draggable.Active.SetDropZone(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Draggable.Active != null)
        {
            Draggable.Active.SetDropZone(null);
            if (scaleDraggables) Draggable.Active.transform.DOScale(Vector3.one * Draggable.Active.scaleOnPickup, 0.2f).SetId("ScaleTween");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag as GameObject;

        if (droppedObject != null)
        {
            if (singleSlot) CloseSlot();
            if (Draggable.Active._drop != null) OnCardDropped?.Invoke(droppedObject, this.gameObject);
        }

        if (droppedObject.GetComponent<Worker>()) droppedObject.GetComponent<Worker>().dropZone = this;

        // Check if the drop zone has the ArrangeTokens component
        if (arrangeTokens != null)
        {
            arrangeTokens.AddObject(droppedObject.GetComponent<RectTransform>());
        }
    }
}
