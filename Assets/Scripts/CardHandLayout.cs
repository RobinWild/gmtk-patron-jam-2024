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

    private RectTransform parentRectTransform;
    public List<GameObject> handCards = new List<GameObject>();
    public List<GameObject> playedCards = new List<GameObject>();

    private void Awake()
    {
        parentRectTransform = GetComponent<RectTransform>();
    }

    public void DealOrDiscardHand()
    {
        if (handCards.Count > 0 || playedCards.Count > 0)
        {
            Debug.Log("Discarding cards");
            DiscardHandAndDealNew();
        }
        else
        {
            DealHand();
        }
    }

    private void DealHand()
    {
        CreateCards();
        ArrangeCardsSmoothly();
    }

    private void CreateCards()
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

            DealHand();
        });

        discardSequence.Play();
    }

    public void DrawCardToHand()
    {
        GameObject newCard = Instantiate(cardPrefab, parentRectTransform);
        handCards.Add(newCard);
        newCard.transform.position = deckZone.position;
        ArrangeCardsSmoothly();
    }

    public void AddCardToHand(GameObject card, GameObject dropZone)
    {
        Debug.Log("Adding card to hand");
        if (handCards.Contains(card))
        {
            card.GetComponent<DragAndDrop>().ResetPosition();
            return;
        }

        handCards.Add(card);
        if (playedCards.Contains(card)){
            playedCards.Remove(card);
        }
        ArrangeCardsSmoothly();
    }

    private void ArrangeCardsSmoothly()
    {
        float containerWidth = parentRectTransform.rect.width;
        float totalWidth = (handCards.Count - 1) * cardSpacing;

        // Adjust card spacing if total width exceeds container width
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

        ArrangeCardsSmoothly();
    }

    public void MoveCardToDropZone(GameObject card, GameObject dropZone)
    {
        Debug.Log("Moving card to dropzone");
        card.transform.DOMove(card.GetComponent<DragAndDrop>()._drop.DropPosition, 0.2f);
        
        if (handCards.Contains(card)) handCards.Remove(card);
        if (!playedCards.Contains(card)) playedCards.Add(card);

        ArrangeCardsSmoothly();
    }
}
