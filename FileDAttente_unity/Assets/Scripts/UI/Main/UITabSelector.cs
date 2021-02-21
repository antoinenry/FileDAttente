using System;

public class UITabSelector
{
    public delegate void SelectionEvent(int selection);
    public event SelectionEvent OnSelectionChange;

    public readonly string[] tabNames;

    private int currentSelection;

    public int CurrentSelection
    {
        get => currentSelection;
        set
        {
            if (currentSelection != value)
            {
                currentSelection = value;
                OnSelectionChange?.Invoke(currentSelection);
            }
        }
    }

    public UITabSelector(string[] tabs)
    {
        tabNames = tabs;
    }
}
