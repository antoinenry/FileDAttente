using System;
using System.Reflection;

[Serializable]
public class UIField
{
    public string label;

    public enum InputType { None, Value, Data }

    public delegate void FieldEvent(UIField field);
    public event FieldEvent onStartEdit;
    public event FieldEvent onEdit;
    public event FieldEvent onEndEdit;
    public event FieldEvent onApplyChanges;
    public event FieldEvent onSetDatabaseID;

    public FieldInfo Info { get; private set; }
    public int ArrayLevel { get; private set; }
    public string DatabaseID { get; private set; }
    public bool ReadOnly { get; private set; }

    public Type FieldType
    {
        get
        {
            if (Info == null) return null;
            DatabaseReferenceAttribute databaseReference = GetDatabaseReferenceAttribute();
            if (databaseReference == null)
                return GetElementType(Info.FieldType, ArrayLevel);
            else
                return GetElementType(databaseReference.dataType, ArrayLevel);
        }
    }

    public static UIField NewField(FieldInfo info, int arrayLevel, bool makeReadOnly)
    {
        if (info == null) return null;
        Type fieldType = GetElementType(info.FieldType, arrayLevel);       
        if (fieldType == null) return null;
        UIField newField;
        bool isArray = fieldType.IsArray;
        DatabaseReferenceAttribute databaseReference = GetDatabaseReferenceAttribute(info);

        if (databaseReference != null)
            fieldType = databaseReference.dataType;

        if (isArray == false)
        {
            InputType inputType = GetInputType(fieldType);
            switch (inputType)
            {
                case InputType.Value: newField = new UIValueField(); break;
                case InputType.Data: newField = new UIDataPackField(); break;
                default: return null;
            }
        }
        else
        {
            Type arrayType = isArray ? fieldType.GetElementType() : null;
            InputType inputType = GetInputType(arrayType);
            switch (inputType)
            {
                case InputType.Value: newField = new UIValueArrayField(); break;
                case InputType.Data: newField = new UIDataPackArrayField(); break;
                default: return null;
            }
        }

        newField.Info = info;
        newField.ArrayLevel = arrayLevel;
        newField.label = UITranslator.TranslateCurrent(info.Name);
        newField.DatabaseID = null;
        newField.ReadOnly = makeReadOnly;// || databaseReference != null;
        return newField;
    }

    public virtual void StartEdit() => onStartEdit?.Invoke(this);

    public void Edit()
    {
        //if (DatabaseID != null)
        //{
        //    DatabaseID = null;
        //    onSetDatabaseID?.Invoke(this);
        //}
        onEdit?.Invoke(this);
    }

    public void EndEdit() => onEndEdit?.Invoke(this);

    public virtual void Destroy() { }

    public virtual void ApplyChanges()
    {
        //if (DatabaseID != null)
        //{
        //    DatabaseID = null;
        //    onSetDatabaseID?.Invoke(this);
        //}
        onApplyChanges?.Invoke(this);
    }

    public override string ToString()
    {
        return "UIField_" + label;
    }

    public void SetDefaultInput(bool isArray)
    {
        object[] args = new object[] { isArray };
        typeof(UIField).GetMethod("SetDefaultInput", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(FieldType).Invoke(this, args);
    }

    private void SetDefaultInput<T>(bool isArray)
    {
        if (isArray)
            SetInput(new T[0]);
        else
            SetInput(default(T));
    }

    public virtual void SetInput(object input)
    {
        SetDatabaseID(DatabaseReferenceAttribute.GetItemID(input));
    }

    public virtual object ApplyField(object fieldValue, out bool changeCheck)
    {
        SetDatabaseID(DatabaseReferenceAttribute.GetItemID(fieldValue));
        changeCheck = true;
        return fieldValue;
    }

    public DatabaseReferenceAttribute GetDatabaseReferenceAttribute()
    {
        return GetDatabaseReferenceAttribute(Info);
    }

    public static DatabaseReferenceAttribute GetDatabaseReferenceAttribute(FieldInfo info)
    {
        if (info == null) return null;
        object[] attributes = info.GetCustomAttributes(false);
        if (attributes == null) return null;
        return Array.Find(attributes, a => a is DatabaseReferenceAttribute) as DatabaseReferenceAttribute;
    }

    private static InputType GetInputType(Type fieldType)
    {
        if (fieldType == typeof(int) || fieldType == typeof(float) || fieldType == typeof(string))
            return InputType.Value;
        else if (typeof(IDatapack).IsAssignableFrom(fieldType))
            return InputType.Data;
        else
            return InputType.None;
    }

    private static Type GetElementType(Type arrayType, int arrayLevel)
    {
        if (arrayType == null || arrayLevel < 0) return null;
        Type elementType = arrayType;
        for (int i = 0; i < arrayLevel; i++)
        {
            if (elementType == null || elementType.IsArray == false) return null;
            elementType = elementType.GetElementType();
        }
        return elementType;
    }

    protected void SetDatabaseID(string newID)
    {
        DatabaseReferenceAttribute databaseReference = GetDatabaseReferenceAttribute();
        if (databaseReference == null || databaseReference.GetIndexInDatabase(newID) == -1)
            newID = null;

        if (DatabaseID != newID)
        {
            DatabaseID = newID;
            onSetDatabaseID?.Invoke(this);
        }
    }

    public bool SetInputFromDatabase(string dataID)
    {
        DatabaseReferenceAttribute databaseReference = GetDatabaseReferenceAttribute();
        if (databaseReference == null) return false;
        bool getItem = databaseReference.TryGetSourceItem(dataID, out object databaseItem);
        if (getItem == true)
        {
            SetInput(databaseItem);
            return true;
        }
        else
            return false;
    }

    public virtual bool SetInputFromDatabase(int indexInDatabase)
    {
        if (indexInDatabase < 0) return false;
        DatabaseReferenceAttribute databaseReference = GetDatabaseReferenceAttribute();
        if (databaseReference == null) return false;
        bool getItem = databaseReference.TryGetSourceItem(indexInDatabase, out object item);
        if (getItem == true)
        {
            SetInput(item);
            return true;
        }
        else
            return false;
    }

    public void SetInputFromDatabase()
    {
        if (DatabaseID != null)
        {
            if (SetInputFromDatabase(DatabaseID) == false)
                DatabaseID = null;
        }
    }

    public int GetDataIndexInDatabase()
    {
        DatabaseReferenceAttribute databaseItemAttribute = GetDatabaseReferenceAttribute();
        if (databaseItemAttribute == null) return -1;
        return databaseItemAttribute.GetIndexInDatabase(DatabaseID);
    }
}
