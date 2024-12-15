using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // todo: inventory, requests
    public static GameManager instance;

    void Awake(){
        if(instance != null) Debug.LogError("uh oh");
        instance = this;
    }

    void Update()
    {
        GameTimeController.Update(Time.deltaTime);
    }
}
