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
        SetButtonState(false);

        if (handCards.Count > 0 || playedCards.Count > 0)
        {
            DiscardHandAndDealNew();
        }
        else
        {
            DealHand();
        }
    }

    public void SetButtonState(bool enable)
    {
        if (buttons != null)
        {
            foreach(Button button in buttons)
            {
                button.interactable = enable;
            }
        }
    }

    private void DealHand()
    {
        // Disable buttons immediately before starting the card instantiation sequence
        SetButtonState(false);

        InstantiateCards();
    }

    private void InstantiateCards()
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < defaultHandSize; i++)
        {
            sequence.AppendCallback(() =>
            {
                DrawCardToHand();
            });

            if (i < defaultHandSize - 1)
            {
                sequence.AppendInterval(0.1f);
            }
        }

        sequence.OnComplete(() =>
        {
            SetButtonState(true);
            SortHandByHierarchy();
            ArrangeCardsSmoothly();
        });

        sequence.Play();
    }

    public void DiscardHandAndDealNew()
    {
        Sequence discardSequence = DOTween.Sequence();
        float delayBetweenCards = 0.05f; // Delay between the start of each card's discard animation
        Vector3 discardPosition = parentRectTransform.InverseTransformPoint(discardZone.position);

        // Discard hand cards
        AnimateCardDiscard(handCards, discardSequence, 0f, delayBetweenCards, discardPosition);

        // Discard played cards after hand cards
        float startDelayForPlayedCards = handCards.Count * delayBetweenCards;
        AnimateCardDiscard(playedCards, discardSequence, startDelayForPlayedCards, delayBetweenCards, discardPosition);

        // Clean up and deal new cards after discarding is complete
        discardSequence.OnComplete(() =>
        {
            ClearCards(handCards);
            ClearCards(playedCards);
            DealHand();
        });

        discardSequence.Play();
    }

    private void AnimateCardDiscard(
        List<GameObject> cards, Sequence sequence, float initialDelay, float delayBetweenCards, Vector3 discardPosition)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            if (card != null)
            {
                RectTransform cardTransform = card.GetComponent<RectTransform>();

                // Add animations to the sequence with staggered start times
                float startTime = initialDelay + i * delayBetweenCards;
                sequence.Insert(startTime, cardTransform.DOAnchorPos(discardPosition, 0.5f).SetEase(Ease.InQuad));
                sequence.Insert(startTime, cardTransform.DORotate(Vector3.zero, 0.3f).SetEase(Ease.InQuad));
            }
        }
    }

    private void ClearCards(List<GameObject> cards)
    {
        foreach (var card in cards)
        {
            if (card != null)
            {
                Destroy(card);
            }
        }
        cards.Clear();
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
