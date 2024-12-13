using UnityEngine;
using DG.Tweening;
using System.Runtime.CompilerServices;

public class CardActionTrade : MonoBehaviour
{
    [System.Serializable]
    public class TradeResource
    {
        public Inventory.ResourceType resourceType;
        public int quantity;
    }

    public TradeResource tradeInput;
    public TradeResource tradeOutput;
    public string targetObjectName = "ResourceTarget";
    public GameObject resourceSpawner;
    private GameObject targetObject;

    private Transform GetTargetPosition()
    {
        targetObject = GameObject.Find(targetObjectName);
        if (targetObject == null)
        {
            Debug.LogError($"Target object with name '{targetObjectName}' not found!");
            return null;
        }
        return targetObject.transform;
    }

    public void CheckTaskReady()
    {
        if (Inventory.GetResource(tradeInput.resourceType) >= tradeInput.quantity)
        {
            GetComponent<CardController>().isTaskReady = true;
            Inventory.RemoveResource(tradeInput.resourceType, tradeInput.quantity);

            for (int i = 0; i < tradeInput.quantity; i++)
            {
                InstantiateAndTweenResource(tradeInput.resourceType, GetTargetPosition(), transform, false, -0.5f, i);
            }
        }
        else
        {
            GetComponent<CardController>().isTaskReady = false;
        }
    }

    public void RefundResources()
    {
            for (int i = 0; i < tradeInput.quantity; i++)
            {
                InstantiateAndTweenResource(tradeInput.resourceType, transform, GetTargetPosition(), true, 1f, i);
            }
        Debug.Log($"Refunded {tradeInput.resourceType}.");
    }

    public async void PerformAction()
    {
        var cardController = GetComponent<CardController>();

        if (cardController.isTaskReady)
        {
            for (int i = 0; i < tradeOutput.quantity; i++)
            {
                InstantiateAndTweenResource(tradeOutput.resourceType, resourceSpawner.transform, GetTargetPosition(), true, 1, i);
            }
        }
    }

    private void InstantiateAndTweenResource(Inventory.ResourceType resourceType, Transform startTransform, Transform endTransform, bool isOutput, float arcScalar = 1, int iteration = 1)
    {
        GameObject resourcePrefab = Inventory.GetResourcePrefab(resourceType);
        if (resourcePrefab == null)
        {
            Debug.LogError($"No prefab found for resource type {resourceType}");
            return;
        }

        GameObject resourceInstance = Instantiate(resourcePrefab, targetObject.transform);

        resourceInstance.transform.position = startTransform.position;
        resourceInstance.transform.localScale = Vector3.one;
        float iterationInterval = 0.2f;
        float moveDuration = 0.5f + iterationInterval * iteration;

        float arcHeight = 1.5f;
        float arcVariance = 0.5f;

        resourceInstance.transform.DOMoveX(endTransform.position.x, moveDuration).SetEase(Ease.Linear);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(resourceInstance.transform.DOMoveY(endTransform.position.y + Random.Range(arcHeight - arcVariance, arcHeight + arcVariance) * arcScalar, moveDuration / 2)
            .SetEase(Ease.OutSine));

        sequence.Append(resourceInstance.transform.DOMoveY(endTransform.position.y, moveDuration / 2)
            .SetEase(Ease.InSine));

        sequence.OnComplete(() =>
        {
            DestroyWithParticleSystems destroyWithParticleSystems = resourceInstance.GetComponent<DestroyWithParticleSystems>();
            if (destroyWithParticleSystems)
            {
                destroyWithParticleSystems.DestroyAfterParticlesStop();
            }
            else
            {
                Destroy(resourceInstance);
            }

            if (isOutput) Inventory.AddResource(resourceType, 1);
        });
    }

    // Call this method when the card is removed or canceled before performing the action
    public void CancelAction()
    {
        if (!GetComponent<CardController>().isTaskReady) return;

        RefundResources();  // Refund the resources
        Debug.Log("Action canceled. Resources refunded.");
    }
}
