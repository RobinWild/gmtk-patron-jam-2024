using UnityEngine;
using UnityEngine.Events;

public class CardController : MonoBehaviour
{
    public float taskDuration = 1f;
    public float taskProgress = 0f;

    [System.Serializable]
    public class CardReadyToStartEvent : UnityEvent<GameObject> { }
    public CardReadyToStartEvent OnCardReadyToStart;

    [System.Serializable]
    public class CardReadyToActEvent : UnityEvent<GameObject> { }
    public CardReadyToActEvent OnCardReadyToAct;

    [System.Serializable]
    public class CardActionEvent : UnityEvent<GameObject> { }
    public CardActionEvent OnCardAction;

    [System.Serializable]
    public class CardInterruptedEvent : UnityEvent<GameObject> { }
    public CardInterruptedEvent OnCardInterrupted;

    public bool isTaskReadyToProgress = false;
    public bool shouldActionTrigger = false;

    void Update()
    {
        if (GameTimeController.DeltaTime == 0) return;
        if (IsCardSlotActive())
        {
            HandleTaskProgress();
        }
        else
        {
            if (taskProgress > 0f)
            {
                OnCardInterrupted?.Invoke(gameObject);
            }
            ResetCooldown();
        }

        if (taskProgress >= taskDuration) OnCardReadyToAct?.Invoke(gameObject);


        if (taskProgress >= taskDuration && shouldActionTrigger)
        {
            CompleteTask();
        }
    }

    private bool IsCardSlotActive()
    {
        var cardSlot = transform.parent.GetComponent<CardSlot>();
        return cardSlot != null && cardSlot.activeSlot;
    }

    private void HandleTaskProgress()
    {
        if (isTaskReadyToProgress)
        {
            taskProgress += GameTimeController.DeltaTime;
        }
        else
        {
            OnCardReadyToStart?.Invoke(gameObject);
        }
    }

    private void CompleteTask()
    {
        OnCardAction?.Invoke(gameObject);
        ResetCooldown();
    }

    public void ResetCooldown()
    {
        isTaskReadyToProgress = false;
        taskProgress = 0f;
        shouldActionTrigger = false;
    }

    public void SetTaskReady()
    {
        isTaskReadyToProgress = true;
    }

    public void GrantApproval()
    {
        shouldActionTrigger = true;
    }
}
