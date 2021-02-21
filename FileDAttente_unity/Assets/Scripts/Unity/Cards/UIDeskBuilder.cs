using UnityEngine;

public class UIDeskBuilder : MonoBehaviour
{
    private UICardPileBuilder[] pileBuilders;

    public UICardDesk CurrentDesk { get; private set; }
    public int PileCount => Mathf.Min(pileBuilders != null ? pileBuilders.Length : 0, CurrentDesk != null ? CurrentDesk.PileCount : 0);

    private void Awake()
    {
        pileBuilders = GetComponentsInChildren<UICardPileBuilder>(true);
    }

    //private void OnEnable()
    //{
    //    List<UICardPile> piles = new List<UICardPile>();
    //    pileBuilders = GetComponentsInChildren<UICardPileBuilder>(true);
    //    foreach (UICardPileBuilder builder in pileBuilders)
    //    {
    //        builder.ResetPile();
    //        piles.Add(builder.CurrentPile);
    //    }
    //    CurrentDesk = new UICardDesk(piles.ToArray());
    //}

    public void Build(UICardDesk desk)
    {
        Clear();
        CurrentDesk = desk;
        if (CurrentDesk != null)
        {
            if (CurrentDesk.PileCount != pileBuilders.Length)
                Debug.LogError("Desk / DeskBuilder size mismatch");
            else
            {
                for (int i = 0, iend = pileBuilders.Length; i < iend; i++)
                {
                    if (pileBuilders[i] == null) continue;
                    pileBuilders[i].BuildPile(CurrentDesk.CardPiles[i]);
                }
            }
        }
    }

    public void Clear()
    {
        for (int i = 0, iend = pileBuilders.Length; i < iend; i++)
        {
            if (pileBuilders[i] == null) continue;
            pileBuilders[i].ClearPile();
        }
    }
}
