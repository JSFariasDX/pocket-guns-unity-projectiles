using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstResponse : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> charHealingPercent = new();
    [SerializeField] List<int> primaryPocketHealingPercent = new();
    [SerializeField] List<int> charSpeedPercent = new();

    float charHealing;
    float primaryPocketHealing;
    float charSpeed;
    float secondaryCharSpeed;

    private void Start()
    {
        charHealing = GetPercentValue(charHealingPercent[GetCurrentPet().level - 1]);
        primaryPocketHealing = GetPercentValue(primaryPocketHealingPercent[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.GetHealth().Heal(charHealing);
        player.GetCurrentPocket().GetHealth().Heal(primaryPocketHealing);
        player.MoveSpeedBonus += charSpeed;

        if (specialParticle)
        {
            SpecialParticles particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.MoveSpeedBonus -= charSpeed;
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.MoveSpeedBonus += secondaryCharSpeed;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
