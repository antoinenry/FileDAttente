using UnityEngine;
using System;

[ExecuteAlways]
public class UICardPileBuilder : MonoBehaviour
{
    [SerializeField] [DataPack] private string currentBottomDataType;
    [SerializeField] [DataPack] private string currentTopDataType;

    [Header("Components")]
    public UICardBuilder bottomBuilder;
    public UICardBuilder topBuilder;

    public UICardPile CurrentPile { get; private set; }

    private void Reset()
    {
        CurrentPile = new UICardPile();
        currentBottomDataType = "(null)";
        currentTopDataType = "(null)";
    }

    private void OnEnable()
    {
        AddListenners();
    }

    private void OnDisable()
    {
        RemoveListenners();
    }

    //private void Update()
    //{
    //    ResetPile();
    //    _pileSize = CurrentPile.Size;
    //}

    private void AddListenners()
    {
        if (bottomBuilder != null) bottomBuilder.onCardChange.AddListener(OnBottomCardChange);
        if (topBuilder != null) topBuilder.onCardChange.AddListener(OnTopCardChange);
        if (CurrentPile != null) CurrentPile.onDealCard += OnDealCard;
    }

    private void RemoveListenners()
    {
        if (bottomBuilder != null) bottomBuilder.onCardChange.RemoveListener(OnBottomCardChange);
        if (topBuilder != null) bottomBuilder.onCardChange.RemoveListener(OnTopCardChange);
        if (CurrentPile != null) CurrentPile.onDealCard -= OnDealCard;
    }

    public void BuildPile(UICardPile pile)
    {
        ClearPile();
        SetCurrentPile(pile);
        BuildBottomCard();
        BuildTopCard();
    }

    public void ClearPile()
    {
        if (bottomBuilder != null) bottomBuilder.ClearCard();
        if (topBuilder != null) topBuilder.ClearCard();
        //if (CurrentPile != null)
        //    CurrentPile.ClearCards();
        //else
            SetCurrentPile(new UICardPile());
    }

    public void BuildBottomCard()
    {
        if (bottomBuilder != null && CurrentPile != null) bottomBuilder.BuildCard(CurrentPile.BottomCard);
    }

    public void BuildTopCard()
    {
        if (topBuilder != null && CurrentPile != null) topBuilder.BuildCard(CurrentPile.TopCard);
    }

    public void ResetPile()
    {
        UICard resetBottomCard = ResetBottomCard();
        UICard resetTopCard = ResetTopCard();
        UICard currentBottomCard = null;
        UICard currentTopCard = null;
        if (CurrentPile != null)
        {
            currentBottomCard = CurrentPile.BottomCard;
            currentTopCard = CurrentPile.TopCard;
        }
        else
            BuildPile(new UICardPile());
        
        if (currentBottomCard != resetBottomCard)
        {
            UICardPile newPile = new UICardPile();
            newPile.PileCard(resetBottomCard);
            newPile.PileCard(resetTopCard);
            BuildPile(newPile);
        }        
        else if (currentTopCard != resetTopCard)
            CurrentPile.PileCard(resetTopCard);
    }

    private void SetCurrentPile(UICardPile newPile)
    {
        RemoveListenners();
        CurrentPile = newPile != null ? newPile : new UICardPile();
        AddListenners();
    }

    private UICard ResetBottomCard()
    {
        if (bottomBuilder == null) return null;
        Type dataType = currentBottomDataType != null ? Type.GetType(currentBottomDataType) : null;
        if (dataType != null)
        {
            if (bottomBuilder.CurrentCard == null || dataType != bottomBuilder.CurrentCard.GetDataType())
                bottomBuilder.BuildCard(UICard.NewCard(dataType, false, false));
        }
        else if (bottomBuilder.CurrentCard != null)
            bottomBuilder.ClearCard();

        return bottomBuilder.CurrentCard;
    }

    private UICard ResetTopCard()
    {
        if (topBuilder == null) return null;
        Type dataType = currentTopDataType != null ? Type.GetType(currentTopDataType) : null;
        if(dataType != null)
        {
            if (topBuilder.CurrentCard == null || dataType != topBuilder.CurrentCard.GetDataType())
                topBuilder.BuildCard(UICard.NewCard(dataType, false, true));
        }
        else if (topBuilder.CurrentCard != null)
            topBuilder.ClearCard();

        return topBuilder.CurrentCard;
    }

    private void OnBottomCardChange()
    {
        if (bottomBuilder == null) return;
        Type bottomType = bottomBuilder.CurrentCard != null ? bottomBuilder.CurrentCard.GetDataType() : null;
        currentBottomDataType = bottomType != null ? bottomType.AssemblyQualifiedName : "(null)";
        ResetBottomCard();
    }

    private void OnTopCardChange()
    {
        if (topBuilder == null) return;
        Type topType = topBuilder.CurrentCard != null ? topBuilder.CurrentCard.GetDataType() : null;
        currentTopDataType = topType != null ? topType.AssemblyQualifiedName : "(null)";
        ResetTopCard();
    }

    private void OnDealCard(UICardPile pile, UICard card)
    {
        if (pile == null || card == null  || pile != CurrentPile) return;
        BuildBottomCard();
        BuildTopCard();
    }
}