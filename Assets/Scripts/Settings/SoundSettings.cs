using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer musicMixer;
    public AudioMixer SFXMixer;

    public void SetVolume(Settings settings)
    {
        musicMixer.SetFloat("MasterVolume", Mathf.Log10(settings.musicVolume) * 20);
        SFXMixer.SetFloat("MasterVolume", Mathf.Log10(settings.musicVolume) * 20);
    }
}
