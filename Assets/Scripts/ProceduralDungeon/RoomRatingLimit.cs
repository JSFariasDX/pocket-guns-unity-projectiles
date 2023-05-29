using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonConfig_", menuName = "PCG/Room Enemies Rating Limit")]
public class RoomRatingLimit : ScriptableObject
{
    [SerializeField]
    public int small = 0;
    [SerializeField]
    public int medium = 0;
    [SerializeField]
    public int big = 0;
}
