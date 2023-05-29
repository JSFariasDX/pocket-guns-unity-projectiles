using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerParams
{
    public const string MASTER = "MasterVolume";
    public const string MAIN = "MainVolume";
    public const string CORRIDOR = "CorridorVolume";
    public const string BOSS = "BossVolume";
}
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    [SerializeField] AudioSource mainSource;
    [SerializeField] AudioSource corridorSource;
    [SerializeField] AudioSource bossSource;
    [SerializeField] AudioSource SFXSource;
    AudioSource source;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        source = GetComponent<AudioSource>();
    }

    public void SetTheme(DungeonConfig config)
    {
        if (config.mainClip) mainSource.clip = config.mainClip;
        if (config.corridorClip) corridorSource.clip = config.corridorClip;
        if (config.bossClip) bossSource.clip = config.bossClip;
    }

    public void SetMixerChVolume(string param, float value, float fadeDuration = 0)
    {
        audioMixer.ClearFloat(param);
        if (fadeDuration > 0)
        {
            StartCoroutine(FadeMixerGroup.StartFade(audioMixer, param, 2f, value));
        }
        else
        {
            audioMixer.SetFloat(param, value);
        }
    }

    #region Self Configuration
    void PlayMainMusic(bool start)
    {
        if (start) mainSource.Play();
        else mainSource.Stop();
    }

    void PlayCorridorMusic(bool start)
    {
        if (start) corridorSource.Play();
        else corridorSource.Stop();
    }

    void PlayBossMusic(bool start)
    {
        if (start) bossSource.Play();
        else bossSource.Stop();
    }
    #endregion

    public void StartMainTheme()
    {
        PlayMainMusic(true);
        PlayCorridorMusic(false);
        PlayBossMusic(false);
    }

    public void StartCorridorTheme()
    {
        PlayMainMusic(false);
        PlayCorridorMusic(true);
        PlayBossMusic(false);
    }

    public void StartBossTheme()
    {
        PlayMainMusic(false);
        PlayCorridorMusic(false);
        PlayBossMusic(true);
    }

    public void StopMusic()
    {
        PlayMainMusic(false);
        PlayCorridorMusic(false);
        PlayBossMusic(false);
    }

    public void PlayMenuTheme(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public void StopMenuTheme()
    {
        source.Play();
        source.clip = null;
    }

    public bool HasClip()
    {
        return source.clip != null;
    }

    public void StopAll()
    {
        StopMenuTheme();
        StopMusic();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
