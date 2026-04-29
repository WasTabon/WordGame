using UnityEngine;

public static class HighScoreManager
{
    private const string EXPLORE_KEY = "WG_HighScore_Explore";
    private const string ESCAPE_KEY = "WG_HighScore_Escape";

    public static int GetHighScore(GameMode.Mode mode)
    {
        return PlayerPrefs.GetInt(KeyFor(mode), 0);
    }

    public static bool TrySetHighScore(GameMode.Mode mode, int score)
    {
        int current = GetHighScore(mode);
        if (score > current)
        {
            PlayerPrefs.SetInt(KeyFor(mode), score);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    private static string KeyFor(GameMode.Mode mode)
    {
        return mode == GameMode.Mode.Escape ? ESCAPE_KEY : EXPLORE_KEY;
    }
}
