using UnityEngine;

public static class LevelManager
{
    private const string KEY_CURRENT = "WG_EscapeLevel";

    public static int CurrentLevel
    {
        get { return Mathf.Max(1, PlayerPrefs.GetInt(KEY_CURRENT, 1)); }
    }

    public static void SetLevel(int level)
    {
        PlayerPrefs.SetInt(KEY_CURRENT, Mathf.Max(1, level));
        PlayerPrefs.Save();
    }

    public static void IncrementLevel()
    {
        SetLevel(CurrentLevel + 1);
    }

    public static void ResetToLevel1()
    {
        SetLevel(1);
    }

    public static int GetRadiusForLevel(int level)
    {
        int l = Mathf.Max(1, level);
        return Mathf.Clamp(1 + l, 2, 8);
    }

    public static float GetCellSizeForLevel(int level)
    {
        int radius = GetRadiusForLevel(level);
        return 230f / (radius + 1);
    }
}
