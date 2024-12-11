using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class LandCardController : MonoBehaviour
{
    public List<Worker> workers = new List<Worker>();

    public float cooldown = 10f;
    public float currentTime;
    public DropZone[] myDropzones;

    [System.Serializable]
    public class TaskCompletedEvent : UnityEvent<GameObject> { }
    public TaskCompletedEvent OnTaskCompleted;

    private void Start()
    {
        currentTime = cooldown;
    }

    private void Update()
    {
        if (workers != null && workers.Count > 0)
        {
            HandleWorkers();
            UpdateCooldown();
        }
        else
        {
            ResetCooldown();
        }
    }

    private void HandleWorkers()
    {
        for (int i = workers.Count - 1; i >= 0; i--)
        {
            var worker = workers[i];
            if (worker != null)
            {
                if (!IsWorkerInMyDropzones(worker))
                {
                    RemoveWorker(worker);
                }
            }
        }
    }

    private bool IsWorkerInMyDropzones(Worker worker)
    {
        foreach (var dropZone in myDropzones)
        {
            if (worker.dropZone == dropZone)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateCooldown()
    {
        float totalWorkSpeed = CalculateTotalWorkSpeed();
        currentTime -= Time.deltaTime * totalWorkSpeed;

        if (currentTime <= 0)
        {
            CompleteTask();
        }
    }

    private float CalculateTotalWorkSpeed()
    {
        float totalWorkSpeed = 0f;
        foreach (var worker in workers)
        {
            if (worker != null)
            {
                totalWorkSpeed += worker.workSpeed;
            }
        }
        return totalWorkSpeed;
    }

    private void CompleteTask()
    {
        OnTaskCompleted?.Invoke(gameObject);
        ResetCooldown();
    }

    private void ResetCooldown()
    {
        currentTime = cooldown;
    }

    public void AddWorker(GameObject workerGO, GameObject dropZone)
    {
        Worker worker = workerGO.GetComponent<Worker>();
        if (worker != null && !workers.Contains(worker))
        {
            workers.Add(worker);
            worker.dropZone = dropZone.GetComponent<DropZone>();
        }
    }

    public void RemoveWorker(Worker worker)
    {
        if (workers.Contains(worker))
        {
            workers.Remove(worker);
        }
    }
}
