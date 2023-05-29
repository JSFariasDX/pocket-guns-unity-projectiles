using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCare : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> charHealingPercent = new();
    [SerializeField] List<int> primaryPocketHealingPercent = new();

    float charHealing;
    float primaryPocketHealing;

    private void Start()
    {
        charHealing = GetPercentValue(charHealingPercent[GetCurrentPet().level - 1]);
        primaryPocketHealing = GetPercentValue(primaryPocketHealingPercent[GetCurrentPet().level - 1]);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.GetHealth().Heal(charHealing);
        player.GetCurrentPocket().GetHealth().Heal(primaryPocketHealing);
        player.AddShield(4);

        if (specialParticle)
        {
            SpecialParticles particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }
}
