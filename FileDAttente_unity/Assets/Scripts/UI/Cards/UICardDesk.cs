using System;

public class UICardDesk
{
    public delegate void CardEvent(UICardDesk desk, UICardPile pile, UICard card);
    public event CardEvent onEndEditCard;

    public UICardPile[] CardPiles { get; private set; }

    public int PileCount => CardPiles != null ? CardPiles.Length : 0;

    public UICardDesk(UICardPile[] piles)
    {
        CardPiles = piles;
        if (CardPiles != null)
            foreach (UICardPile p in CardPiles)
            {
                p.onCardRequest += OnCardRequest;
                p.onEndEditCard += OnEndEditCard;
            }
    }

    ~UICardDesk()
    {
        if (CardPiles != null)
            foreach (UICardPile p in CardPiles)
            {
                p.onCardRequest -= OnCardRequest;
                p.onEndEditCard -= OnEndEditCard;
            }
    }

    public UICard GetTopCard(int pileIndex, out bool isOnlyCard)
    {
        isOnlyCard = false;
        if (CardPiles == null || pileIndex < 0 || pileIndex > CardPiles.Length)
            return null;
        else
        {
            isOnlyCard = (CardPiles[pileIndex].Size == 1);
            return CardPiles[pileIndex].TopCard;
        }
    }    

    public void DealCard(int pileIndex, UICard card)
    {
        if (CardPiles == null || pileIndex < 0 || CardPiles.Length <= pileIndex)
            return;

        if (CardPiles[pileIndex] == null)
            CardPiles[pileIndex] = new UICardPile();

        CardPiles[pileIndex].PileCard(card);
    }

    public void CloseAllCards()
    {
        foreach (UICardPile pile in CardPiles)
            pile.CloseAllCards();
    }

    private void OnCardRequest(UICardPile pile, UICard card)
    {
        if (pile == null || card == null || CardPiles == null || CardPiles.Length == 0) return;

        int pileIndex = Array.FindIndex(CardPiles, p => p.ContainsCard(card));
        if (pileIndex < 0)
        {
            pileIndex = Array.IndexOf(CardPiles, pile);
            if (pileIndex >= 0)
                pileIndex = (pileIndex + 1) % CardPiles.Length;
            else
                pileIndex = 0;
        }
        DealCard(pileIndex, card);
    }

    private void OnEndEditCard(UICardPile pile, UICard card)
    {
        onEndEditCard?.Invoke(this, pile, card);
    }
}
