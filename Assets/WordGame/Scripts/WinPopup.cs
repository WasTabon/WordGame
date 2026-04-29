using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinPopup : PopupBase
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeBonusText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI newRecordBadge;
    public Button restartButton;
    public Button mainMenuButton;

    public int timeBonusPerSecond = 5;

    private void Start()
    {
        Debug.Assert(restartButton != null, "WinPopup: restartButton missing!");
        Debug.Assert(mainMenuButton != null, "WinPopup: mainMenuButton missing!");

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(OnRestart);

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    public void ShowResult(int wordScore, float secondsLeft)
    {
        int bonus = Mathf.RoundToInt(secondsLeft) * timeBonusPerSecond;
        int total = wordScore + bonus;

        if (titleText != null) titleText.text = "ESCAPED!";
        if (scoreText != null) scoreText.text = total.ToString();
        if (timeBonusText != null) timeBonusText.text = "+ " + bonus + " time bonus";

        bool isNewRecord = HighScoreManager.TrySetHighScore(GameMode.Current, total);
        int hs = HighScoreManager.GetHighScore(GameMode.Current);

        if (highScoreText != null) highScoreText.text = "Best: " + hs;
        if (newRecordBadge != null) newRecordBadge.gameObject.SetActive(isNewRecord);

        Show();
    }

    private void OnRestart()
    {
        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene("Game");
        else SceneManager.LoadScene("Game");
    }

    private void OnMainMenu()
    {
        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene("MainMenu");
        else SceneManager.LoadScene("MainMenu");
    }
}
