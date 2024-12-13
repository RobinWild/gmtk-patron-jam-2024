using System;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory
{
    public enum ResourceType
    {
        Gold,
        BuildingMaterials,
        TradeGoods,
        Corpses,
        Military
    }

    private static readonly Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private static readonly Dictionary<ResourceType, int> maxResources = new Dictionary<ResourceType, int>();
    private static readonly Dictionary<ResourceType, Action> resourceChangeListeners = new Dictionary<ResourceType, Action>();

    // Dictionary to associate each ResourceType with a prefab
    private static readonly Dictionary<ResourceType, GameObject> resourcePrefabs = new Dictionary<ResourceType, GameObject>();

    static Inventory()
    {
        foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
        {
            resources[resource] = 0;
            maxResources[resource] = int.MaxValue;
            resourceChangeListeners[resource] = null;
            resourcePrefabs[resource] = null; // Initialize with null prefabs
        }
    }

    public static void AddResource(ResourceType resource, int amount)
    {
        if (amount < 0) throw new ArgumentException("Amount to add cannot be negative.");

        resources[resource] = Math.Min(resources[resource] + amount, maxResources[resource]);

        NotifyResourceChange(resource);
    }

    public static void RemoveResource(ResourceType resource, int amount)
    {
        if (amount < 0) throw new ArgumentException("Amount to remove cannot be negative.");

        if (resources[resource] < amount)
        {
            return;
        }

        resources[resource] -= amount;

        NotifyResourceChange(resource);
    }

    public static int GetResource(ResourceType resource) => resources[resource];

    public static void SetResource(ResourceType resource, int amount)
    {
        if (amount < 0) throw new ArgumentException("Resource amount cannot be negative.");
        resources[resource] = Math.Min(amount, maxResources[resource]);

        NotifyResourceChange(resource);
    }

    public static void SetMaxResource(ResourceType resource, int maxAmount)
    {
        if (maxAmount < 0) throw new ArgumentException("Max resource amount cannot be negative.");
        maxResources[resource] = maxAmount;
    }

    public static void ResetInventory()
    {
        foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
        {
            resources[resource] = 0;
            NotifyResourceChange(resource);
        }
    }

    public static void RegisterResourceThresholdListener(ResourceType resource, int threshold, Action callback)
    {
        Action listener = () =>
        {
            if (resources[resource] >= threshold)
            {
                callback?.Invoke();
            }
        };

        resourceChangeListeners[resource] += listener;
        listener.Invoke();
    }

    public static void SetResourcePrefab(ResourceType resource, GameObject prefab)
    {
        if (prefab == null) throw new ArgumentException("Prefab cannot be null.");
        resourcePrefabs[resource] = prefab;
    }

    public static GameObject GetResourcePrefab(ResourceType resource)
    {
        if (!resourcePrefabs.ContainsKey(resource) || resourcePrefabs[resource] == null)
        {
            throw new Exception($"No prefab set for resource type {resource}.");
        }

        return resourcePrefabs[resource];
    }

    private static void NotifyResourceChange(ResourceType resource)
    {
        resourceChangeListeners[resource]?.Invoke();
    }
}
