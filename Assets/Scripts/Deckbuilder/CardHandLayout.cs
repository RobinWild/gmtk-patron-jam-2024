using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class CardHandLayout : MonoBehaviour
{
    [Header("Card Hand Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int defaultHandSize = 5;
    [SerializeField] private float cardSpacing = 50f;

    [Header("Zones")]
    [SerializeField] private Transform deckZone;
    [SerializeField] private Transform discardZone;
    [SerializeField] public DropZone handZone;

    [Header("Buttons")]
    [SerializeField] private Button[] buttons;

    [Header("Timings")]
    [SerializeField] private float delayBetweenCards = 0.1f;
    [SerializeField] private float discardAnimationDuration = 0.25f;

    private RectTransform parentRectTransform;
    private List<GameObject> handCards = new();
    private List<GameObject> playedCards = new();

    private void Awake() => parentRectTransform = GetComponent<RectTransform>();

    public void DealOrDiscardHand()
    {
        SetButtonState(false);
        if (handCards.Count > 0 || playedCards.Count > 0) DiscardHandAndDealNew();
        else DealHand();
    }

    public void SetButtonState(bool enabled)
    {
        if (buttons == null) return;
        foreach (Button button in buttons) button.interactable = enabled;
    }

    private void DealHand()
    {
        SetButtonState(false);
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < defaultHandSize; i++)
        {
            sequence.AppendCallback(DrawCardToHand);
            if (i < defaultHandSize - 1) sequence.AppendInterval(0.1f);
        }

        sequence.OnComplete(() =>
        {
            SetButtonState(true);
            SortAndArrangeHand();
        }).Play();
    }

    public void DiscardHandAndDealNew()
    {
        Vector3 discardPosition = parentRectTransform.InverseTransformPoint(discardZone.position);
        Sequence discardSequence = DOTween.Sequence();

        AnimateCards(handCards, discardSequence, 0f, delayBetweenCards, discardPosition);
        AnimateCards(playedCards, discardSequence, handCards.Count * delayBetweenCards, delayBetweenCards, discardPosition);

        discardSequence.OnComplete(() =>
        {
            ClearCards(handCards);
            ClearCards(playedCards);
            DealHand();
        }).Play();
    }

    private void AnimateCards(List<GameObject> cards, Sequence sequence, float startDelay, float delayBetween, Vector3 targetPosition)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            if (card == null) continue;

            RectTransform cardTransform = card.GetComponent<RectTransform>();
            float startTime = startDelay + i * delayBetween;

            sequence.Insert(startTime, cardTransform.DOAnchorPos(targetPosition, discardAnimationDuration).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    Destroy(card); // Destroy the card after it finishes the position animation
                }));
            sequence.Insert(startTime, cardTransform.DORotate(Vector3.zero, discardAnimationDuration).SetEase(Ease.InQuad));
            }
    }

    private void ClearCards(List<GameObject> cards)
    {
        foreach (GameObject card in cards) if (card != null) Destroy(card);
        cards.Clear();
    }

    public void DrawCardToHand()
    {
        GameObject card = Instantiate(cardPrefab, parentRectTransform);
        handCards.Add(card);
        card.transform.position = deckZone.position;
        SortAndArrangeHand();
    }

    public void AddCardToHand(GameObject card, GameObject dropZone)
    {
        if (handCards.Contains(card))
        {
            card.GetComponent<Draggable>().ResetPosition();
            ArrangeHand();
            return;
        }

        if (playedCards.Contains(card)) playedCards.Remove(card);

        handCards.Add(card);
        SortAndArrangeHand();
    }

    private void SortAndArrangeHand()
    {
        SortHandByHierarchy();
        NormalizeCanvasSorting();
        ArrangeHand();
    }

    private void SortHandByHierarchy() =>
        handCards.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

    private void NormalizeCanvasSorting()
    {
        for (int i = 0; i < handCards.Count; i++)
        {
            Canvas canvas = handCards[i].GetComponent<Canvas>();
            if (canvas != null) canvas.sortingOrder = i;
        }
    }

    public void ArrangeHand()
    {
        float containerWidth = parentRectTransform.rect.width;
        float totalWidth = (handCards.Count - 1) * cardSpacing;
        float adjustedSpacing = totalWidth > containerWidth ? cardSpacing * (containerWidth / totalWidth) : cardSpacing;
        float startX = (handCards.Count - 1) * adjustedSpacing / 2;
        float rotationStep = 3f;
        float verticalArcHeight = 30f;

        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < handCards.Count; i++)
        {
            RectTransform cardTransform = handCards[i].GetComponent<RectTransform>();
            Vector3 position = new Vector3(
                startX - i * adjustedSpacing,
                -Mathf.Pow((i - (handCards.Count - 1) / 2f) * (2f / handCards.Count), 2) * verticalArcHeight,
                0
            );
            float angle = -rotationStep * (handCards.Count - 1) / 2 + i * rotationStep;

            sequence.Join(cardTransform.DOAnchorPos(position, discardAnimationDuration).SetEase(Ease.OutQuad));
            sequence.Join(cardTransform.DORotate(new Vector3(0, 0, angle), discardAnimationDuration).SetEase(Ease.OutQuad));
        }

        sequence.Play();
    }

    public void PlayCard(GameObject playedCard)
    {
        if (!handCards.Contains(playedCard)) return;

        playedCards.Add(playedCard);
        handCards.Remove(playedCard);
        SortAndArrangeHand();
    }

    public void MoveCardToDropZone(GameObject card, GameObject dropZone)
    {
        card.transform.DOMove(card.GetComponent<Draggable>()._drop.dropPosition, 0.2f);
        if (handCards.Contains(card)) handCards.Remove(card);
        if (!playedCards.Contains(card)) playedCards.Add(card);
        SortAndArrangeHand();
    }
}
