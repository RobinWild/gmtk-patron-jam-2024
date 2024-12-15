using TMPro;
using UnityEngine;

public class ResourceCountDisplay : MonoBehaviour
{
    [SerializeField] private Inventory.ResourceType resourceType;
    private TextMeshProUGUI resourceText;

    private void Start()
    {
        resourceText = GetComponent<TextMeshProUGUI>();
        Inventory.RegisterResourceThresholdListener(resourceType, 0, UpdateResourceText);
        UpdateResourceText();
    }

    private void UpdateResourceText()
    {
        int resourceCount = Inventory.GetResource(resourceType);
        resourceText.text = $"{resourceCount}";
    }
}
