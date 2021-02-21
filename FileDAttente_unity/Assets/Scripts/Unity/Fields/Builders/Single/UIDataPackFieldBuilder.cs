using System;
using UnityEngine;
using UnityEngine.UI;

public class UIDataPackFieldBuilder : UIFieldBuilder
{
    public Button buttonComponent;
    public Text textComponent;
    public Color databaseInputColor = Color.yellow;
    public Color readOnlyColor = Color.clear;

    private Color defaultInputColor;
    private Color defaultButtonColor;

    private void Awake()
    {
        if (textComponent != null)
            defaultInputColor = textComponent.color;
        if (buttonComponent != null)
            defaultButtonColor = buttonComponent.colors.normalColor;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (buttonComponent != null)
            buttonComponent.onClick.AddListener(OnClickButton);
        AddCurrentFieldListeners();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (buttonComponent != null)
            buttonComponent.onClick.RemoveListener(OnClickButton);
        RemoveCurrentFieldListeners();
    }

    protected override void Build(UIField f)
    {
        RemoveCurrentFieldListeners();
        base.Build(f);
        AddCurrentFieldListeners();
    }

    private void AddCurrentFieldListeners()
    {
        if (CurrentField == null) return;
        CurrentField.onStartEdit += OnStartEditField;
        CurrentField.onEndEdit += OnEndEditField;
    }

    private void RemoveCurrentFieldListeners()
    {
        if (CurrentField == null) return;
        CurrentField.onStartEdit -= OnStartEditField;
        CurrentField.onEndEdit -= OnEndEditField;
    }

    private void OnEndEditField(UIField f)
    {
        if (f == CurrentField) UpdateInputComponent();
    }

    private void OnStartEditField(UIField f)
    {
        if (textComponent != null)
            textComponent.text = "...";
    }

    private void OnClickButton()
    {
        if (CurrentField == null || (CurrentField is UIDataPackField == false)) return;
        CurrentField.StartEdit();
    }

    public override void UpdateInputComponent()
    {
        if (CurrentField == null || CurrentField is UIDataPackField == false) return;
        UIDataPackField dataPackField = CurrentField as UIDataPackField;
        if (textComponent != null)
        {
            if (dataPackField.InputCard == null)
                textComponent.text = dataPackField.dataInput != null ? dataPackField.dataInput.DisplayName : "";
            else
                textComponent.text = "...";
        }
        UpdateButtonColor();
    }

    protected override void OnSetDatabaseIndex(UIField f)
    {
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        if (CurrentField != null)
        {
            if (textComponent != null)
                textComponent.color = CurrentField.DatabaseID == null ? defaultInputColor : databaseInputColor;
            if (buttonComponent != null)
            {
                ColorBlock colors = buttonComponent.colors;
                colors.normalColor = CurrentField.ReadOnly ? readOnlyColor : defaultButtonColor;
                buttonComponent.colors = colors;
            }
        }
    }

    public override Type GetUIFieldType()
    {
        return typeof(UIDataPackField);
    }
}
