using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GKcard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public GKslots slots;
    public int slotIndex;

    public RectTransform fill;
    
    bool active = true;

    float procTime = 2f;
    float procProg = 0f;
    public event Action proc;

    void Update(){
        if (GameTimeController.DeltaTime == 0 || !active) return;
        procProg += GameTimeController.DeltaTime;
        setFill(procProg / procTime);
        if(procProg > procTime){
            procProg = 0;
            proc?.Invoke();
        }
    }

    #region APIs

    Tween posTween;

    public void ConformToSlotPosition(){
        if(posTween != null) posTween.Kill();

        var correctPos = new Vector3(slots.GetSlotPosition(slotIndex),0,0);
        if(transform.localPosition != correctPos){
            TweenScale(1f);
            var img = GetComponent<Image>();
            img.raycastTarget = false;
            posTween = transform.DOLocalMove(correctPos, 0.2f).OnComplete(() => img.raycastTarget = true);
        }
    }

    void ToggleActive(){
        // todo: visuals
        active = !active;
    }

    void setFill(float val){
        val = 1 - val;
        var o = fill.offsetMax;
        o.y = (fill.parent as RectTransform).sizeDelta.y * -val;
        fill.offsetMax = o;
    }

    #endregion

    #region Mouse Handling

    Vector3 dragStartPos;
    Vector3 mouseDragStartPos;
    Tween scaleTween;

    public void OnPointerClick(PointerEventData e){
        if(e.button == PointerEventData.InputButton.Right) ToggleActive();
    }
    public void OnBeginDrag(PointerEventData e){
        transform.SetAsLastSibling();
        dragStartPos = transform.position;
        mouseDragStartPos = e.pointerCurrentRaycast.worldPosition;
        TweenScale(1.3f);
    }
    public void OnDrag(PointerEventData e){
        transform.position = dragStartPos + (e.pointerCurrentRaycast.worldPosition - mouseDragStartPos);
    }
    public void OnEndDrag(PointerEventData e){
        slots.MoveSlot(slotIndex, slots.PosToIndex(transform.localPosition.x));
        ConformToSlotPosition();
    }
    public void OnPointerEnter(PointerEventData e){
        TweenScale(1.1f);
    }
    public void OnPointerExit(PointerEventData e){
        TweenScale(1);
    }

    void TweenScale(float s){
        if(scaleTween != null) scaleTween.Kill();
        scaleTween = transform.DOScale(s, 0.1f);
    }

    #endregion
}
