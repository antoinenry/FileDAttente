//using System;
//using System.Reflection;

//[Serializable]
//public class DatabaseItemAttribute : Attribute
//{
//    private string itemArrayFieldNameInDatabase;

//    public static Database CurrentDatabase;

//    private Type DataType
//    {
//        get
//        {
//            if (CurrentDatabase != null)
//            {
//                FieldInfo fieldInfo = typeof(Database).GetField(itemArrayFieldNameInDatabase);
//                if (fieldInfo != null && fieldInfo.FieldType != null && fieldInfo.FieldType.IsArray)
//                    return fieldInfo.FieldType.GetElementType();
//            }
//            return null;
//        }
//    }

//    public DatabaseItemAttribute(string fieldNameInDatabase)
//    {
//        itemArrayFieldNameInDatabase = fieldNameInDatabase;
//    }

//    public object GetAllDatabaseItems()
//    {
//        if (CurrentDatabase == null) return false;
//        FieldInfo fieldInfo = typeof(Database).GetField(itemArrayFieldNameInDatabase);
//        if (fieldInfo == null || fieldInfo.FieldType == null) return false;
//        if (fieldInfo.FieldType.IsArray)
//            return fieldInfo.GetValue(CurrentDatabase);
//        else
//            return null;
//    }

//    public string[] GetAllDatabaseItemIDs()
//    {
//        Type dataType = DataType;
//        if (dataType != null)
//            return typeof(DatabaseItemAttribute).GetMethod("GetAllDatabaseItemIDs", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, null) as string[];
//        else
//            return null;
//    }

//    private string[] GetAllDatabaseItemIDs<T>()
//    {
//        object itemArray = GetAllDatabaseItems();
//        if (itemArray != null)
//        {
//            T[] items = itemArray as T[];
//            return Array.ConvertAll(items, item => GetItemID(item));
//        }
//        return null;
//    }

//    public bool TryGetDatabaseItem(string id, out object item)
//    {
//        item = null;
//        if (id == null) return false;
//        Type dataType = DataType;
//        if (dataType != null)
//        {
//            if (GetIndexInDatabase(id) != -1)
//            {
//                item = typeof(DatabaseItemAttribute).GetMethod("GetDatabaseItemByID", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, new object[] { id });
//                return true;
//            }
//        }

//        return false;
//    }

//    private object GetDatabaseItemByID<T>(string id)
//    {
//        if (id != null)
//        {
//            object itemArray = GetAllDatabaseItems();
//            if (itemArray != null)
//            {
//                T[] items = itemArray as T[];
//                return Array.Find(items, x => GetItemID(x) == id);
//            }
//        }

//        //item = null;
//        return false;
//    }

//    public bool TryGetDatabaseItem(int index, out object item)
//    {
//        item = null;
//        Type dataType = DataType;
//        if (dataType != null)
//        {
//            int itemCount = (int)typeof(DatabaseItemAttribute).GetMethod("GetDatabaseItemCount", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, null);
//            if (index >= 0 && index < itemCount)
//            {
//                item = typeof(DatabaseItemAttribute).GetMethod("GetDatabaseItemByIndex", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, new object[] { index });
//                return true;
//            }
//        }

//        return false;
//    }

//    private object GetDatabaseItemByIndex<T>(int index)
//    {
//        object itemArray = GetAllDatabaseItems();
//        if (itemArray != null)
//        {
//            T[] items = itemArray as T[];
//            if (items != null && index >= 0 && index < items.Length)
//                return items[index];
//        }

//        //item = null;
//        return false;
//    }

//    private int GetDatabaseItemCount<T>()
//    {
//        object itemArray = GetAllDatabaseItems();
//        if (itemArray != null)
//        {
//            T[] items = itemArray as T[];
//            if (items != null) return items.Length;
//        }
//        return -1;
//    }

//    public int GetIndexInDatabase(string id)
//    {
//        if (id == null) return -1;
//        Type dataType = DataType;
//        if (dataType != null)
//            return (int)typeof(DatabaseItemAttribute).GetMethod("GetIndexInDatabase", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(dataType).Invoke(this, new object[] { id });
//        else
//            return -1;
//    }

//    private int GetIndexInDatabase<T>(string id)
//    {
//        if (id == null) return -1;
//        object itemArray = GetAllDatabaseItems();
//        if (itemArray != null)
//        {
//            T[] items = itemArray as T[];
//            return Array.FindIndex(items, item => GetItemID(item) == id);
//        }
//        return -1;
//    }

//    public static string GetItemID(object item)
//    {
//        if (item == null) return null;
//        if (item is IDatapack) return (item as IDatapack).DataID;
//        else return item.ToString();
//    }
//}