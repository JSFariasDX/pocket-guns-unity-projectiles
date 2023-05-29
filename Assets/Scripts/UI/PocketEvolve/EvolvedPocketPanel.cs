using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvolvedPocketPanel : PocketPanel
{
    public override void Setup(Pocket pocket)
    {
        base.Setup(pocket);

        if (!pocket) return;

        PocketDisplayInformations pocketInfo = pocket.GetComponent<PocketDisplayInformations>();

        int newLevel = pocket.level + 1;
        pocketLevel.text = "Level " + newLevel;

        pocketHpBar.SetValue(pocketInfo.GetEvolvedAttributes().displayHp);
        pocketSpecialStatsBar.SetValue(pocketInfo.GetEvolvedAttributes().specialStats);
        pocketDropRateBar.SetValue(pocketInfo.GetEvolvedAttributes().dropRate);
        pocketWeaponBar.SetValue(pocketInfo.GetEvolvedAttributes().weapon);
        pocketHealingBar.SetValue(pocketInfo.GetEvolvedAttributes().healing);
        pocketMovementBar.SetValue(pocketInfo.GetEvolvedAttributes().movement);

        PaintExtraBarTiles(pocketHpBar, pocketInfo.GetAttributes().displayHp, pocketInfo.GetEvolvedAttributes().displayHp);
        PaintExtraBarTiles(pocketSpecialStatsBar, pocketInfo.GetAttributes().specialStats, pocketInfo.GetEvolvedAttributes().specialStats);
        PaintExtraBarTiles(pocketDropRateBar, pocketInfo.GetAttributes().dropRate, pocketInfo.GetEvolvedAttributes().dropRate);
        PaintExtraBarTiles(pocketWeaponBar, pocketInfo.GetAttributes().weapon, pocketInfo.GetEvolvedAttributes().weapon);
        PaintExtraBarTiles(pocketHealingBar, pocketInfo.GetAttributes().healing, pocketInfo.GetEvolvedAttributes().healing);
        PaintExtraBarTiles(pocketMovementBar, pocketInfo.GetAttributes().movement, pocketInfo.GetEvolvedAttributes().movement);
    }

    public void PaintExtraBarTiles(AttributeBar bar, int currentValue, int targetValue)
	{
        int first = currentValue + 1;
        int last = targetValue + 1;
        bar.SetColorByInterval(new Vector2Int(first, last), true);
    }
}
