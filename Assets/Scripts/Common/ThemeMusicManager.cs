using UnityEngine;
using UnityEngine.Audio;

public static class MixerParams
{
    public const string MainVolume = "MainVolume";
    public const string DrumSingleVolume = "DrumSingleVolume";
    public const string DrumDoubleVolume = "DrumDoubleVolume";
    public const string DangerVolume = "DangerVolume";
}

public class ThemeMusicManager : MonoBehaviour
{
    public static ThemeMusicManager Instance;
    AudioSource audioSource;
    public AudioMixer audioMixer;

    OST overlayedOST;
    OST currentOST;

    [SerializeField]
    AudioSource baseTrackSource;
    [SerializeField]
    AudioSource singleDrumTrackSource;
    [SerializeField]
    AudioSource doubleDrumTrackSource;
    [SerializeField]
    AudioSource dangerTrackSource;
    [SerializeField]
    AudioSource SFXSource;

    private float _dangerSourceStartVolume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _dangerSourceStartVolume = dangerTrackSource.volume;
    }

    public void SetTheme(OST ost)
    {
        if (ost.BossOST)
        {
            overlayedOST = currentOST;
            currentOST = ost;

            InitializeClips(currentOST);
            return;
        }

        currentOST = ost;
        InitializeClips(currentOST);
    }

    public void RevertTheme()
    {
        if (overlayedOST == null)
            return;
            
        currentOST = overlayedOST;
        InitializeClips(currentOST);
        overlayedOST = null;

        StartTheme();
    }

    private void InitializeClips(OST ost)
    {
        SetBaseTrackClip(ost.baseTrack);
        if (ost.singleDrumTrack)
        {
            SetDrumSingleChClip(ost.singleDrumTrack);
        }
        if (ost.doubleDrumTrack)
        {
            SetDrumDoubleChClip(ost.doubleDrumTrack);
        }
        if (ost.dangerTrack)
        {
            SetDangerChClip(ost.dangerTrack);
        }
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void SetMixerChVolume(string param, float value, float fadeDuration = 0)
    {
        audioMixer.ClearFloat(param);
        if (fadeDuration > 0)
        {
            StartCoroutine(FadeMixerGroup.StartFade(audioMixer, param, 2f, value));
        } else
        {
            audioMixer.SetFloat(param, value);
        }
    }

    public void SetBaseTrackClip(AudioClip clip)
    {
        baseTrackSource.clip = clip;
    }

    public void SetDrumSingleChClip(AudioClip clip)
    {
        singleDrumTrackSource.clip = clip;
    }

    public void SetDrumDoubleChClip(AudioClip clip)
    {
        doubleDrumTrackSource.clip = clip;
    }

    public void SetDangerChClip(AudioClip clip)
    {
        dangerTrackSource.clip = clip;
    }

    public void ResetClips()
    {
        baseTrackSource.clip = null;
        singleDrumTrackSource.clip = null;
        doubleDrumTrackSource.clip = null;
        dangerTrackSource.clip = null;
    }

    public void PlayAllLines()
    {
        baseTrackSource.Play();
        singleDrumTrackSource.Play();
        doubleDrumTrackSource.Play();
        dangerTrackSource.Play();
    }

    public void StartTheme(bool shouldStartDrums = false)
    {
        float drums = shouldStartDrums ? 1f : -80;
        PlayAllLines();
        SetMixerChVolume(MixerParams.MainVolume, 1f);
        SetMixerChVolume(MixerParams.DrumSingleVolume, drums);
        SetMixerChVolume(MixerParams.DrumDoubleVolume, -80);
        SetMixerChVolume(MixerParams.DangerVolume, -80);
    }

    public void StartDrums()
    {
        SetMixerChVolume(MixerParams.DrumSingleVolume, 1f);
    }

    public void PlayAction()
    {
        SetMixerChVolume(MixerParams.DrumSingleVolume, -80, 2f);
        SetMixerChVolume(MixerParams.DrumDoubleVolume, 1f, 2f);
    }

    public void StopAction()
    {
        SetMixerChVolume(MixerParams.DrumSingleVolume, 1f, 2f);
        SetMixerChVolume(MixerParams.DrumDoubleVolume, -80, 2f);
    }

    public void PlayDanger()
    {
        SetMixerChVolume(MixerParams.DangerVolume, 1f, 2f);
    }

    public void StopDanger()
    {
        SetMixerChVolume(MixerParams.DangerVolume, -80, 2f);
    }

    public void MuteDanger()
    {
        dangerTrackSource.volume = 0f;
    }
    
    public void UnmuteDanger()
    {
        dangerTrackSource.volume = _dangerSourceStartVolume;
    }

    public bool IsDangerMuted()
    {
        return dangerTrackSource.volume == 0f;
    }

    public void EndAllTracksButMain()
    {
        SetMixerChVolume(MixerParams.DrumSingleVolume, -80, 1f);
        SetMixerChVolume(MixerParams.DrumDoubleVolume, -80, 1f);
        SetMixerChVolume(MixerParams.DangerVolume, -80, 1f);
    }

    public void StopAllSources()
    {
        ResetClips();

        baseTrackSource.Stop();
        singleDrumTrackSource.Stop();
        doubleDrumTrackSource.Stop();
        dangerTrackSource.Stop();
    }

    public void FinalizeOST()
    {
        EndAllTracksButMain();
        SetMixerChVolume(MixerParams.MainVolume, -80, 1f);

        SFXSource.PlayOneShot(currentOST.endSFX);
    }

    public void PlayFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    void MuteAllLines()
    {
        baseTrackSource.volume = 0;
        singleDrumTrackSource.volume = 0;
        doubleDrumTrackSource.volume = 0;
        dangerTrackSource.volume = 0;
    }

    public OST GetCurrentOST()
    {
        return currentOST;
    }
}
