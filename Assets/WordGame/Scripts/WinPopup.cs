using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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
    public float countUpDuration = 0.9f;

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
        if (scoreText != null) scoreText.text = "0";
        if (timeBonusText != null) timeBonusText.text = "+ " + bonus + " time bonus";

        bool isNewRecord = HighScoreManager.TrySetHighScore(GameMode.Current, total);
        int hs = HighScoreManager.GetHighScore(GameMode.Current);

        if (highScoreText != null) highScoreText.text = "Best: " + hs;
        if (newRecordBadge != null) newRecordBadge.gameObject.SetActive(isNewRecord);

        Show();
        AnimateScore(total);
    }

    private void AnimateScore(int target)
    {
        if (scoreText == null) return;
        scoreText.transform.DOKill();
        int displayed = 0;
        DOTween.To(() => displayed, v => { displayed = v; scoreText.text = v.ToString(); }, target, countUpDuration)
            .SetEase(Ease.OutQuart)
            .SetDelay(0.25f)
            .OnComplete(() => scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 4, 0.5f));
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
