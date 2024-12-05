using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector3 dropOffset;
    public Vector3 DropPosition => transform.localPosition + dropOffset;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragAndDrop.Active != null) // Check if a drag operation is active
        {
            DragAndDrop.Active.SetDropZone(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DragAndDrop.Active != null) // Check if a drag operation is active
        {
            DragAndDrop.Active.SetDropZone(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + dropOffset, Vector3.one);
    }
}