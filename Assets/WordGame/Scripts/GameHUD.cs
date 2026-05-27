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

    public Button panToggleButton;
    public Image panToggleBackground;
    public TextMeshProUGUI panToggleLabel;
    public PanController panController;

    public Color panOffColor = new Color(0.29f, 0.33f, 0.41f, 1f);
    public Color panOnColor = new Color(0.91f, 0.65f, 0.27f, 1f);

    private void Start()
    {
        Debug.Assert(backButton != null, "GameHUD: backButton missing!");
        Debug.Assert(modeLabel != null, "GameHUD: modeLabel missing!");

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBack);

        if (GameMode.Current == GameMode.Mode.Escape)
            modeLabel.text = "ESCAPE • LV " + LevelManager.CurrentLevel;
        else
            modeLabel.text = "EXPLORE";

        if (scoreLabel != null) scoreLabel.text = "0";

        if (panToggleButton != null)
        {
            panToggleButton.onClick.RemoveAllListeners();
            panToggleButton.onClick.AddListener(OnPanToggle);
        }
        RefreshPanVisual();
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

    private void OnPanToggle()
    {
        if (panController == null) return;
        panController.TogglePanMode();
        RefreshPanVisual();
    }

    private void RefreshPanVisual()
    {
        bool active = panController != null && panController.PanModeActive;
        if (panToggleBackground != null)
            panToggleBackground.color = active ? panOnColor : panOffColor;
        if (panToggleLabel != null)
            panToggleLabel.color = active ? new Color(0.10f, 0.14f, 0.20f, 1f) : Color.white;
    }

    private void OnBack()
    {
        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene("MainMenu");
        else SceneManager.LoadScene("MainMenu");
    }
}
