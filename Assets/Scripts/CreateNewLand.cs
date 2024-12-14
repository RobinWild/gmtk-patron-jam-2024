using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CreateNewLand : MonoBehaviour
{
    public List<GameObject> landPool = new List<GameObject>(); // Pool of land card prefabs
    public ArrangeLand arrangeLand; // Reference to the ArrangeLand script

    public void Create()
    {
        // Check if the landPool is not empty
        if (landPool == null || landPool.Count == 0)
        {
            Debug.LogWarning("Land pool is empty. Cannot create a new land.");
            return;
        }

        // Choose a random land card from the pool
        int randomIndex = Random.Range(0, landPool.Count);
        GameObject landToInstantiate = landPool[randomIndex];

        // Instantiate the land card
        GameObject newLand = Instantiate(landToInstantiate, arrangeLand.transform);
        newLand.transform.position = transform.position;

        if (arrangeLand != null)
        {
            arrangeLand.AddObject(newLand);
        }
    }
}
