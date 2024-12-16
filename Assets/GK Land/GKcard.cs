using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class GKcard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public RectTransform fill;
    public GameObject discardVisual;
    public GameObject deactivatedVisual;
    public TMPro.TMP_Text textComponent;

    [System.NonSerialized]
    public GKslots slots;
    [System.NonSerialized]
    public int slotIndex;

    List<CardEffect> effects = new();

    float procProg = 0f;
    float procTime => effects.Aggregate(1f, (res, ce) => res * ce.time_mult);
    
    bool active = true;
    bool discarded = false;
    bool deactivatable => effects.All(ce => ce.deactivatable) && !discarded;
    bool discardable => effects.All(ce => ce.discardable) && !discarded;
    bool movable => effects.All(ce => ce.movable) && !discarded;

    void Start(){
        ConformToSlotPosition(true);
        transform.localScale = Vector3.zero;
        TweenScale(1);
    }

    void Update(){
        if(
            GameTimeController.DeltaTime == 0 ||
            !effects.All(ce => ce.Condition()) ||
            !active ||
            discarded
        ) return;

        procProg += GameTimeController.DeltaTime;
        setFill(procProg / procTime);
        foreach(var ce in effects) ce.Progress(procProg / procTime);

        if(procProg > procTime){
            procProg = 0;
            setFill(0);
            foreach(var ce in effects) ce.Proc();
        }
    }


    #region APIs

    Tween posTween;

    public void ConformToSlotPosition(bool instant = false){
        var correctPos = new Vector3(slots.GetSlotPosition(slotIndex),0,0);

        if(instant) transform.localPosition = correctPos;
        else if(transform.localPosition != correctPos){
            posTween?.Kill();
            posTween = transform.DOLocalMove(correctPos, 0.2f);
        }
    }

    void SetActive(bool to){
        if(!deactivatable && !to) return;
        active = to;
        deactivatedVisual.SetActive(!active);
    }

    void setFill(float val){
        val = 1 - val;
        var o = fill.offsetMax;
        o.y = (fill.parent as RectTransform).sizeDelta.y * -val;
        fill.offsetMax = o;
    }
    
    public void GainResources(string key, int amount){
        GameManager.AddResourceAmount(key, amount);

        for (int i = 0; i < amount; i++)
            SpawnBespokeParticle(key, transform.position, GameManager.instance.incomingParticleTarget.position, 1, i);
    }

    public void SpendResources(string key, int amount){
        GameManager.AddResourceAmount(key, -amount);
        
        for (int i = 0; i < amount; i++)
            SpawnBespokeParticle(key, GameManager.instance.outgoingParticleTarget.position, transform.position, -0.5f, i);
    }

    public void RegisterEffects(params CardEffect[] toRegister){
        foreach(var ce in toRegister) RegisterEffect(ce);
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

        discarded = true;
        slots.SetSlot(slotIndex, null);
        foreach(var ce in effects) DeRegisterEffect(ce, false);
        transform.DOScale(0, 0.25f).OnComplete(() => Destroy(gameObject));
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
            TweenScale(1.1f);
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
        if(discarded) return;
        scaleTween = transform.DOScale(s, 0.1f);
    }

    #endregion


    #region Resource Particles

    private void SpawnBespokeParticle(string key, Vector3 startPos, Vector3 endPos, float arcScalar, int iteration)
    {
        if(!GameManager.instance.prefabDict.ContainsKey(key)) return;

        GameObject particle = Instantiate(GameManager.instance.prefabDict[key], GameManager.instance.canvasRoot);
        var ptransform = particle.transform;

        ptransform.position = startPos;
        ptransform.localScale = Vector3.one;
        ptransform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));

        float moveDuration = 0.5f * (1 + Mathf.Exp(-iteration / 5f));
        float arcHeight = 3f;
        float arcVariance = 0.5f;

        var movexTween = ptransform.DOMoveX(endPos.x, moveDuration).SetEase(Ease.Linear);
        var rotateTween = ptransform.DORotate(
            ptransform.eulerAngles + new Vector3(0f, 0f, Random.Range(-1f, 1f) * 1080),
            3f,
            RotateMode.FastBeyond360
        );

        DOTween.Sequence()
            .Join(
                ptransform.DOMoveY(
                    endPos.y + Random.Range(arcHeight - arcVariance, arcHeight + arcVariance) * arcScalar,
                    moveDuration / 2
                ).SetEase(Ease.OutSine)
            )
            .Append(
                ptransform.DOMoveY(endPos.y, moveDuration / 2).SetEase(Ease.InSine)
            )
            .OnComplete(
                () => {
                    Destroy(particle);
                    // prevent the console from filling with warnings/errors
                    movexTween.Kill();
                    rotateTween.Kill();
                }
            );
    }

    #endregion
}

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
