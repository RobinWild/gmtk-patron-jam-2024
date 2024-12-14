using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePrefabManager : MonoBehaviour
{
    [Tooltip("List of resource types and their associated prefabs.")]
    public List<ResourcePrefabPair> resourcePrefabs = new List<ResourcePrefabPair>();

    private void Awake()
    {
        AssignPrefabsToInventory();
    }

    private void AssignPrefabsToInventory()
    {
        foreach (var pair in resourcePrefabs)
        {
            if (pair.prefab == null)
            {
                Debug.LogError($"Prefab for {pair.resourceType} is not assigned.");
                continue;
            }

            Inventory.SetResourcePrefab(pair.resourceType, pair.prefab);
        }
    }
}

[Serializable]
public class ResourcePrefabPair
{
    public Inventory.ResourceType resourceType;
    public GameObject prefab;
}

