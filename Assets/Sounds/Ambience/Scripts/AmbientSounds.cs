using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ambient Sounds", menuName = "Sounds/New Ambient Sounds")]
public class AmbientSounds : ScriptableObject
{
    public AudioClip backgroundSound;

    [Header("Sounds")]
    public List<AudioClip> ambientSounds;

    [Header("Settings")]
    public float minInterval = 30;
    public float maxInterval = 60;
    public float volumeMultiplier = 1;
}
