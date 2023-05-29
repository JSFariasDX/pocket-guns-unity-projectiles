using UnityEngine;

[CreateAssetMenu(fileName = "OST_", menuName = "OST/Create New")]
public class OST : ScriptableObject
{
    [Header("Settings")]
    public bool BossOST;

    [Header("Theme Lines")]
    public AudioClip baseTrack;
    public AudioClip singleDrumTrack;
    public AudioClip doubleDrumTrack;
    public AudioClip dangerTrack;
    public AudioClip endSFX;
}
