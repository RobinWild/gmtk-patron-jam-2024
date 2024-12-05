using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    // Event triggered when all objects are finished acting
    public event Action OnAllActionsCompleted;

    // List to keep track of objects currently acting
    public List<GameObject> actingObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this to notify the TurnManager that an object has started acting
    public void NotifyActionStarted(GameObject actingObject)
    {
        if (!actingObjects.Contains(actingObject))
        {
            actingObjects.Add(actingObject);
        }
    }

    // Call this to notify the TurnManager that an object has finished acting
    public void NotifyActionCompleted(GameObject actingObject)
    {
        if (actingObjects.Contains(actingObject))
        {
            actingObjects.Remove(actingObject);

            // Check if all actions are completed
            if (actingObjects.Count == 0)
            {
                OnAllActionsCompleted?.Invoke();
            }
        }
    }
}
