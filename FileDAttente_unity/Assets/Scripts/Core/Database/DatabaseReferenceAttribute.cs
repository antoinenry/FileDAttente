using System;
using System.Reflection;

public class DatabaseReferenceAttribute : Attribute
{
    public static Database CurrentDatabase;

    public readonly Type dataType;
    public readonly string sourceField;

    public DatabaseReferenceAttribute(Type t, string source)
    {
        dataType = t;
        sourceField = source;
    }

    public string GetCompleteID(string shortID)
    {
        return dataType.AssemblyQualifiedName + "/" + shortID;
    }

    public string GetShortID(string completeID)
    {
        string typeString = dataType.AssemblyQualifiedName;
        if (completeID.StartsWith(typeString))
            return completeID.Substring(typeString.Length + 1);
        else
            return null;
    }

    public bool TryGetSourceArray(out object sourceArray)
    {
        sourceArray = null;
        if (CurrentDatabase == null) return false;
        FieldInfo fieldInfo = typeof(Database).GetField(sourceField);
        if (fieldInfo == null || fieldInfo.FieldType == null) return false;
        if (fieldInfo.FieldType.IsArray)
        {
            sourceArray = fieldInfo.GetValue(CurrentDatabase);
            if (sourceArray != null)
                return true;
        }
        return false;
    }

    public string[] GetSourceIDs()
    {
        if (dataType != null)
            return typeof(DatabaseReferenceAttribute).GetMethod("GetSourceIDs", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, null) as string[];
        else
            return null;
    }

    private string[] GetSourceIDs<T>()
    {
        bool getArray = TryGetSourceArray(out object itemArray);
        if (getArray == true)
        {
            T[] items = itemArray as T[];
            return Array.ConvertAll(items, item => GetItemID(item));
        }
        return null;
    }

    public static string GetItemID(object item)
    {
        if (item == null) return null;
        if (item is IDatapack) return (item as IDatapack).DataID;
        else return item.ToString();
    }

    public static bool TryGetSourceItem<T>(string sourceField, string id, out T item)
    {
        if (new DatabaseReferenceAttribute(typeof(T), sourceField).TryGetSourceItem(id, out object itemObject) == true)
        {
            item = (T)itemObject;
            return true;
        }
        else
        {
            item = default(T);
            return false;
        }
    }

    public bool TryGetSourceItem(string id, out object item)
    {
        item = null;
        if (id == null) return false;
        if (dataType != null)
        {
            if (GetIndexInDatabase(id) != -1)
            {
                item = typeof(DatabaseReferenceAttribute).GetMethod("GetSourceItemByID", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, new object[] { id });
                return true;
            }
        }

        return false;
    }

    private object GetSourceItemByID<T>(string id)
    {
        if (id != null)
        {
            bool getArray = TryGetSourceArray(out object itemArray);
            if (getArray == true)
            {
                T[] items = itemArray as T[];
                return Array.Find(items, x => GetItemID(x) == id);
            }
        }
        return false;
    }

    public int GetIndexInDatabase(string id)
    {
        if (id == null) return -1;
        if (dataType != null)
            return (int)typeof(DatabaseReferenceAttribute).GetMethod("GetIndexInSourceArray", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, new object[] { id });
        else
            return -1;
    }

    private int GetIndexInSourceArray<T>(string id)
    {
        if (id == null) return -1;
        bool getArray = TryGetSourceArray(out object itemArray);
        if (getArray == true)
        {
            T[] items = itemArray as T[];
            return Array.FindIndex(items, item => GetItemID(item) == id);
        }
        return -1;
    }


    public bool TryGetSourceItem(int index, out object item)
    {
        item = null;
        if (dataType != null)
        {
            int itemCount = (int)typeof(DatabaseReferenceAttribute).GetMethod("GetSourceItemCount", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, null);
            if (index >= 0 && index < itemCount)
            {
                item = typeof(DatabaseReferenceAttribute).GetMethod("GetSourceItemByIndex", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, new object[] { index });
                return true;
            }
        }

        return false;
    }

    private int GetSourceItemCount<T>()
    {
        bool getArray = TryGetSourceArray(out object itemArray);
        if (getArray == true)
        {
            T[] items = itemArray as T[];
            if (items != null) return items.Length;
        }
        return -1;
    }

    private object GetSourceItemByIndex<T>(int index)
    {
        bool getArray = TryGetSourceArray(out object itemArray);
        if (getArray == true)
        {
            T[] items = itemArray as T[];
            if (items != null && index >= 0 && index < items.Length)
                return items[index];
        }

        return false;
    }
}