using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ModeSelectPopup : PopupBase
{
    public Button escapeButton;
    public Button exploreButton;
    public Button closeButton;
    public Button blockerButton;

    public TextMeshProUGUI escapeBestText;
    public TextMeshProUGUI exploreBestText;

    public PlaceholderPopup placeholderPopup;

    private void Start()
    {
        Debug.Assert(escapeButton != null, "ModeSelectPopup: escapeButton missing!");
        Debug.Assert(exploreButton != null, "ModeSelectPopup: exploreButton missing!");

        escapeButton.onClick.RemoveAllListeners();
        escapeButton.onClick.AddListener(OnEscapeSelected);

        exploreButton.onClick.RemoveAllListeners();
        exploreButton.onClick.AddListener(OnExploreSelected);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Hide);

        if (blockerButton != null)
        {
            blockerButton.onClick.RemoveAllListeners();
            blockerButton.onClick.AddListener(Hide);
        }
    }

    private void OnEnable()
    {
        if (escapeBestText != null)
        {
            int best = HighScoreManager.GetHighScore(GameMode.Mode.Escape);
            int level = LevelManager.CurrentLevel;
            escapeBestText.text = "Lv " + level + " • Best: " + best;
        }
        if (exploreBestText != null)
            exploreBestText.text = "Best: " + HighScoreManager.GetHighScore(GameMode.Mode.Explore);
    }

    private void OnEscapeSelected()
    {
        GameMode.SetMode(GameMode.Mode.Escape);
        LoadGame();
    }

    private void OnExploreSelected()
    {
        GameMode.SetMode(GameMode.Mode.Explore);
        LoadGame();
    }

    private void LoadGame()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("Game");
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
    }
}
