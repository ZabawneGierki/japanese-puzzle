using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class VocabularyLoader : MonoBehaviour
{
    public string fileName = "n5.xml"; // must be inside Assets/Resources

    private VocabularyList vocabList;

    void Awake()
    {
        LoadVocabulary();
        ShuffleVocabulary();
    }

    void LoadVocabulary()
    {
        TextAsset xmlData = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(fileName));
        if (xmlData == null)
        {
            Debug.LogError("XML file not found in Resources: " + fileName);
            return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(VocabularyList));
        using (StringReader reader = new StringReader(xmlData.text))
        {
            vocabList = serializer.Deserialize(reader) as VocabularyList;
        }
    }

    void PrintAllWords()
    {
        if (vocabList == null) return;

        foreach (var word in vocabList.Words)
        {
            Debug.Log($"Kanji: {word.kanji}, Kana: {word.kana}, " +
                      $"Translation: {word.translation}, " +
                      $"JP: {word.jp}, EN: {word.en}");
        }
    }

    /// <summary>
    /// Shuffle the loaded vocabulary list (Fisher–Yates algorithm).
    /// </summary>
    public void ShuffleVocabulary()
    {
        if (vocabList == null || vocabList.Words.Count == 0) return;

        System.Random rng = new System.Random();
        int n = vocabList.Words.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            WordEntry value = vocabList.Words[k];
            vocabList.Words[k] = vocabList.Words[n];
            vocabList.Words[n] = value;
        }
    }

    /// <summary>
    /// Get a list of N words (randomized).
    /// </summary>
    public List<WordEntry> GetWords(int count)
    {
        if (vocabList == null)
        {
            Debug.LogWarning("Vocabulary not loaded.");
            return new List<WordEntry>();
        }

        count = Mathf.Clamp(count, 0, vocabList.Words.Count);
        return vocabList.Words.GetRange(0, count);
    }
}

