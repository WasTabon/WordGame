using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource musicSource;
    public AudioSource sfxSource;

    private const string MUSIC_VOL_KEY = "WG_MusicVolume";
    private const string SFX_VOL_KEY = "WG_SfxVolume";

    public float MusicVolume { get; private set; } = 0.7f;
    public float SfxVolume { get; private set; } = 1f;

    private AudioClip clickClip;
    private AudioClip selectStartClip;
    private AudioClip[] selectAddClips;
    private AudioClip successClip;
    private AudioClip errorClip;
    private AudioClip vacantClip;
    private AudioClip tickClip;
    private AudioClip winClip;
    private AudioClip loseClip;
    private AudioClip popupOpenClip;
    private AudioClip popupCloseClip;

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

        GenerateClips();
    }

    private void GenerateClips()
    {
        clickClip = ProceduralSounds.Click();
        selectStartClip = ProceduralSounds.SelectStart();
        selectAddClips = new AudioClip[12];
        for (int i = 0; i < 12; i++) selectAddClips[i] = ProceduralSounds.SelectAdd(i);
        successClip = ProceduralSounds.Success();
        errorClip = ProceduralSounds.Error();
        vacantClip = ProceduralSounds.VacantPop();
        tickClip = ProceduralSounds.Tick();
        winClip = ProceduralSounds.Win();
        loseClip = ProceduralSounds.Lose();
        popupOpenClip = ProceduralSounds.PopupOpen();
        popupCloseClip = ProceduralSounds.PopupClose();
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

    public void PlayButtonClick() { Play(clickClip); }
    public void PlaySelectStart() { Play(selectStartClip); }
    public void PlaySelectAdd(int chainIndex)
    {
        if (selectAddClips == null) return;
        int idx = Mathf.Clamp(chainIndex, 0, selectAddClips.Length - 1);
        Play(selectAddClips[idx]);
    }
    public void PlaySuccess() { Play(successClip); }
    public void PlayError() { Play(errorClip); }
    public void PlayVacantPop() { Play(vacantClip, 0.6f); }
    public void PlayTick() { Play(tickClip, 0.5f); }
    public void PlayWin() { Play(winClip); }
    public void PlayLose() { Play(loseClip); }
    public void PlayPopupOpen() { Play(popupOpenClip); }
    public void PlayPopupClose() { Play(popupCloseClip); }

    private void Play(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }
}
