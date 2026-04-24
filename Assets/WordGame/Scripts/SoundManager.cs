using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip buttonClickClip;
    public AudioClip popupOpenClip;
    public AudioClip popupCloseClip;

    private const string MUSIC_VOL_KEY = "WG_MusicVolume";
    private const string SFX_VOL_KEY = "WG_SfxVolume";

    public float MusicVolume { get; private set; } = 0.7f;
    public float SfxVolume { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Assert(musicSource != null, "SoundManager: musicSource not assigned!");
        Debug.Assert(sfxSource != null, "SoundManager: sfxSource not assigned!");

        MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 0.7f);
        SfxVolume = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);

        musicSource.volume = MusicVolume;
        sfxSource.volume = SfxVolume;
    }

    public void SetMusicVolume(float v)
    {
        MusicVolume = Mathf.Clamp01(v);
        musicSource.volume = MusicVolume;
        PlayerPrefs.SetFloat(MUSIC_VOL_KEY, MusicVolume);
    }

    public void SetSfxVolume(float v)
    {
        SfxVolume = Mathf.Clamp01(v);
        sfxSource.volume = SfxVolume;
        PlayerPrefs.SetFloat(SFX_VOL_KEY, SfxVolume);
    }

    public void PlayButtonClick()
    {
        if (buttonClickClip != null) sfxSource.PlayOneShot(buttonClickClip);
    }

    public void PlayPopupOpen()
    {
        if (popupOpenClip != null) sfxSource.PlayOneShot(popupOpenClip);
    }

    public void PlayPopupClose()
    {
        if (popupCloseClip != null) sfxSource.PlayOneShot(popupCloseClip);
    }
}
