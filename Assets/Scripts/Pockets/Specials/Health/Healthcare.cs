using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthcare : Special
{

    [Header("Special custom parameters")]
    [SerializeField] List<int> secondaryPocketsHealingPercent = new();
    float secondaryPocketHealing;

    private void Start()
    {
        secondaryPocketHealing = GetPercentValue(secondaryPocketsHealingPercent[GetCurrentPet().level - 1]);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        List<Pocket> secondarys = player.GetSecondaryPockets();
        foreach (var pocket in secondarys)
        {
            pocket.GetHealth().Heal(secondaryPocketHealing);
        }

        if (specialParticle)
        {
            SpecialParticles particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }
}
