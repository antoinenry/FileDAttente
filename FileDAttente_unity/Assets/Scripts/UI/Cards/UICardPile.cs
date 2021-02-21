using System.Collections.Generic;

public class UICardPile
{
    private List<UICard> cards;

    //public delegate void PileEvent(UICardPile pile);
    //public event PileEvent onClearPile;

    public delegate void CardEvent(UICardPile pile, UICard card);
    public event CardEvent onDealCard;
    public event CardEvent onCardRequest;
    public event CardEvent onEndEditCard;
    public event CardEvent onSaveCard;

    public int Size => cards != null ? cards.Count : 0;
    public UICard TopCard => cards != null && cards.Count > 1 ? cards[cards.Count - 1] : null;
    public UICard BottomCard => cards != null && cards.Count > 0 ? cards[0] : null;

    public void PileCard(UICard card)
    {
        if (card == null) return;
        if (cards == null)
        {
            cards = new List<UICard> { card };
            AddCardListeners(card);
        }
        else
        {
            int indexInPile = cards.IndexOf(card);
            if (indexInPile == -1)
            {
                cards.Add(card);
                AddCardListeners(card);
            }
            else
            {
                cards.RemoveAt(indexInPile);
                cards.Add(card);
            }
        }
        if (onDealCard != null)
            onDealCard?.Invoke(this, card);
    }

    public void RemoveCard(UICard card)
    {
        if (card == null || cards == null) return;
        RemoveCardListeners(card);
        cards.Remove(card);
        onDealCard?.Invoke(this, card);
    }

    public bool ContainsCard(UICard card)
    {
        return cards != null && card != null && cards.Contains(card);
    }

    public void CloseAllCards()
    {
        for (int i = Size - 1; i >= 0; i--)
            if (cards[i].closable) cards[i].Close();
    }

    private void AddCardListeners(UICard card)
    {
        if (card == null) return;
        card.onCardRequest += OnCardRequest;
        card.onClose += OnCardClosed;
        card.onEndEdit += OnCardEndEdit;
        card.onSave += OnCardSaved;
    }

    private void RemoveCardListeners(UICard card)
    {
        if (card == null) return;
        card.onCardRequest -= OnCardRequest;
        card.onClose -= OnCardClosed;
        card.onEndEdit -= OnCardEndEdit;
        card.onSave -= OnCardSaved;
    }

    private void OnCardRequest(UICard card)
    {
        onCardRequest?.Invoke(this, card);
    }

    private void OnCardClosed(UICard card)
    {
        RemoveCard(card);
    }

    private void OnCardEndEdit(UICard card)
    {
        onEndEditCard?.Invoke(this, card);
    }

    private void OnCardSaved(UICard card)
    {
        onSaveCard?.Invoke(this, card);
    }
}