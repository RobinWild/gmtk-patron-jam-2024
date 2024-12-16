using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class GKcard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    // todo: deactivation visuals
    public RectTransform fill;
    public GameObject discardVisual;

    [NonSerialized]
    public GKslots slots;
    [NonSerialized]
    public int slotIndex;

    List<CardEffect> effects = new();

    float procProg = 0f;
    float procTime => effects.Aggregate(1f, (res, ce) => res * ce.time_mult);
    
    bool active = true;
    bool deactivatable => effects.All(ce => ce.deactivatable);
    bool discardable => effects.All(ce => ce.discardable);
    bool movable => effects.All(ce => ce.movable);

    public abstract class CardEffect {
        public GKcard card;
        public float time_mult = 1f;
        public bool movable = true;
        public bool discardable = true;
        public bool deactivatable = true;

        public virtual void Register(){}
        public virtual void DeRegister(){}

        public virtual bool Condition() => true;
        public virtual void Progress(float t){}
        public virtual void Proc(){}
    }

    class TestEffect : CardEffect {
        public override void Proc(){
            Debug.Log("proc");
            GameManager.AddResourceAmount("gold", 5);
        }

        public TestEffect(){
            time_mult = 0.5f;
        }
    }

    void Start(){
        RegisterEffect(new TestEffect());
    }

    void Update(){
        if(
            GameTimeController.DeltaTime == 0 ||
            !effects.All(ce => ce.Condition())
        ) return;

        procProg += GameTimeController.DeltaTime;
        setFill(procProg / procTime);
        foreach(var ce in effects) ce.Progress(procProg / procTime);

        if(procProg > procTime){
            procProg = 0;
            if(active) foreach(var ce in effects) ce.Proc();
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

    void SetActive(bool to){
        if(!deactivatable && !to) return;
        active = to;
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

    public void DeRegisterEffect(CardEffect effect, bool removeFromList = true){
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
        // need to be able to enumerate the list fully when discarding, so can't modify the list at the same time
        if(removeFromList) effects.Remove(effect);
    }

    public void Discard(){
        if(!discardable){
            discardVisual.SetActive(false);
            ConformToSlotPosition();
            return;
        }
        foreach(var ce in effects) DeRegisterEffect(ce, false);
        slots.SetSlot(slotIndex, null);
        Destroy(gameObject);
    }

    #endregion


    #region Mouse Handling

    Vector3 dragStartPos;
    Vector3 mouseDragStartPos;
    Tween scaleTween;

    public void OnPointerClick(PointerEventData e){
        if(e.button == PointerEventData.InputButton.Left && !movable){
            // todo: visual indicator
        }
        if(e.button == PointerEventData.InputButton.Right) SetActive(!active);
    }
    public void OnBeginDrag(PointerEventData e){
        if(!movable) return;
        transform.SetAsLastSibling();
        dragStartPos = transform.position;
        mouseDragStartPos = e.pointerCurrentRaycast.worldPosition;

        posTween?.Kill();
        TweenScale(1.3f);
    }
    public void OnDrag(PointerEventData e){
        if(!movable) return;
        transform.position = dragStartPos + (e.pointerCurrentRaycast.worldPosition - mouseDragStartPos);
        
        discardVisual.SetActive(slots.PosToIndex(transform.localPosition.x) > GKslots.NUM_SLOTS);
    }
    public void OnEndDrag(PointerEventData e){
        if(!movable) return;

        var i = slots.PosToIndex(transform.localPosition.x);

        if(i > GKslots.NUM_SLOTS) Discard();
        else {
            slots.MoveSlot(slotIndex, i);
            ConformToSlotPosition();
        }
    }
    public void OnPointerEnter(PointerEventData e){
        if(!movable) return;
        TweenScale(1.1f);
    }
    public void OnPointerExit(PointerEventData e){
        if(!movable) return;
        TweenScale(1);
    }

    void TweenScale(float s){
        scaleTween?.Kill();
        scaleTween = transform.DOScale(s, 0.1f);
    }

    #endregion
}
