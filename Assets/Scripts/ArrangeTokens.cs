using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class ArrangeTokens : MonoBehaviour
{
    public List<RectTransform> objectsToArrange = new List<RectTransform>();
    public float spacing = 50f;
    public float moveDuration = 0.2f;

    public RectTransform targetArea;  // The area where tokens will be arranged

    void Start()
    {
        ArrangeObjectsInRow();
    }

    public void ArrangeObjectsInRow()
    {
        if (objectsToArrange == null || objectsToArrange.Count == 0 || targetArea == null) return;

        // Get the starting position (center of the target area)
        float startX = targetArea.rect.width / 2f;

        // Ensure the first object is centered within the target area
        if (objectsToArrange.Count > 0)
        {
            RectTransform firstObject = objectsToArrange[0];
            startX -= firstObject.rect.width / 2f;  // Adjust starting position to center the first object
        }

        // Loop through each object to arrange them
        for (int i = 0; i < objectsToArrange.Count; i++)
        {
            RectTransform obj = objectsToArrange[i];

            // Calculate the new X position for each object
            float targetX = startX + (i * spacing);

            // Animate the object to the new position simultaneously
            obj.DOAnchorPosX(targetX, moveDuration).SetEase(Ease.OutQuad);
        }
    }

    public void AddObject(RectTransform newObject)
    {
        if (!objectsToArrange.Contains(newObject) && newObject != null)
        {
            objectsToArrange.Add(newObject);
        }
        SortHandByHierarchy();
        ArrangeObjectsInRow();
    }

    public void RemoveObject(RectTransform objectToRemove)
    {
        if (objectToRemove == null || !objectsToArrange.Contains(objectToRemove)) return;

        objectsToArrange.Remove(objectToRemove);
        SortHandByHierarchy();
        ArrangeObjectsInRow();
    }

    public void ClearObjects()
    {
        objectsToArrange.Clear();
    }

    private void SortHandByHierarchy(){
        objectsToArrange.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }
}
