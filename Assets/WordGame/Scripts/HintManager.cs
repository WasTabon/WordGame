using System;
using UnityEngine;

public static class HintManager
{
    private const string KEY = "WG_Hints";
    private const string FIRST_RUN_KEY = "WG_Hints_Initialized";
    private const int STARTER_HINTS = 5;
    public const int MIN_WORD_LENGTH_FOR_HINT = 6;
    public const int IAP_PACK_AMOUNT = 10;

    public static event Action OnHintsChanged;

    public static int Hints
    {
        get
        {
            EnsureInitialized();
            return PlayerPrefs.GetInt(KEY, 0);
        }
    }

    private static void EnsureInitialized()
    {
        if (PlayerPrefs.GetInt(FIRST_RUN_KEY, 0) == 0)
        {
            PlayerPrefs.SetInt(KEY, STARTER_HINTS);
            PlayerPrefs.SetInt(FIRST_RUN_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log("[HintManager] First run: granted " + STARTER_HINTS + " starter hints.");
        }
    }

    public static bool TrySpend()
    {
        EnsureInitialized();
        int cur = Hints;
        if (cur <= 0) return false;
        PlayerPrefs.SetInt(KEY, cur - 1);
        PlayerPrefs.Save();
        FireChange();
        Debug.Log("[HintManager] Spent 1 hint. Remaining: " + Hints);
        return true;
    }

    public static void Earn()
    {
        EnsureInitialized();
        PlayerPrefs.SetInt(KEY, Hints + 1);
        PlayerPrefs.Save();
        FireChange();
        Debug.Log("[HintManager] Earned 1 hint. Total: " + Hints);
    }

    public static bool CheckAndRewardForWord(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;
        if (word.Length < MIN_WORD_LENGTH_FOR_HINT) return false;
        Earn();
        return true;
    }

    public static void GrantTenHints()
    {
        EnsureInitialized();
        PlayerPrefs.SetInt(KEY, Hints + IAP_PACK_AMOUNT);
        PlayerPrefs.Save();
        FireChange();
        Debug.Log("[HintManager] IAP success: granted " + IAP_PACK_AMOUNT + " hints. Total: " + Hints);
    }

    public static void OnPurchaseFailed()
    {
        Debug.LogWarning("[HintManager] IAP: purchase failed or cancelled.");
    }

    public static void OnProductFetched()
    {
        Debug.Log("[HintManager] IAP: product info fetched.");
    }

    public static void ResetForDebug()
    {
        PlayerPrefs.DeleteKey(KEY);
        PlayerPrefs.DeleteKey(FIRST_RUN_KEY);
        PlayerPrefs.Save();
        FireChange();
        Debug.Log("[HintManager] Reset for debug.");
    }

    private static void FireChange()
    {
        var ev = OnHintsChanged;
        if (ev != null) ev();
    }
}
