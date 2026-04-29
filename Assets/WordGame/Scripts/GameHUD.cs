using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameHUD : MonoBehaviour
{
    public Button backButton;
    public TextMeshProUGUI modeLabel;
    public TextMeshProUGUI scoreLabel;
    public ScoreManager scoreManager;

    private void Start()
    {
        Debug.Assert(backButton != null, "GameHUD: backButton missing!");
        Debug.Assert(modeLabel != null, "GameHUD: modeLabel missing!");

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBack);

        modeLabel.text = GameMode.Current == GameMode.Mode.Escape ? "ESCAPE" : "EXPLORE";
        if (scoreLabel != null) scoreLabel.text = "0";
    }

    private void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= HandleScoreChanged;
            scoreManager.OnScoreChanged += HandleScoreChanged;
        }
    }

    private void OnDisable()
    {
        if (scoreManager != null) scoreManager.OnScoreChanged -= HandleScoreChanged;
    }

    private void HandleScoreChanged(int newScore, int delta)
    {
        if (scoreLabel == null) return;
        scoreLabel.text = newScore.ToString();
        if (delta > 0)
        {
            scoreLabel.transform.DOKill();
            scoreLabel.transform.localScale = Vector3.one;
            scoreLabel.transform.DOPunchScale(Vector3.one * 0.25f, 0.35f, 6, 0.6f);
        }
    }

    private void OnBack()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
