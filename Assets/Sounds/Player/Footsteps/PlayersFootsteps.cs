using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Footsteps", menuName = "Sounds/New Player Footsteps")]
public class PlayersFootsteps : ScriptableObject
{
    public List<AudioClip> labFootsteps;
    public List<AudioClip> forestFootsteps;
    public List<AudioClip> caveFootsteps;
    public List<AudioClip> glacierFootsteps;
    public List<AudioClip> sinisterLabFootsteps;
}
