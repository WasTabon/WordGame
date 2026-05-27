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
    public GameController gameController;

    public Button panToggleButton;
    public Image panToggleBackground;
    public TextMeshProUGUI panToggleLabel;
    public PanController panController;

    public Button hintButton;
    public TextMeshProUGUI hintCountLabel;
    public HintHighlighter hintHighlighter;
    public OutOfHintsPopup outOfHintsPopup;

    public Color panOffColor = new Color(0.29f, 0.33f, 0.41f, 1f);
    public Color panOnColor = new Color(0.91f, 0.65f, 0.27f, 1f);

    private int displayedStage = -1;

    private void Start()
    {
        Debug.Assert(backButton != null, "GameHUD: backButton missing!");
        Debug.Assert(modeLabel != null, "GameHUD: modeLabel missing!");

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBack);

        RefreshModeLabel();

        if (scoreLabel != null) scoreLabel.text = "0";

        if (panToggleButton != null)
        {
            panToggleButton.onClick.RemoveAllListeners();
            panToggleButton.onClick.AddListener(OnPanToggle);
        }
        RefreshPanVisual();

        if (hintButton != null)
        {
            hintButton.onClick.RemoveAllListeners();
            hintButton.onClick.AddListener(OnHintClicked);
        }
        RefreshHintCount();
    }

    private void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= HandleScoreChanged;
            scoreManager.OnScoreChanged += HandleScoreChanged;
        }
        HintManager.OnHintsChanged -= OnHintsChangedExternal;
        HintManager.OnHintsChanged += OnHintsChangedExternal;
    }

    private void OnDisable()
    {
        if (scoreManager != null) scoreManager.OnScoreChanged -= HandleScoreChanged;
        HintManager.OnHintsChanged -= OnHintsChangedExternal;
    }

    private void Update()
    {
        if (GameMode.Current == GameMode.Mode.Explore && gameController != null)
        {
            int stage = gameController.CurrentExploreStage;
            if (stage != displayedStage)
            {
                displayedStage = stage;
                RefreshModeLabel();
            }
        }
    }

    private void RefreshModeLabel()
    {
        if (modeLabel == null) return;
        if (GameMode.Current == GameMode.Mode.Escape)
        {
            modeLabel.text = "ESCAPE • LV " + LevelManager.CurrentLevel;
        }
        else
        {
            int stage = gameController != null ? gameController.CurrentExploreStage : 1;
            modeLabel.text = stage <= 1 ? "EXPLORE" : "EXPLORE • STAGE " + stage;
            displayedStage = stage;
        }
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

    private void OnHintClicked()
    {
        if (WordBuilder.Instance != null && WordBuilder.Instance.IsActivelyBuilding) return;

        if (HintManager.Hints <= 0)
        {
            if (outOfHintsPopup != null) outOfHintsPopup.Show();
            else Debug.LogWarning("GameHUD: outOfHintsPopup not assigned!");
            return;
        }

        if (gameController == null || gameController.grid == null)
        {
            Debug.LogWarning("GameHUD: gameController/grid missing for hint!");
            return;
        }

        var path = HintFinder.FindValidWord(gameController.grid, gameController.validator);
        if (path == null)
        {
            Debug.Log("[GameHUD] Hint: no valid word found on board.");
            return;
        }

        if (!HintManager.TrySpend()) return;

        if (hintHighlighter != null)
        {
            hintHighlighter.Highlight(path);
        }
        else
        {
            Debug.LogWarning("GameHUD: hintHighlighter not assigned!");
        }
    }

    private void OnHintsChangedExternal()
    {
        RefreshHintCount();
        if (hintCountLabel != null)
        {
            hintCountLabel.transform.DOKill();
            hintCountLabel.transform.localScale = Vector3.one;
            hintCountLabel.transform.DOPunchScale(Vector3.one * 0.3f, 0.35f, 6, 0.6f);
        }
    }

    private void RefreshHintCount()
    {
        if (hintCountLabel != null) hintCountLabel.text = HintManager.Hints.ToString();
    }

    private void OnBack()
    {
        if (gameController != null) gameController.SaveExploreProgressOnExit();

        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene("MainMenu");
        else SceneManager.LoadScene("MainMenu");
    }
}
