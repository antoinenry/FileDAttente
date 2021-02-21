using System;
using System.Reflection;
using System.Collections.Generic;

[Serializable]
public class UIArrayField : UIField
{
    public int selectionIndex;

    public List<UIField> ItemFields { get; private set; }

    public int ItemCount
    {
        get => ItemFields != null ? ItemFields.Count : 0;
    }

    public UIField SelectedField
    {
        get
        {
            if (selectionIndex >= 0 && selectionIndex < ItemCount)
                return ItemFields[selectionIndex];
            else
                return null;
        }
    }

    //public override void SetInput(object input)
    //{
    //    if (input != null && input.GetType().IsArray)
    //    {
    //        IEnumerable<object> array = (IEnumerable<object>)input;
    //        int i = 0;
    //        foreach (object item in array)
    //        {
    //            if (i >= ItemCount) break;
    //            UIField itemField = ItemFields[i];
    //            if (itemField != null && itemField is UIValueField)
    //                itemField.SetInput(item);
    //        }
    //    }
    //}

    public override void SetInput(object input)
    {
        Type elementType = FieldType != null && FieldType.IsArray ? FieldType.GetElementType() : null;
        if (elementType != null)
        {
            object[] args = new object[] { input };
            typeof(UIArrayField).GetMethod("SetFieldInput", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(elementType).Invoke(this, args);
            base.SetInput(input);
        }
        else
            throw new Exception("InputType should never be null");
    }

    private void SetFieldInput<T>(object input)
    {
        T[] inputArray = input != null ? (T[])input : null;
        if (inputArray != null)
        {
            int valueCount = inputArray.Length;
            if (ItemFields == null)
                ItemFields = new List<UIField>(valueCount);
            for (int i = 0; i < valueCount; i++)
            {
                if (i < ItemCount)
                    ItemFields[i].SetInput(inputArray[i]);
                else
                    AddField(inputArray[i]);

                //UIField itemField;
                //itemField = i < ItemCount ? ItemFields[i] : AddField();
                //itemField.SetInput(inputArray[i]);
            }
            for (int i = ItemCount - 1; i >= valueCount; i--)
                RemoveFieldAt(i);
            
        }
        else
            ItemFields = new List<UIField>();
    }

    public override object ApplyField(object fieldValue, out bool changeCheck)
    {
        object modifiedFieldValue = fieldValue;
        changeCheck = true;

        Type elementType = FieldType != null && FieldType.IsArray ? FieldType.GetElementType() : null;
        if (elementType != null)
        {
            object[] args = new object[] { fieldValue };
            modifiedFieldValue = typeof(UIArrayField).GetMethod("ApplyField", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(elementType).Invoke(this, args);            
        }
        else
            ItemFields = new List<UIField>();

        return modifiedFieldValue;
    }

    private object ApplyField<T>(object fieldValue)
    {
        int valueCount = ItemFields != null ? ItemFields.Count : 0;
        T[] fieldValues = fieldValue != null ? (T[])fieldValue : new T[valueCount];
        T[] modifiedFieldValues = new T[valueCount];

        for (int i = 0; i < valueCount; i++)
        {
            object fieldObject;
            if (i < fieldValues.Length)
                fieldObject = ItemFields[i].ApplyField(fieldValues[i], out bool changeCheck_i);
            else
                fieldObject = ItemFields[i].ApplyField(null, out bool changeCheck_i);

            if (fieldObject != null)
                modifiedFieldValues[i] = (T)fieldObject;
            else
                modifiedFieldValues[i] = default(T);
        }

        return modifiedFieldValues;
    }

    public UIField AddField()
    {
        //UIField newField = NewField(FieldType.GetElementType());
        UIField newField = NewField(Info, ArrayLevel + 1, ReadOnly);
        AddItemField(newField);
        return newField;
    }

    public UIField AddField(object fieldInput)
    {
        //UIField newField = NewField(FieldType.GetElementType());
        UIField newField = NewField(Info, ArrayLevel + 1, ReadOnly);
        newField.SetInput(fieldInput);
        AddItemField(newField);
        return newField;
    }

    private void AddItemField(UIField newField)
    {
        Edit();
        if (newField != null)
        {
            int newFieldIndex = ItemCount;
            newField.label = "#" + newFieldIndex.ToString();
            AddFieldListeners(newField);
        }
        ItemFields.Add(newField);
        ApplyChanges();
        //EndEdit();
    }

    public bool RemoveFieldAt(int removeIndex)
    {
        if (removeIndex < 0 || removeIndex >= ItemCount) return false;
        Edit();
        UIField removedField = ItemFields[removeIndex];
        if (removedField != null)
        {
            removedField.Destroy();
            //removedField.EndEdit();
            RemoveFieldListeners(removedField);
        }
        ItemFields.RemoveAt(removeIndex);
        UpdateItemLabels();
        ApplyChanges();
        EndEdit();
        return true;
    }

    public bool MoveField(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= ItemCount) return false;
        if (toIndex < 0 || toIndex >= ItemCount) return false;
        Edit();
        UIField movedField = ItemFields[fromIndex];
        ItemFields.RemoveAt(fromIndex);
        ItemFields.Insert(toIndex, movedField);
        UpdateItemLabels();
        ApplyChanges();
        EndEdit();
        return true;
    }

    private void AddFieldListeners(UIField f)
    {
        if (f == null) return;
        f.onStartEdit += OnStartEditField;
        f.onEdit += OnEditItemField;
        f.onEndEdit += OnEndEditItemField;
        f.onApplyChanges += OnApplyChangeOnItemField;
    }

    private void RemoveFieldListeners(UIField f)
    {
        if (f == null) return;
        f.onStartEdit -= OnStartEditField;
        f.onEdit -= OnEditItemField;
        f.onEndEdit -= OnEndEditItemField;
        f.onApplyChanges -= OnApplyChangeOnItemField;
    }

    private void UpdateItemLabels()
    {
        if (ItemCount == 0) return;
        int itemCounter = 0;
        foreach (UIField field in ItemFields)
            if (field != null)
                field.label = "#" + itemCounter++.ToString();
    }

    private void OnStartEditField(UIField itemField)
    {
        StartEdit();
    }

    private void OnEditItemField(UIField itemField)
    {
        Edit();
    }

    private void OnEndEditItemField(UIField itemField)
    {
        //ApplyChanges();
        EndEdit();
    }

    private void OnApplyChangeOnItemField(UIField itemField)
    {
        base.ApplyChanges();
    }

    public override void ApplyChanges()
    {
        if (ItemFields != null)
        {
            foreach (UIField itemField in ItemFields)
                if (itemField != null) itemField.ApplyChanges();
        }

        base.ApplyChanges();
    }

    public override void Destroy()
    {
        if (ItemFields != null)
        {
            foreach (UIField itemField in ItemFields)
                if (itemField != null) itemField.Destroy();
        }
    }
}
