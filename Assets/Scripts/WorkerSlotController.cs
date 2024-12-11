using UnityEngine;
using DG.Tweening;

public class WorkerSlotController : MonoBehaviour
{
    public Worker currentWorker;

    public void MoveCardToDropZone(GameObject card, GameObject dropZone)
    {
        card.transform.DOMove(card.GetComponent<Draggable>()._drop.dropPosition, 0.2f);

        card.transform.SetParent(dropZone.transform);

        currentWorker = card.GetComponent<Worker>();
    }
}
