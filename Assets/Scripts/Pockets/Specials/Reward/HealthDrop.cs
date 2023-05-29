using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : Special
{
    [SerializeField] int charHealPercent = 15;
    [SerializeField] int primaryPetHealPercent = 15;
    [SerializeField] int secondaryPetsHealPercent = 10;

    float charHeal;
    float primaryPetHeal;
    float secondaryPetsHeal;

    private void Start()
    {
        charHeal = GetPercentValue(charHealPercent);
        primaryPetHeal = GetPercentValue(primaryPetHealPercent);
        secondaryPetsHeal = GetPercentValue(secondaryPetsHealPercent);
    }

    public override void OnActivate()
    {
        Start();
        player.GetHealth().Heal(charHeal);
        player.GetCurrentPocket().GetHealth().Heal(primaryPetHeal);
        List<Pocket> secondarys = player.GetSecondaryPockets();
        foreach (var pocket in secondarys)
        {
            pocket.GetHealth().Heal(secondaryPetsHeal);
        }

        base.OnActivate();
    }
}
