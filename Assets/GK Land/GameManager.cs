using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // todo: requests

    public GKslots slots;
    public GKcard cardPrefab;
    public Transform counters;
    
    public RectTransform canvasRoot;
    public RectTransform incomingParticleTarget;
    public RectTransform outgoingParticleTarget;

    public string[] prefabKeys = new string[]{};
    public GameObject[] resourcePrefabs = new GameObject[]{};
    public Dictionary<string, GameObject> prefabDict = new();

    public static GameManager instance;

    void Awake(){
        instance = this;

        foreach(var key in new string[]{"gold", "wood", "food", "military", "reputation", "revolution"}) SetResourceAmount(key, 0);
        SetResourceAmount("reputation", 1000);

        for (int i = 0; i < prefabKeys.Length; i++)
            prefabDict[ prefabKeys[i] ] = resourcePrefabs[i];

        DOTween.Sequence()
            .AppendInterval(1f)
            .AppendCallback(CardGenerator.Taxation)
            .AppendInterval(1f)
            .AppendCallback(CardGenerator.WoodSlaves)
            .AppendInterval(1f)
            .AppendCallback(CardGenerator.WoodImport);
    }

    void Start(){
        inventoryChange += (key, amount) => {
            var t = counters.Find(key);
            if(t != null) t.GetComponentInChildren<TMPro.TMP_Text>().text = amount.ToString();
        };
    }

    void Update(){
        GameTimeController.Update(Time.deltaTime);
    }

    #region Inventory

    static Dictionary <string, int> inventoryData = new();
    public static event Action<string, int> inventoryChange;

    public static int GetResourceAmount(string key) => inventoryData[key];
    public static void SetResourceAmount(string key, int amount){
        inventoryData[key] = Mathf.Clamp(amount, 0, 1000);
        inventoryChange?.Invoke(key, amount);
    }
    public static void AddResourceAmount(string key, int amount) => SetResourceAmount(key, GetResourceAmount(key) + amount);

    #endregion
}
