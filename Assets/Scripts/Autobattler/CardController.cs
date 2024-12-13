using UnityEngine;
using UnityEngine.Events;

public class CardController : MonoBehaviour
{
    public float taskDuration = 1f;
    public float taskProgress = 0f;

    [System.Serializable]
    public class TaskCompletedEvent : UnityEvent<GameObject> { }
    public TaskCompletedEvent OnTaskCompleted;

    [System.Serializable]
    public class TaskStartedEvent : UnityEvent<GameObject> { }
    public TaskStartedEvent OnAttemptedTaskStart;

    [System.Serializable]
    public class TaskInterruptedEvent : UnityEvent<GameObject> { } // New event for interruption
    public TaskInterruptedEvent OnTaskInterrupted;

    public bool isTaskReady = false;  // Flag to track if the task can start

    void Update()
    {
        if (IsCardSlotActive())
        {
            HandleTaskProgress();
        }
        else
        {
            if (taskProgress > 0f)  // Only trigger interruption if task progress has started
            {
                InterruptTask();
            }
            ResetCooldown();
        }

        if (taskProgress >= taskDuration)
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
        if (isTaskReady)
        {
            taskProgress += GameTimeController.DeltaTime;
        }
        else
        {
            AttemptToStartTask();
        }
    }

    private void CompleteTask()
    {
        OnTaskCompleted?.Invoke(gameObject);
        ResetCooldown();
    }

    private void AttemptToStartTask()
    {
        OnAttemptedTaskStart?.Invoke(gameObject);
    }

    private void InterruptTask()
    {
        OnTaskInterrupted?.Invoke(gameObject); // Trigger the interruption event
        Debug.Log("Task was interrupted due to the card slot being inactive.");
    }

    public void ResetCooldown()
    {
        isTaskReady = false;
        taskProgress = 0f;
    }
}
