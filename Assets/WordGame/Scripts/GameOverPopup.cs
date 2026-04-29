using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverPopup : PopupBase
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI newRecordBadge;
    public Button restartButton;
    public Button mainMenuButton;

    private void Start()
    {
        Debug.Assert(restartButton != null, "GameOverPopup: restartButton missing!");
        Debug.Assert(mainMenuButton != null, "GameOverPopup: mainMenuButton missing!");

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(OnRestart);

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    public void ShowResult(int finalScore, string title)
    {
        if (titleText != null) titleText.text = title;
        if (scoreText != null) scoreText.text = finalScore.ToString();

        bool isNewRecord = HighScoreManager.TrySetHighScore(GameMode.Current, finalScore);
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
