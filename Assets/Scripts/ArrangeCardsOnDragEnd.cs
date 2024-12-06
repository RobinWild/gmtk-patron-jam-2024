using UnityEngine;
using UnityEngine.EventSystems;

public class ArrangeCardsOnDragEnd : MonoBehaviour, IEndDragHandler
{
    private CardHandLayout cardHandLayout;

    void Start()
    {
        cardHandLayout = FindFirstObjectByType<CardHandLayout>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cardHandLayout.ArrangeCardsSmoothly();
    }
}
