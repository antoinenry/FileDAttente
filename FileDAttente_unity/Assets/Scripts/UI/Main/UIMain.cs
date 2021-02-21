public class UIMain
{
    public string databaseSavePath;

    private UICard databaseCard;
    private UICard calculatorInputCard;
    private UICard calculatorOutputCard;
    private int currentTab;
    private Calculator calculator;

    public delegate void DeskEvent(UICardDesk desk);
    public event DeskEvent onSwitchDesk;

    public enum Tabs { Calculator, Database }

    public UITabSelector TabSelector { get; private set; }

    public UICardDesk[] Desks { get; private set; }
      

    public UIMain(string savePath)
    {
        UITranslator.filePath = savePath + @"\Translation.json";
        UITranslator.LoadCurrent();

        string[] tabNames = typeof(Tabs).GetEnumNames();
        for (int i = 0, iend = tabNames.Length; i < iend; i++)
            tabNames[i] = UITranslator.TranslateCurrent(tabNames[i]);
        TabSelector = new UITabSelector(tabNames);
        TabSelector.OnSelectionChange += OnTabSelectionChange;
        currentTab = TabSelector.CurrentSelection;

        Desks = new UICardDesk[3];
        Desks[(int)Tabs.Calculator] = NewCalculatorDesk(out calculatorInputCard, out calculatorOutputCard);
        Desks[(int)Tabs.Calculator].onEndEditCard += OnEndEditCalulatorInputCard;

        databaseSavePath = savePath + @"\Database.json";
        Database currentDatabase = DatapackSerializer.Deserialize(databaseSavePath);
        Desks[(int)Tabs.Database] = NewDatabaseDesk(currentDatabase, out databaseCard);
        Desks[(int)Tabs.Database].onEndEditCard += OnEndEditDatabaseCard;
        DatabaseReferenceAttribute.CurrentDatabase = currentDatabase;

        calculator = new Calculator();
    }

    ~UIMain()
    {
        Desks[(int)Tabs.Database].onEndEditCard -= OnEndEditDatabaseCard;
        Desks[(int)Tabs.Calculator].onEndEditCard -= OnEndEditCalulatorInputCard;
        TabSelector.OnSelectionChange -= OnTabSelectionChange;
    }

    private static UICardDesk NewDatabaseDesk(Database database, out UICard dataBaseCard)
    {
        UICardPile[] cardPiles = new UICardPile[] { new UICardPile(), new UICardPile() };
        dataBaseCard = UICard.NewCard(typeof(Database), false, false, database);
        cardPiles[0].PileCard(dataBaseCard);
        return new UICardDesk(cardPiles);
    }

    private static UICardDesk NewCalculatorDesk(out UICard inputCard, out UICard outputCard)
    {
        UICardPile[] cardPiles = new UICardPile[] { new UICardPile(), new UICardPile() };
        inputCard = UICard.NewCard(typeof(CalculatorInput), false, false);
        cardPiles[0].PileCard(inputCard);
        outputCard = UICard.NewCard(typeof(CalculatorOutput), true, false);
        cardPiles[1].PileCard(outputCard);
        return new UICardDesk(cardPiles);
    }

    private void OnTabSelectionChange(int newSelection)
    {
        if (Desks != null && newSelection >= 0 && newSelection < Desks.Length)
        {
            if (Desks[currentTab] != null)
                Desks[currentTab].CloseAllCards();
            currentTab = newSelection;
            onSwitchDesk?.Invoke(Desks[newSelection]);
            if (currentTab == (int)Tabs.Calculator) RefreshCalculator();
                
        }
    }

    private void OnEndEditDatabaseCard(UICardDesk desk, UICardPile pile, UICard card)
    {
        if (card != null && card == databaseCard)
        {
            Database editedData = databaseCard.Data as Database;
            DatapackSerializer.Serialize(editedData, databaseSavePath);
            databaseCard.Data = editedData;
            //onSaveDatabase?.Invoke((Database)card.Data);
        }
    }

    private void OnEndEditCalulatorInputCard(UICardDesk desk, UICardPile pile, UICard card)
    {
        if (card != null && card == calculatorInputCard)
            RefreshCalculator();
    }

    private void RefreshCalculator()
    {
        calculator.Input = (CalculatorInput)calculatorInputCard.Data;
        calculatorOutputCard.Data = calculator.Output;
    }
}
