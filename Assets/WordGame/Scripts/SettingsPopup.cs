using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPopup : PopupBase
{
    public Slider musicSlider;
    public TextMeshProUGUI musicValueText;
    public Slider sfxSlider;
    public TextMeshProUGUI sfxValueText;
    public Button closeButton;
    public Button blockerButton;
    public Button resetTutorialButton;
    public TextMeshProUGUI resetTutorialFeedback;

    private void Start()
    {
        Debug.Assert(musicSlider != null, "SettingsPopup: musicSlider missing!");
        Debug.Assert(sfxSlider != null, "SettingsPopup: sfxSlider missing!");

        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);

        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Hide);

        if (blockerButton != null)
        {
            blockerButton.onClick.RemoveAllListeners();
            blockerButton.onClick.AddListener(Hide);
        }

        if (resetTutorialButton != null)
        {
            resetTutorialButton.onClick.RemoveAllListeners();
            resetTutorialButton.onClick.AddListener(OnResetTutorial);
        }
        if (resetTutorialFeedback != null) resetTutorialFeedback.gameObject.SetActive(false);
    }

    private void OnResetTutorial()
    {
        Tutorial.ResetAll();
        if (resetTutorialFeedback != null)
        {
            resetTutorialFeedback.gameObject.SetActive(true);
            resetTutorialFeedback.text = "Tutorial reset!";
            CancelInvoke(nameof(HideResetFeedback));
            Invoke(nameof(HideResetFeedback), 1.5f);
        }
    }

    private void HideResetFeedback()
    {
        if (resetTutorialFeedback != null) resetTutorialFeedback.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (SoundManager.Instance == null) return;
        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(SoundManager.Instance.MusicVolume);
            UpdateMusicText(musicSlider.value);
        }
        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(SoundManager.Instance.SfxVolume);
            UpdateSfxText(sfxSlider.value);
        }
    }

    private void OnMusicChanged(float v)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetMusicVolume(v);
        UpdateMusicText(v);
    }

    private void OnSfxChanged(float v)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetSfxVolume(v);
        UpdateSfxText(v);
    }

    private void UpdateMusicText(float v)
    {
        if (musicValueText != null) musicValueText.text = Mathf.RoundToInt(v * 100f) + "%";
    }

    private void UpdateSfxText(float v)
    {
        if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(v * 100f) + "%";
    }
}
