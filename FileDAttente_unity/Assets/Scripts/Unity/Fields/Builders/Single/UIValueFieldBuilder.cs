using System;
using UnityEngine;
using UnityEngine.UI;

public class UIValueFieldBuilder : UIFieldBuilder
{
    public InputField inputComponent;
    public Color databaseInputColor = Color.yellow;

    private Color defaultInputColor;

    private void Awake()
    {
        if (inputComponent != null)
            defaultInputColor = inputComponent.textComponent.color;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (inputComponent != null)
        {
            inputComponent.onEndEdit.AddListener(OnFieldEndEdit);
            inputComponent.onValueChanged.AddListener(OnFieldValueChange);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (inputComponent != null)
        {
            inputComponent.onEndEdit.RemoveListener(OnFieldEndEdit);
            inputComponent.onValueChanged.AddListener(OnFieldValueChange);
        }
    }

    private void OnFieldValueChange(string stringValue)
    {
        if (CurrentField == null || (CurrentField is UIValueField == false)) return;
        UIValueField currentValueField = CurrentField as UIValueField;
        if (currentValueField.StringInput != stringValue)
        {
            onEdit.Invoke();
            CurrentField.Edit();
        }
    }

    private void OnFieldEndEdit(string stringValue)
    {
        if (CurrentField == null || (CurrentField is UIValueField == false)) return;
        UIValueField currentValueField = CurrentField as UIValueField;
        if (currentValueField.StringInput != stringValue)
        {
            //currentValueField.StringInput = stringValue;
            currentValueField.SetInput(stringValue);
            onEndEdit.Invoke();
            CurrentField.ApplyChanges();
            CurrentField.EndEdit();
        }
    }

    public override void UpdateInputComponent()
    {
        if (CurrentField != null && CurrentField is UIValueField && inputComponent != null)
        {
            if (CurrentField.FieldType == typeof(int))
                inputComponent.contentType = InputField.ContentType.IntegerNumber;
            else if (CurrentField.FieldType == typeof(float))
                inputComponent.contentType = InputField.ContentType.DecimalNumber;
            else
                inputComponent.contentType = InputField.ContentType.Standard;

            inputComponent.text = (CurrentField as UIValueField).StringInput;
            inputComponent.interactable = !CurrentField.ReadOnly;
            UpdateInputFieldColor();
        }
    }

    protected override void OnSetDatabaseIndex(UIField f)
    {
        UpdateInputFieldColor();
    }

    private void UpdateInputFieldColor()
    {
        if (CurrentField != null && inputComponent != null)
            inputComponent.textComponent.color = CurrentField.DatabaseID == null ? defaultInputColor : databaseInputColor;
    }

    public override Type GetUIFieldType()
    {
        return typeof(UIValueField);
    }
}