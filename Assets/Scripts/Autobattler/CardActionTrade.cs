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
    public float resourceTweenDuration = 1f;
    public GameObject resourceSpawner;
    private GameObject targetObject;

    private CardController cardController => GetComponent<CardController>();

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

    public void CheckTaskReadyToStart()
    {
        if (Inventory.GetResource(tradeInput.resourceType) >= tradeInput.quantity)
        {
            GetComponent<CardController>().SetTaskReady();
            Inventory.RemoveResource(tradeInput.resourceType, tradeInput.quantity);

            for (int i = 0; i < tradeInput.quantity; i++)
            {
                InstantiateAndTweenResource(tradeInput.resourceType, GetTargetPosition(), transform.parent.transform, false, -0.5f, i);
            }
        }
        else
        {
            GetComponent<CardController>().isTaskReadyToProgress = false;
        }
    }

    public void RefundResources()
    {
            for (int i = 0; i < tradeInput.quantity; i++)
            {
                InstantiateAndTweenResource(tradeInput.resourceType, transform, GetTargetPosition(), true, 1f, i);
            }
    }

    public void CheckIfReadyToPerformAction()
    {
        bool hasRoomForMoreResource = Inventory.GetResource(tradeOutput.resourceType) <=  Inventory.GetMaxResource(tradeOutput.resourceType) - 1;
        if (hasRoomForMoreResource) cardController.GrantApproval();
    }

    public async void PerformAction()
    {
        var cardController = GetComponent<CardController>();

        if (cardController.isTaskReadyToProgress)
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

        float moveDuration = resourceTweenDuration * (1 + Mathf.Exp(-iteration / 5f));

        float arcHeight = 3f;
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
        if (!GetComponent<CardController>().isTaskReadyToProgress) return;

        RefundResources();  // Refund the resources
        Debug.Log("Action canceled. Resources refunded.");
    }
}
