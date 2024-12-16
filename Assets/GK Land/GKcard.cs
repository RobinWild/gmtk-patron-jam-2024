using System;
using System.Collections.Generic;
using System.Linq;
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

    List<CardEffect> effects = new();

    public abstract class CardEffect {
        public GKcard card;

        public virtual void Register(){}
        public virtual void DeRegister(){}

        public virtual bool Condition() => true;
        public virtual void Progress(float t){}
        public virtual void Proc(){}
    }

    class TestEffect : CardEffect {
        public override void Proc(){
            Debug.Log("proc");
        }
    }

    void Start(){
        RegisterEffect(new TestEffect());
    }

    void Update(){
        if(
            GameTimeController.DeltaTime == 0 ||
            !effects.All(ce => ce.Condition()) ||
            !active
        ) return;

        procProg += GameTimeController.DeltaTime;
        setFill(procProg / procTime);
        foreach(var ce in effects) ce.Progress(procProg / procTime);

        if(procProg > procTime){
            procProg = 0;
            foreach(var ce in effects) ce.Proc();
        }
    }


    #region APIs

    Tween posTween;

    public void ConformToSlotPosition(){
        var correctPos = new Vector3(slots.GetSlotPosition(slotIndex),0,0);

        if(transform.localPosition != correctPos){
            posTween?.Kill();
            posTween = transform.DOLocalMove(correctPos, 0.2f);
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

    public void RegisterEffect(CardEffect effect){
        if(effects.Contains(effect)){
            Debug.LogWarning("Effect already registered on this card");
            return;
        }
        if(effect.card != null){
            Debug.LogWarning("Effect already registered on another card");
            return;
        }
        effect.card = this;
        effect.Register();
        effects.Add(effect);
    }

    public void DeRegisterEffect(CardEffect effect){
        if(effect.card == null){
            Debug.LogWarning("Effect already deregistered");
            return;
        }
        else if(effect.card != this){
            Debug.LogWarning("Effect registered on another card, cannot be deregistered from this one");
            return;
        }
        effect.DeRegister();
        effect.card = null;
        effects.Remove(effect);
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

        posTween?.Kill();
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
        scaleTween?.Kill();
        scaleTween = transform.DOScale(s, 0.1f);
    }

    #endregion
}
