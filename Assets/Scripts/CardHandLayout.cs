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

    private RectTransform parentRectTransform;
    public List<GameObject> handCards = new List<GameObject>();
    public List<GameObject> playedCards = new List<GameObject>();

    private void Awake()
    {
        parentRectTransform = GetComponent<RectTransform>();
    }

    public void DealOrDiscardHand()
    {
        if (buttons != null)
        {
            foreach(Button button in buttons)
            {
                button.interactable = false;
            }
        }

        if (handCards.Count > 0 || playedCards.Count > 0)
        {
            DiscardHandAndDealNew();
        }
        else
        {
            DealHand();
        }
    }

    private void DealHand()
    {
        if (buttons != null)
        {
            foreach(Button button in buttons)
            {
                button.interactable = true;
            }
        }

        InstantiateCards();
        SortHandByHierarchy();
        ArrangeCardsSmoothly();
    }

    private void InstantiateCards()
    {
        for (int i = 0; i < defaultHandSize; i++)
        {
            GameObject card = Instantiate(cardPrefab, parentRectTransform);
            handCards.Add(card);
            card.transform.position = deckZone.position;
        }
    }

    public void DiscardHandAndDealNew()
    {
        Sequence discardSequence = DOTween.Sequence();

        foreach (var card in handCards)
        {
            if (card != null)
            {
                RectTransform cardTransform = card.GetComponent<RectTransform>();
                Vector3 discardPosition = parentRectTransform.InverseTransformPoint(discardZone.position);
                discardSequence.Join(cardTransform.DOAnchorPos(discardPosition, 0.5f).SetEase(Ease.InQuad));
                discardSequence.Join(cardTransform.DORotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.InQuad));
            }
        }

        foreach (var playedCard in playedCards)
        {
            if (playedCard != null)
            {
                RectTransform playedCardTransform = playedCard.GetComponent<RectTransform>();
                Vector3 discardPosition = parentRectTransform.InverseTransformPoint(discardZone.position);
                discardSequence.Join(playedCardTransform.DOAnchorPos(discardPosition, 0.5f).SetEase(Ease.InQuad));
            }
        }

        discardSequence.OnComplete(() =>
        {
            foreach (var card in handCards)
            {
                if (card != null) Destroy(card);
            }

            foreach (var playedCard in playedCards)
            {
                if (playedCard != null) Destroy(playedCard);
            }

            handCards.Clear();
            playedCards.Clear();

            DealHand();  // Call DealHand after discard is complete
        });

        discardSequence.Play();
    }

    public void DrawCardToHand()
    {
        GameObject newCard = Instantiate(cardPrefab, parentRectTransform);
        handCards.Add(newCard);
        newCard.transform.position = deckZone.position;
        SortHandByHierarchy();
        ArrangeCardsSmoothly();
    }

    public void AddCardToHand(GameObject card, GameObject dropZone)
    {
        // Prevent duplicates
        if (handCards.Contains(card))
        {
            card.GetComponent<Draggable>().ResetPosition();
            ArrangeCardsSmoothly();
            return;
        }

        // Remove from played cards if applicable
        if (playedCards.Contains(card))
        {
            playedCards.Remove(card);
        }

        // Add to the hand and sort by hierarchy
        handCards.Add(card);
        SortHandByHierarchy();

        // Normalize canvas sorting for all hand cards
        NormalizeCanvasSorting();
        ArrangeCardsSmoothly();
    }

    private void SortHandByHierarchy()
    {
        // Sort handCards by their hierarchy order
        handCards.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }

    private void NormalizeCanvasSorting()
    {
        // Ensure cards in the hand have their Canvas sorting adjusted based on their position
        for (int i = 0; i < handCards.Count; i++)
        {
            Canvas canvas = handCards[i].GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = i; // Assign sort order by position
            }
        }
    }

    public void ArrangeCardsSmoothly()
    {
        float containerWidth = parentRectTransform.rect.width;
        float totalWidth = (handCards.Count - 1) * cardSpacing;

        float adjustedSpacing = totalWidth > containerWidth
            ? cardSpacing * (containerWidth / totalWidth)
            : cardSpacing;

        float startX = (handCards.Count - 1) * adjustedSpacing / 2;
        float rotationStep = 3f;
        float verticalArcHeight = 30f;

        Sequence arrangementSequence = DOTween.Sequence();

        for (int i = 0; i < handCards.Count; i++)
        {
            RectTransform cardTransform = handCards[i].GetComponent<RectTransform>();
            Vector3 targetCardPosition = new Vector3(
                startX - i * adjustedSpacing,
                -Mathf.Pow((i - (handCards.Count - 1) / 2f) * (2f / handCards.Count), 2) * verticalArcHeight,
                0
            );

            float rotationAngle = -rotationStep * (handCards.Count - 1) / 2 + i * rotationStep;

            arrangementSequence.Join(cardTransform
                .DOAnchorPos(targetCardPosition, 0.2f)
                .SetEase(Ease.OutQuad));

            arrangementSequence.Join(cardTransform
                .DORotate(new Vector3(0, 0, rotationAngle), 0.2f)
                .SetEase(Ease.OutQuad));
        }

        arrangementSequence.Play();
    }

    public void PlayCard(GameObject playedCard)
    {
        if (!handCards.Contains(playedCard)) return;

        playedCards.Add(playedCard);
        handCards.Remove(playedCard);

        SortHandByHierarchy();
        ArrangeCardsSmoothly();
    }

    public void MoveCardToDropZone(GameObject card, GameObject dropZone)
    {
        card.transform.DOMove(card.GetComponent<Draggable>()._drop.DropPosition, 0.2f);
        
        if (handCards.Contains(card)) handCards.Remove(card);
        if (!playedCards.Contains(card)) playedCards.Add(card);

        SortHandByHierarchy();
        ArrangeCardsSmoothly();
    }
}
