using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class UITranslator
{
    public static UITranslator currentTranslator;
    public static string filePath;

    [Serializable]
    public class Translation
    {
        public string original;
        public string translated;
    }

    public List<Translation> translations;

    public static string TranslateCurrent(string word)
    {
        if (currentTranslator == null) currentTranslator = new UITranslator();
        return currentTranslator.Translate(word);
    }

    public static void SaveCurrent()
    {
        if (currentTranslator == null) currentTranslator = new UITranslator();
        currentTranslator.Save();
    }

    public static void LoadCurrent()
    {
        if (File.Exists(filePath) && filePath.EndsWith(".json"))
        {
            string jsonString = File.ReadAllText(filePath, Encoding.GetEncoding(28591));
            currentTranslator = JsonUtility.FromJson<UITranslator>(jsonString);
            if (currentTranslator == null) currentTranslator = new UITranslator();
        }
        else
            SaveCurrent();
    }

    private string Translate(string word)
    {
        if (translations == null) translations = new List<Translation>();
        int translationIndex = translations.FindIndex(t => t.original == word);
        if (translationIndex != -1)
        {
            if (translations[translationIndex].translated != "?") return translations[translationIndex].translated;
        }
        else
        {
            translations.Add(new Translation() { original = word, translated = "?" });
            Save();
        }

        return word;
    }

    private void Save()
    {
        if (translations == null) translations = new List<Translation>();
        string jsonString = JsonUtility.ToJson(this, true); ;
        File.WriteAllText(filePath, jsonString, Encoding.GetEncoding(28591));
    }
}
