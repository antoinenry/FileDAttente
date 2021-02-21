using System;
using System.Reflection;

[Serializable]
public class UICard
{
    public readonly bool readOnly;
    public readonly bool closable;
    public UIField[] fields;

    public delegate void CardEvent(UICard card);
    public event CardEvent onEdit;
    public event CardEvent onEndEdit;
    public event CardEvent onFieldChange;
    public event CardEvent onCardRequest;
    public event CardEvent onClose;
    public event CardEvent onSave;
    public event CardEvent onSetData;

    private IDatapack data;

    public bool isEdited { get; private set; }

    public string HeaderText
    {
        get
        {
            string txt = "";
            if (GetDataType() != null) txt = GetDataType().ToString();
            txt = UITranslator.TranslateCurrent(txt);
            if (readOnly == true)
            {
                txt += " " + UITranslator.TranslateCurrent("(read only)");
            }
            return txt;
        }
    }

    public IDatapack Data
    {
        get => data;
        set
        {
            if (value == null) return;
            bool resetField = data == null || data.GetType() != value.GetType();
            data = value;
            if (resetField)
                SetFields();
            else
                ResetFieldInputs();

            onSetData?.Invoke(this);
            //onEdit?.Invoke(this);
        }
    }

    public int FieldCount => fields != null ? fields.Length : 0;

    private UICard(bool makeReadOnly, bool makeClosable)
    {
        readOnly = makeReadOnly;
        closable = makeClosable;
    }

    public static UICard NewCard(Type dataType, bool makeReadOnly, bool makeClosable)
    {
        if (typeof(IDatapack).IsAssignableFrom(dataType))
        {
            UICard newCard = new UICard(makeReadOnly, makeClosable);
            newCard.SetDataType(dataType);
            return newCard;
        }
        else return null;
    }

    public static UICard NewCard(Type dataType, bool makeReadOnly, bool makeClosable, IDatapack data)
    {
        if (typeof(IDatapack).IsAssignableFrom(dataType))
        {
            UICard newCard = new UICard(makeReadOnly, makeClosable);
            newCard.Data = data;
            if (data == null) newCard.SetDataType(dataType);
            return newCard;
        }
        else return null;
    }

    public Type GetDataType()
    {
        if (Data == null || Data.GetType() == typeof(IDatapack)) return null;
        else return Data.GetType();
    }

    public void SetDataType(Type dataType)
    {
        if (dataType != null && typeof(IDatapack).IsAssignableFrom(dataType))
            typeof(UICard).GetMethod("SetDataType", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, null);
        else
            Data = null;
    }

    public void Save()
    {
        foreach (UIField f in fields) f.ApplyChanges();
        onSave?.Invoke(this);
    }

    public void Close()
    {
        onClose?.Invoke(this);
        foreach (UIField f in fields) f.Destroy();
    }

    private void SetDataType<T>() where T : IDatapack, new()
    {
        Data = new T();
        //SetFields();
    }   

    private void SetFields()
    {
        ClearFields();
        Type dataType = GetDataType();
        if (dataType == null) return;
        FieldInfo[] fieldInfos = dataType.GetFields();
        int fieldCount = fieldInfos != null ? fieldInfos.Length : 0;
        fields = new UIField[fieldCount];
        for (int i = 0; i < fieldCount; i++)
        {
            fields[i] = UIField.NewField(fieldInfos[i], 0, readOnly);
            if (fields[i] != null)
            {
                SetFieldInput(fields[i]);
                //fields[i].SetInput(fieldInfos[i].GetValue(Data));
                fields[i].onStartEdit += OnStartEditField;
                fields[i].onEdit += OnEditField;
                fields[i].onEndEdit += OnEndEditField;
                fields[i].onApplyChanges += OnApplyFieldChanges;
            }
        }
    }

    private void ResetFieldInputs()
    {
        if (fields == null) return;
        Type dataType = GetDataType();
        if (dataType == null) return;
        //FieldInfo[] fieldInfos = dataType.GetFields();
        //int fieldCount = Math.Min(FieldCount, fieldInfos.Length);
        //for (int i = 0; i < fieldCount; i++)
        foreach(UIField field in fields)
            if (field != null)
                SetFieldInput(field);
        //for (int i = 0; i < fieldCount; i++)
        //if (fields[i] != null && fieldInfos[i] != null)
        //    fields[i].SetInput(fieldInfos[i].GetValue(Data));
    }

    private void SetFieldInput(UIField f)
    {
        if (f == null || fields == null) return;
        int fieldIndex = Array.IndexOf(fields, f);
        Type dataType = GetDataType();
        if (dataType != null)
        {
            FieldInfo[] fieldInfos = dataType.GetFields();
            if (fieldInfos != null && fieldInfos.Length > fieldIndex && fieldInfos[fieldIndex] != null)
            {
                FieldInfo fieldInfo = fieldInfos[fieldIndex];
                DatabaseReferenceAttribute databaseReference = UIField.GetDatabaseReferenceAttribute(fieldInfo);
                if (databaseReference == null)
                    f.SetInput(fieldInfo.GetValue(Data));
                else if (databaseReference.TryGetSourceItem((string)fieldInfo.GetValue(Data), out object sourceItem) == true)
                    f.SetInput(sourceItem);
            }
        }
    }

    private void ClearFields()
    {
        if (fields == null) return;
        foreach (UIField f in fields)
        {
            f.onStartEdit -= OnStartEditField;
            f.onEdit -= OnEditField;
            f.onEndEdit -= OnEndEditField;
            f.onApplyChanges -= OnApplyFieldChanges;
        }
        fields = null;
    }
    
    private void OnStartEditField(UIField f)
    {
        if (f is UIDataPackField)
        {
            SetFieldInput(f);
            UICard fieldCard = (f as UIDataPackField).InputCard;
            onCardRequest?.Invoke(fieldCard);
        }
        else if (f is UIDataPackArrayField)
        {
            UIDataPackArrayField dataPackArrayField = f as UIDataPackArrayField;
            UIField itemField = dataPackArrayField.SelectedField;
            if (itemField != null && itemField is UIDataPackField)
            {
                SetFieldInput(f);
                UICard fieldCard = (itemField as UIDataPackField).InputCard;
                onCardRequest?.Invoke(fieldCard);
            }
        }
    }

    private void OnEditField(UIField f)
    {
        isEdited = true;
        onEdit?.Invoke(this);
    }

    private void OnEndEditField(UIField f)
    {
        SetFieldInput(f);
        onEndEdit?.Invoke(this);
    }   
    
    private void OnApplyFieldChanges(UIField f)
    {
        int fieldIndex = Array.IndexOf(fields, f);
        Type dataType = GetDataType();
        if (Data != null)
        {
            FieldInfo[] fieldInfos = dataType.GetFields();
            if (fieldIndex >= 0 && fieldIndex < fieldInfos.Length)
            {
                FieldInfo fieldInfo = fieldInfos[fieldIndex];
                if (fieldInfo != null)
                {
                    object fieldValue = fieldInfo.GetValue(Data);
                    fieldValue = f.ApplyField(fieldValue, out bool changeCheck);
                    if (changeCheck)
                    {
                        if (UIField.GetDatabaseReferenceAttribute(fieldInfo) == null)
                            fieldInfo.SetValue(Data, fieldValue);
                        else
                        {
                            if (typeof(IDatapack).IsAssignableFrom(fieldValue.GetType()))
                                fieldInfo.SetValue(Data, (fieldValue as IDatapack).DataID);
                            else
                                fieldInfo.SetValue(Data, fieldValue.ToString());
                        }
                        onFieldChange?.Invoke(this);
                    }
                }
            }
        }
    }
}
