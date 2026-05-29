using System.Collections.Generic;
using UnityEngine;

public static class Dictionary
{
    private const string PLACEMENT_FILE = "word_list";
    private const string VALIDATION_FILE = "words";

    private static List<string> placementPool;
    private static HashSet<string> placementSet;
    private static HashSet<string> validationSet;
    private static bool placementLoaded;
    private static bool validationLoaded;

    public static List<string> AllWords
    {
        get
        {
            if (!placementLoaded) LoadPlacement();
            return placementPool;
        }
    }

    public static int PlacementPoolSize
    {
        get
        {
            if (!placementLoaded) LoadPlacement();
            return placementPool != null ? placementPool.Count : 0;
        }
    }

    public static int ValidationSetSize
    {
        get
        {
            if (!validationLoaded) LoadValidation();
            return validationSet != null ? validationSet.Count : 0;
        }
    }

    public static bool HasLargeValidationSet
    {
        get
        {
            if (!validationLoaded) LoadValidation();
            return validationSet != null && validationSet.Count > 1000;
        }
    }

    public static bool IsValidWord(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;
        string upper = word.ToUpperInvariant();

        if (!validationLoaded) LoadValidation();
        if (validationSet != null && validationSet.Count > 0)
        {
            return validationSet.Contains(upper);
        }

        if (!placementLoaded) LoadPlacement();
        if (placementSet == null) return false;
        return placementSet.Contains(upper);
    }

    public static void Preload()
    {
        if (!placementLoaded) LoadPlacement();
        if (!validationLoaded) LoadValidation();
    }

    private static void LoadPlacement()
    {
        placementLoaded = true;
        var asset = Resources.Load<TextAsset>(PLACEMENT_FILE);
        if (asset == null)
        {
            Debug.LogError("[Dictionary] word_list.txt not found in Resources! Generator will fail.");
            placementPool = new List<string>();
            placementSet = new HashSet<string>();
            return;
        }
        placementPool = new List<string>();
        placementSet = new HashSet<string>();
        var lines = asset.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var w = lines[i].Trim().ToUpperInvariant();
            if (w.Length >= 3)
            {
                placementPool.Add(w);
                placementSet.Add(w);
            }
        }
        Debug.Log("[Dictionary] Loaded " + placementPool.Count + " placement words (>= 3 letters).");
    }

    private static void LoadValidation()
    {
        validationLoaded = true;
        var asset = Resources.Load<TextAsset>(VALIDATION_FILE);
        if (asset == null)
        {
            Debug.LogWarning("[Dictionary] words.txt not found in Resources/. Validation falls back to placement pool (" + (placementPool != null ? placementPool.Count : 0) + " words). To enable real validation, place words.txt with 466k+ words in Assets/WordGame/Resources/");
            validationSet = null;
            return;
        }

        float t0 = Time.realtimeSinceStartup;
        validationSet = new HashSet<string>();
        var lines = asset.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var w = lines[i].Trim().ToUpperInvariant();
            if (w.Length >= 3) validationSet.Add(w);
        }
        float elapsed = Time.realtimeSinceStartup - t0;
        Debug.Log("[Dictionary] Loaded " + validationSet.Count + " validation words (>= 3 letters) in " + (elapsed * 1000f).ToString("F0") + "ms.");
    }
}
