using UnityEngine;

public class GKslots : MonoBehaviour
{
    public const int NUM_SLOTS = 8;
    const float CARD_WIDTH = 139;
    const float BASE_OFFSET = -((CARD_WIDTH * (NUM_SLOTS/2)) - CARD_WIDTH/2);

    GKcard[] cards = new GKcard[NUM_SLOTS]; // create an array with length NUM_SLOTS

    void Start(){
        for (int i = 0; i < transform.childCount; i++){
            SetSlot(i, transform.GetChild(i).GetComponent<GKcard>());
        }

        ConformCards();
    }

    public float GetSlotPosition(int index) =>  BASE_OFFSET + (index * CARD_WIDTH);
    public int PosToIndex(float xPos) =>  Mathf.FloorToInt((xPos / CARD_WIDTH) + (NUM_SLOTS / 2));

    public void ConformCards(){
        foreach(var c in cards)
            if(c != null) c.ConformToSlotPosition();
    }

    public GKcard NewCard(){
        int at = -1;
        int i;

        for(i = 0; i < NUM_SLOTS; i++) if(EmptySlot(i)){
            at = i;
            break;
        }

        if(at == -1) return null;

        var card = Instantiate(GameManager.instance.cardPrefab, transform);
        SetSlot(at, card);

        return card;
    }

    public bool EmptySlot(int index) => cards[index] == null;

    public void SetSlot(int index, GKcard card){
        cards[index] = card;

        if(card != null){
            card.slotIndex = index;
            card.slots = this;
        }
    }
    public void MoveSlot(int from, int to){
        // if the attempted move is invalid, return
        // todo: allow moves where cards are all shuffled along one slot
        if(to < 0 || to >= NUM_SLOTS || from == to || !EmptySlot(to)) return;

        var card = cards[from];
        SetSlot(from, null);
        SetSlot(to, card);
    }
}
