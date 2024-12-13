using UnityEngine;

public class GameTimeUpdater : MonoBehaviour
{
    void Update()
    {
        GameTimeController.Update(Time.deltaTime);
    }
}