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
        return 1 + l;
    }

    public static float GetCellSizeForLevel(int level)
    {
        return level <= 1 ? 75f : 65f;
    }
}
