using System.Collections.Generic;
using UnityEngine;

public enum ValidationResult
{
    Valid,
    TooShort,
    NotInDictionary,
    AlreadyUsed,
    Invalid
}

public class WordValidator : MonoBehaviour
{
    public int minWordLength = 2;

    private readonly HashSet<string> usedWords = new HashSet<string>();

    public ValidationResult Validate(string word, int requiredMinLength)
    {
        if (string.IsNullOrEmpty(word)) return ValidationResult.Invalid;
        string upper = word.ToUpperInvariant();

        int effectiveMin = Mathf.Max(minWordLength, requiredMinLength);
        if (upper.Length < effectiveMin) return ValidationResult.TooShort;

        if (usedWords.Contains(upper)) return ValidationResult.AlreadyUsed;

        if (!Dictionary.IsValidWord(upper)) return ValidationResult.NotInDictionary;

        return ValidationResult.Valid;
    }

    public void MarkUsed(string word)
    {
        if (string.IsNullOrEmpty(word)) return;
        usedWords.Add(word.ToUpperInvariant());
    }

    public bool IsAlreadyUsed(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;
        return usedWords.Contains(word.ToUpperInvariant());
    }

    public void ResetUsedWords()
    {
        usedWords.Clear();
    }
}
