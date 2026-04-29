using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    public RectTransform titleText;
    public Button playButton;
    public Button settingsButton;
    public Button howToPlayButton;
    public Button statsButton;

    public SettingsPopup settingsPopup;
    public HowToPlayPopup howToPlayPopup;
    public ModeSelectPopup modeSelectPopup;
    public StatsPopup statsPopup;

    private void Start()
    {
        Debug.Assert(titleText != null, "MainMenuUI: titleText missing!");
        Debug.Assert(playButton != null, "MainMenuUI: playButton missing!");
        Debug.Assert(settingsPopup != null, "MainMenuUI: settingsPopup missing!");
        Debug.Assert(howToPlayPopup != null, "MainMenuUI: howToPlayPopup missing!");
        Debug.Assert(modeSelectPopup != null, "MainMenuUI: modeSelectPopup missing!");

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(OnPlayClicked);

        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(OnSettingsClicked);

        howToPlayButton.onClick.RemoveAllListeners();
        howToPlayButton.onClick.AddListener(OnHowToPlayClicked);

        if (statsButton != null)
        {
            statsButton.onClick.RemoveAllListeners();
            statsButton.onClick.AddListener(OnStatsClicked);
        }

        AnimateIn();
    }

    private void AnimateIn()
    {
        titleText.localScale = Vector3.zero;
        playButton.transform.localScale = Vector3.zero;
        settingsButton.transform.localScale = Vector3.zero;
        howToPlayButton.transform.localScale = Vector3.zero;
        if (statsButton != null) statsButton.transform.localScale = Vector3.zero;

        titleText.DOScale(1f, 0.55f).SetDelay(0.1f).SetEase(Ease.OutBack);
        playButton.transform.DOScale(1f, 0.5f).SetDelay(0.25f).SetEase(Ease.OutBack);
        settingsButton.transform.DOScale(1f, 0.45f).SetDelay(0.4f).SetEase(Ease.OutBack);
        howToPlayButton.transform.DOScale(1f, 0.45f).SetDelay(0.45f).SetEase(Ease.OutBack);
        if (statsButton != null) statsButton.transform.DOScale(1f, 0.45f).SetDelay(0.5f).SetEase(Ease.OutBack);

        titleText.DOAnchorPosY(titleText.anchoredPosition.y + 30f, 1.8f)
            .SetDelay(0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnPlayClicked()
    {
        modeSelectPopup.Show();
    }

    private void OnSettingsClicked()
    {
        settingsPopup.Show();
    }

    private void OnHowToPlayClicked()
    {
        howToPlayPopup.Show();
    }

    private void OnStatsClicked()
    {
        if (statsPopup != null) statsPopup.Show();
    }
}
