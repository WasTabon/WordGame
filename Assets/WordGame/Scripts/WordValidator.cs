using System.Collections.Generic;
using UnityEngine;

public enum ValidationResult
{
    Valid,
    TooShort,
    NotInDictionary,
    AlreadyUsed
}

public class WordValidator : MonoBehaviour
{
    public int minWordLength = 2;

    private readonly HashSet<string> usedWords = new HashSet<string>();

    public ValidationResult Validate(string word, int requiredMinLength)
    {
        if (string.IsNullOrEmpty(word)) return ValidationResult.TooShort;
        var upper = word.ToUpperInvariant();
        int needed = Mathf.Max(minWordLength, requiredMinLength);
        if (upper.Length < needed) return ValidationResult.TooShort;
        if (!Dictionary.Contains(upper)) return ValidationResult.NotInDictionary;
        if (usedWords.Contains(upper)) return ValidationResult.AlreadyUsed;
        return ValidationResult.Valid;
    }

    public void MarkUsed(string word)
    {
        if (string.IsNullOrEmpty(word)) return;
        usedWords.Add(word.ToUpperInvariant());
    }

    public void ResetUsedWords()
    {
        usedWords.Clear();
    }

    public int UsedCount { get { return usedWords.Count; } }
}
