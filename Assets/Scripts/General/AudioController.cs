using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public AudioMixer audioMixer;
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource bgsSource;

    private Coroutine fadeOutCoroutine;

    private float ConvertToDecibels(float volumePercent)
    {
        if (volumePercent <= 0) return -80f;
        float volume = volumePercent / 100f;
        return 20f * Mathf.Log10(volume);
    }

    public void Start()
    {
        var configData = Config.Instance.configData;
        audioMixer.SetFloat("SFX", ConvertToDecibels(configData["settings"]["sfxVolume"].AsFloat));
        audioMixer.SetFloat("BGS", ConvertToDecibels(configData["settings"]["bgsVolume"].AsFloat));
        audioMixer.SetFloat("BGM", ConvertToDecibels(configData["settings"]["bgmVolume"].AsFloat));
    }

    public static void PlayBGM(AudioTypes type, float fadeInDuration = 0f)
    {
        var clip = StorageController.Instance.GetAudio(type)?.clip;
        if (clip == null) return;

        if (Instance.bgmSource.isPlaying)
            Instance.bgmSource.Stop();

        Instance.bgmSource.clip = clip;
        if (fadeInDuration > 0f)
        {
            Instance.bgmSource.volume = 0f;
            Instance.bgmSource.Play();
            Instance.bgmSource.DOFade(1f, fadeInDuration)
                .SetEase(Ease.InQuad);
        }
        else
        {
            Instance.bgmSource.volume = 1f;
            Instance.bgmSource.Play();
        }
    }

    public static void StopBGM(float fadeOutDuration = 1f)
    {
        if (Instance == null || !Instance.bgmSource.isPlaying) return;

        Instance.bgmSource.DOKill();
        Instance.bgmSource.DOFade(0f, fadeOutDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Instance.bgmSource.Stop();
                Instance.bgmSource.volume = 1f;
            });
    }

    public static void PlaySFX(AudioTypes type)
    {
        var clip = StorageController.Instance.GetAudio(type)?.clip;
        if (clip == null) return;

        Instance.sfxSource.PlayOneShot(clip);
    }

    public static void PlayBGS(AudioTypes type, float fadeInDuration = 0f)
    {
        var clip = StorageController.Instance.GetAudio(type)?.clip;
        if (clip == null) return;

        if (Instance.bgsSource.isPlaying)
            Instance.bgsSource.Stop();

        Instance.bgsSource.clip = clip;
        if (fadeInDuration > 0f)
        {
            Instance.bgsSource.volume = 0f;
            Instance.bgsSource.Play();
            Instance.bgsSource.DOFade(1f, fadeInDuration)
                .SetEase(Ease.InQuad);
        }
        else
        {
            Instance.bgsSource.volume = 1f;
            Instance.bgsSource.Play();
        }
    }

    public static void StopBGS(float fadeOutDuration = 1f)
    {
        if (Instance == null || !Instance.bgsSource.isPlaying) return;

        Instance.bgsSource.DOKill();
        Instance.bgsSource.DOFade(0f, fadeOutDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Instance.bgsSource.Stop();
                Instance.bgsSource.volume = 1f;
            });
    }

    public static void SetVolumeBGS(float newVolume = 0f, float fadeInDuration = 0f)
    {
        Instance.bgsSource.DOKill();
        Instance.bgsSource.DOFade(newVolume, fadeInDuration).SetEase(Ease.InQuad);
    }


}

public enum AudioTypes
{
    None = -1,
    BGM_intro = 100,
    BGS_crowd = 200,
    SFX_uiClick = 300,
    SFX_countdown = 301,
    SFX_countdownEnd = 302,
    SFX_roulette = 303,
    SFX_win = 304,
    SFX_lose = 305,
    SFX_inout = 306,
    SFX_score = 307,

    SFX_ballPlaced = 399,
    SFX_ballHit = 400,
    SFX_ballShoot = 401,
    SFX_goal = 402,
}
