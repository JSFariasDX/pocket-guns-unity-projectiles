using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PocketDisplayAttributes
{
    public int displayHp;
    public int dropRate;
    public int weapon;
    public int healing;
    public int movement;
    public int specialStats;

    public PocketDisplayAttributes(int level)
	{
        displayHp = 4 + level;
        dropRate = 4 + level;
        weapon = 4 + level;
        healing = 4 + level;
        movement = 4 + level;
        specialStats = 4 + level;
	}
}
