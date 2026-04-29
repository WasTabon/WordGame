using UnityEngine;

public static class GameStats
{
    private const string GAMES_TOTAL = "WG_Stats_GamesTotal";
    private const string GAMES_EXPLORE = "WG_Stats_GamesExplore";
    private const string GAMES_ESCAPE = "WG_Stats_GamesEscape";
    private const string ESCAPE_WINS = "WG_Stats_EscapeWins";
    private const string ESCAPE_LOSSES = "WG_Stats_EscapeLosses";
    private const string WORDS_TOTAL = "WG_Stats_WordsTotal";
    private const string LONGEST_WORD = "WG_Stats_LongestWord";
    private const string BEST_WORD_SCORE = "WG_Stats_BestWordScore";
    private const string TIME_PLAYED = "WG_Stats_TimePlayed";

    public static int GamesTotal { get { return PlayerPrefs.GetInt(GAMES_TOTAL, 0); } }
    public static int GamesExplore { get { return PlayerPrefs.GetInt(GAMES_EXPLORE, 0); } }
    public static int GamesEscape { get { return PlayerPrefs.GetInt(GAMES_ESCAPE, 0); } }
    public static int EscapeWins { get { return PlayerPrefs.GetInt(ESCAPE_WINS, 0); } }
    public static int EscapeLosses { get { return PlayerPrefs.GetInt(ESCAPE_LOSSES, 0); } }
    public static int WordsTotal { get { return PlayerPrefs.GetInt(WORDS_TOTAL, 0); } }
    public static string LongestWord { get { return PlayerPrefs.GetString(LONGEST_WORD, ""); } }
    public static int BestWordScore { get { return PlayerPrefs.GetInt(BEST_WORD_SCORE, 0); } }
    public static float TimePlayed { get { return PlayerPrefs.GetFloat(TIME_PLAYED, 0f); } }

    public static void RecordGameStarted(GameMode.Mode mode)
    {
        PlayerPrefs.SetInt(GAMES_TOTAL, GamesTotal + 1);
        if (mode == GameMode.Mode.Explore) PlayerPrefs.SetInt(GAMES_EXPLORE, GamesExplore + 1);
        else PlayerPrefs.SetInt(GAMES_ESCAPE, GamesEscape + 1);
        PlayerPrefs.Save();
    }

    public static void RecordEscapeWin()
    {
        PlayerPrefs.SetInt(ESCAPE_WINS, EscapeWins + 1);
        PlayerPrefs.Save();
    }

    public static void RecordEscapeLoss()
    {
        PlayerPrefs.SetInt(ESCAPE_LOSSES, EscapeLosses + 1);
        PlayerPrefs.Save();
    }

    public static void RecordWord(string word, int score)
    {
        if (string.IsNullOrEmpty(word)) return;
        PlayerPrefs.SetInt(WORDS_TOTAL, WordsTotal + 1);

        if (word.Length > LongestWord.Length)
            PlayerPrefs.SetString(LONGEST_WORD, word.ToUpperInvariant());

        if (score > BestWordScore)
            PlayerPrefs.SetInt(BEST_WORD_SCORE, score);

        PlayerPrefs.Save();
    }

    public static void AddTimePlayed(float seconds)
    {
        if (seconds <= 0f) return;
        PlayerPrefs.SetFloat(TIME_PLAYED, TimePlayed + seconds);
        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(GAMES_TOTAL);
        PlayerPrefs.DeleteKey(GAMES_EXPLORE);
        PlayerPrefs.DeleteKey(GAMES_ESCAPE);
        PlayerPrefs.DeleteKey(ESCAPE_WINS);
        PlayerPrefs.DeleteKey(ESCAPE_LOSSES);
        PlayerPrefs.DeleteKey(WORDS_TOTAL);
        PlayerPrefs.DeleteKey(LONGEST_WORD);
        PlayerPrefs.DeleteKey(BEST_WORD_SCORE);
        PlayerPrefs.DeleteKey(TIME_PLAYED);
        PlayerPrefs.Save();
    }

    public static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        int h = total / 3600;
        int m = (total % 3600) / 60;
        int s = total % 60;
        if (h > 0) return string.Format("{0}h {1}m", h, m);
        if (m > 0) return string.Format("{0}m {1}s", m, s);
        return s + "s";
    }
}
