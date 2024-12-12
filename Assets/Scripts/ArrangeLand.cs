using DG.Tweening;
using UnityEngine;

public class ArrangeLand : MonoBehaviour
{
    public float spacing = 200f;
    public float moveDuration = 0.2f;

    public RectTransform targetArea;  // The area where tokens will be arranged

    public GameObject newLandCard;

    void Start()
    {
        ArrangeObjectsInRow();
    }

    public void ArrangeObjectsInRow()
    {
        if (transform.childCount == 0 || targetArea == null) return;

        Vector3 startPos = targetArea.anchoredPosition;

        for (int i = 0; i < transform.childCount; i++)
        {
            // Get the child at index `i`
            RectTransform child = transform.GetChild(i) as RectTransform;

            // Ensure the child is a RectTransform
            if (child != null)
            {
                Vector3 newPosition = new Vector3(startPos.x + (i * spacing), startPos.y, startPos.z);

                // Animate the child to its new position
                child.DOAnchorPos(newPosition, moveDuration).SetEase(Ease.OutQuad);
            }
        }
    }

    public void AddObject(GameObject newObject)
    {
        newLandCard.transform.SetAsLastSibling();

        // Parent the new object to this transform and ensure it's a direct child
        if (newObject != null && newObject.transform.parent != transform)
        {
            newObject.transform.SetParent(transform);
        }
        ArrangeObjectsInRow();

    }

    public void RemoveObject(GameObject objectToRemove)
    {
        if (objectToRemove != null && objectToRemove.transform.parent == transform)
        {
            objectToRemove.transform.SetParent(null);
        }
        ArrangeObjectsInRow();
    }

    public void ClearObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            child.SetParent(null);
        }
    }
}
