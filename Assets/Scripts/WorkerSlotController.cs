using UnityEngine;
using DG.Tweening;
using Spine;

public class WorkerSlotController : MonoBehaviour
{
    public Worker currentWorker;

    public void MoveCardToDropZone(GameObject card, GameObject dropZone)
    {
        card.transform.DOMove(card.GetComponent<Draggable>()._drop.dropPosition, 0.2f);

        card.transform.SetParent(dropZone.transform);

        currentWorker = card.GetComponent<Worker>();
    }

    public void EjectWorker()
    {
        // TODO: make worker go to predefined home
        currentWorker.home.ManuallyDropObject(currentWorker.gameObject);
        currentWorker = null;
    }
}
