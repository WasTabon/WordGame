using System.Collections.Generic;
using UnityEngine;

public static class Dictionary
{
    private static HashSet<string> wordSet;
    private static List<string> wordList;

    public static int WordCount
    {
        get
        {
            EnsureLoaded();
            return wordSet.Count;
        }
    }

    public static IReadOnlyList<string> AllWords
    {
        get
        {
            EnsureLoaded();
            return wordList;
        }
    }

    public static bool Contains(string word)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(word)) return false;
        return wordSet.Contains(word.ToUpperInvariant());
    }

    private static void EnsureLoaded()
    {
        if (wordSet != null) return;

        wordSet = new HashSet<string>();
        wordList = new List<string>();

        var asset = Resources.Load<TextAsset>("word_list");
        if (asset == null)
        {
            Debug.LogError("Dictionary: Resources/word_list.txt not found! Run 'WordGame > Setup Dictionary (Iteration 4)' to create it.");
            return;
        }

        var lines = asset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            var w = lines[i].Trim();
            if (w.Length == 0) continue;
            var upper = w.ToUpperInvariant();
            if (wordSet.Add(upper)) wordList.Add(upper);
        }
        Debug.Log("Dictionary loaded: " + wordSet.Count + " words.");
    }
}
