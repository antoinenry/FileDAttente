using UnityEngine;
using System.IO;

public static class DatapackSerializer
{
    public static void Serialize(IDatapack dataPack, string filePath)
    {        
        string jsonString = JsonUtility.ToJson(dataPack, true);
        File.WriteAllText(filePath, jsonString);
    }

    public static Database Deserialize(string filePath)
    {
        if (File.Exists(filePath) && filePath.EndsWith(".json"))
        {
            string jsonString = File.ReadAllText(filePath);
            Database database = JsonUtility.FromJson<Database>(jsonString);
            return database;
        }
        else
            return new Database();
    }
}
