using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // todo: requests, card gen

    public Transform counters;

    public static GameManager instance;

    void Awake(){
        if(instance != null) Debug.LogError("uh oh");
        instance = this;

        foreach(var key in new string[]{"gold", "wood", "food", "military", "reputation", "revolution"}) SetResourceAmount(key, 0);
    }

    void Start(){
        inventoryChange += (key, amount) => {
            var t = counters.Find(key);
            if(t != null) t.GetComponentInChildren<TMPro.TMP_Text>().text = amount.ToString();
        };
    }

    void Update()
    {
        GameTimeController.Update(Time.deltaTime);
    }

    #region Inventory

    static Dictionary <string, int> inventoryData = new();
    public static event Action<string, int> inventoryChange;

    public static int GetResourceAmount(string key) => inventoryData[key];
    public static void SetResourceAmount(string key, int amount){
        inventoryData[key] = Mathf.Max(0, amount);
        inventoryChange?.Invoke(key, amount);
    }
    public static void AddResourceAmount(string key, int amount) => SetResourceAmount(key, GetResourceAmount(key) + amount);

    #endregion
}
