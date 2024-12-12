using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Worker : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public float workSpeed = 1f;
    public DropZone previousDropZone;
    public DropZone dropZone;
    private Draggable draggable;

    public void Start()
    {
        draggable = GetComponent<Draggable>();
    }

    public void Update()
    {
        if (previousDropZone != dropZone)
        {
            if (previousDropZone != null){
                if (previousDropZone.child == GetComponent<Draggable>()) previousDropZone.child = null;
                previousDropZone.OpenSlot();

                if (previousDropZone.GetComponent<ArrangeTokens>())
                {
                    previousDropZone.GetComponent<ArrangeTokens>().RemoveObject(this.GetComponent<RectTransform>());
                }
            }
        }


        previousDropZone = dropZone;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (previousDropZone != null){
            previousDropZone.OpenSlot();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggable._drop == null)
        {
            if (previousDropZone != null && previousDropZone.singleSlot) previousDropZone.CloseSlot();
        }
    }

}