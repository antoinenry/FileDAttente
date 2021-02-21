using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public abstract class UIFieldBuilder : MonoBehaviour
{
    public Text labelComponent;
    public Dropdown dropdownComponent;

    public UnityEvent onEdit;
    public UnityEvent onEndEdit;

    public UIField CurrentField
    {
        get => currentField;
        set => Build(value);
    }

    private UIField currentField;

    protected virtual void OnEnable()
    {
        if (dropdownComponent != null)
            dropdownComponent.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    protected virtual void OnDisable()
    {
        if (dropdownComponent != null)
            dropdownComponent.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    protected virtual void Build(UIField f)
    {
        RemoveCurrentFieldListeners();

        if (f != null && f.GetType() == GetUIFieldType())
        {            
            name = f.ToString();
            currentField = f;
            AddCurrentFieldListeners();
            currentField.SetInputFromDatabase();
            UpdateLabelComponent();
            UpdateInputComponent();
            UpdateDropdownComponent();
        }
    }

    public void UpdateLabelComponent()
    {
        if (labelComponent != null)
        {
            labelComponent.text = (CurrentField != null && CurrentField.label != null) ? CurrentField.label : "NewField";
        }
    }

    public void UpdateDropdownComponent()
    {
        if (dropdownComponent != null)
        {
            string[] itemOptionNames = null;
            if (CurrentField.ReadOnly == false)
            {
                DatabaseReferenceAttribute databaseReference = CurrentField != null ? CurrentField.GetDatabaseReferenceAttribute() : null;
                if (databaseReference != null)
                {
                    dropdownComponent.gameObject.SetActive(true);
                    itemOptionNames = databaseReference.GetSourceIDs();
                }
            }

            if (itemOptionNames != null)
            {
                dropdownComponent.options = new List<Dropdown.OptionData>(Array.ConvertAll(itemOptionNames, s => new Dropdown.OptionData(s)));
                int indexInDatabase = CurrentField.GetDataIndexInDatabase();
                if (indexInDatabase == -1)
                {
                    dropdownComponent.options.Add(new Dropdown.OptionData("..."));
                    dropdownComponent.SetValueWithoutNotify(dropdownComponent.options.Count - 1);
                }
                else
                    dropdownComponent.SetValueWithoutNotify(indexInDatabase);
                dropdownComponent.RefreshShownValue();
            }
            else
                dropdownComponent.gameObject.SetActive(false);
        }
    }

    public abstract void UpdateInputComponent();

    public abstract Type GetUIFieldType();

    protected abstract void OnSetDatabaseIndex(UIField f);

    protected virtual void OnDropdownValueChanged(int value)
    {
        if (CurrentField != null)
        {
            CurrentField.SetInputFromDatabase(value);
            UpdateInputComponent();
            UpdateDropdownComponent();
        }
    }

    protected virtual void OnFieldChange(UIField f)
    {
        UpdateDropdownComponent();        
    }

    private void AddCurrentFieldListeners()
    {
        if (currentField == null) return;
        currentField.onApplyChanges += OnFieldChange;
        currentField.onSetDatabaseID += OnSetDatabaseIndex;
    }

    private void RemoveCurrentFieldListeners()
    {
        if (currentField == null) return;
        currentField.onApplyChanges -= OnFieldChange;
        currentField.onSetDatabaseID -= OnSetDatabaseIndex;
    }
}
