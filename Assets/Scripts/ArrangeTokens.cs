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

        Vector3 startPos = targetArea.anchoredPosition;

        for (int i = 0; i < objectsToArrange.Count; i++)
        {
            Vector3 newPosition = new Vector3 (startPos.x + (i * spacing), (startPos.y) + (i % 2 * -spacing), startPos.z);

            objectsToArrange[i].DOAnchorPos(newPosition, moveDuration).SetEase(Ease.OutQuad);
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
