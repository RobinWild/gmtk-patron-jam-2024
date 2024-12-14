using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using System.ComponentModel;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public bool activeSlot = false;
    public bool scaleDraggables = true;
    public float draggableScale = 1f;

    public DraggableCard child;

    public bool singleSlot = true;

    [System.Serializable] public class DroppedObjectEvent : UnityEvent<GameObject, GameObject> { }
    public DroppedObjectEvent OnCardDropped;

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
        if (DraggableCard.Active != null)
        {
            if (scaleDraggables)
            {
                DOTween.Kill("ScaleTween");
                DraggableCard.Active.transform.DOScale(Vector3.one * draggableScale, 0.2f).SetId("ScaleTween");
            }

            DraggableCard.Active.SetDropZone(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DraggableCard.Active != null)
        {
            DraggableCard.Active.SetDropZone(null);
            if (scaleDraggables) DraggableCard.Active.transform.DOScale(Vector3.one * DraggableCard.Active.scaleOnPickup, 0.2f).SetId("ScaleTween");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag as GameObject;

        if (droppedObject != null)
        {
            droppedObject.transform.DOMove(transform.position, 0.2f);
            droppedObject.transform.SetParent(transform);
            CloseSlot();
            if (DraggableCard.Active._drop != null) OnCardDropped?.Invoke(droppedObject, this.gameObject);
        }
    }
}
