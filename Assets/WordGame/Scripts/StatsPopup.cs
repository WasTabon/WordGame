using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsPopup : PopupBase
{
    public TextMeshProUGUI gamesPlayedText;
    public TextMeshProUGUI wordsTotalText;
    public TextMeshProUGUI longestWordText;
    public TextMeshProUGUI bestWordScoreText;
    public TextMeshProUGUI escapeWinrateText;
    public TextMeshProUGUI timePlayedText;
    public TextMeshProUGUI exploreBestText;
    public TextMeshProUGUI escapeBestText;
    public Button closeButton;
    public Button blockerButton;
    public Button resetButton;
    public TextMeshProUGUI resetFeedback;

    private void Start()
    {
        Debug.Assert(closeButton != null, "StatsPopup: closeButton missing!");

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Hide);

        if (blockerButton != null)
        {
            blockerButton.onClick.RemoveAllListeners();
            blockerButton.onClick.AddListener(Hide);
        }
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(OnReset);
        }
        if (resetFeedback != null) resetFeedback.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Refresh();
        if (resetFeedback != null) resetFeedback.gameObject.SetActive(false);
    }

    private void Refresh()
    {
        if (gamesPlayedText != null)
            gamesPlayedText.text = GameStats.GamesTotal.ToString();
        if (wordsTotalText != null)
            wordsTotalText.text = GameStats.WordsTotal.ToString();
        if (longestWordText != null)
        {
            var lw = GameStats.LongestWord;
            longestWordText.text = string.IsNullOrEmpty(lw) ? "-" : lw + " (" + lw.Length + ")";
        }
        if (bestWordScoreText != null)
            bestWordScoreText.text = GameStats.BestWordScore.ToString();
        if (escapeWinrateText != null)
        {
            int total = GameStats.GamesEscape;
            int wins = GameStats.EscapeWins;
            if (total == 0) escapeWinrateText.text = "-";
            else escapeWinrateText.text = wins + " / " + total + " (" + Mathf.RoundToInt(100f * wins / total) + "%)";
        }
        if (timePlayedText != null)
            timePlayedText.text = GameStats.FormatTime(GameStats.TimePlayed);
        if (exploreBestText != null)
            exploreBestText.text = HighScoreManager.GetHighScore(GameMode.Mode.Explore).ToString();
        if (escapeBestText != null)
            escapeBestText.text = HighScoreManager.GetHighScore(GameMode.Mode.Escape).ToString();
    }

    private void OnReset()
    {
        GameStats.ResetAll();
        Refresh();
        if (resetFeedback != null)
        {
            resetFeedback.gameObject.SetActive(true);
            CancelInvoke(nameof(HideResetFeedback));
            Invoke(nameof(HideResetFeedback), 1.5f);
        }
    }

    private void HideResetFeedback()
    {
        if (resetFeedback != null) resetFeedback.gameObject.SetActive(false);
    }
}
