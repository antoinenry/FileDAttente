using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UITabSelectorBuilder : MonoBehaviour
{
    public Text activeTabLabel;
    public Button[] tabButtons;

    public UITabSelector CurrentSelector { get; private set; }
    public UnityAction[] buttonActions;

    private void Awake()
    {
        int buttonCount = tabButtons.Length;
        buttonActions = new UnityAction[buttonCount];
        for (int i = 0; i < buttonCount; i++)
        {
            int buttonIndex = i;
            buttonActions[i] = new UnityAction(() => OnPressButton(buttonIndex));
        }
    }

    private void OnEnable()
    {
        for (int i = 0, iend = tabButtons.Length; i < iend; i++)
            tabButtons[i].onClick.AddListener(buttonActions[i]);
    }

    private void OnDisable()
    {
        for (int i = 0, iend = tabButtons.Length; i < iend; i++)
            tabButtons[i].onClick.RemoveListener(buttonActions[i]);

    }

    public void Build(UITabSelector tabSelector)
    {
        CurrentSelector = tabSelector;
        SetTexts();
    }

    private void OnPressButton(int buttonIndex)
    {
        if (CurrentSelector != null)
        {
            if (buttonIndex < CurrentSelector.CurrentSelection)
                CurrentSelector.CurrentSelection = buttonIndex;
            else
                CurrentSelector.CurrentSelection = buttonIndex + 1;

            SetTexts();
        }
    }

    private void SetTexts()
    {
        if (CurrentSelector != null && CurrentSelector.tabNames != null)
        {
            int buttonIndex = 0;
            for (int i = 0, iend = CurrentSelector.tabNames.Length; i < iend; i++)
            {
                if (i == CurrentSelector.CurrentSelection)
                    activeTabLabel.text = CurrentSelector.tabNames[i];
                else
                    tabButtons[buttonIndex++].GetComponentInChildren<Text>().text = CurrentSelector.tabNames[i];

            }
        }
    }
}
