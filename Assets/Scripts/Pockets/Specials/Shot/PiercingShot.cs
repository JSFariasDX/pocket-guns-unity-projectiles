using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingShot : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> bulletDamagePercentage = new();
    [SerializeField] List<bool> projectileFlight = new();
    [SerializeField] List<bool> projectilePierce = new();

    float damage;

    float secondaryDamage;

    bool powerUpApplied = false;

    private void Start()
    {
        damage = GetPercentValue(bulletDamagePercentage[GetCurrentPet().level - 1]);

        secondaryDamage = damage * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        if (player.ProjectilePierce == false || player.ProjectileFlight == false)
        {
            player.ProjectilePierce = projectilePierce[GetCurrentPet().level - 1];
            player.ProjectileFlight = projectileFlight[GetCurrentPet().level - 1];
        }
        else
            powerUpApplied = true;

        player.DamageBonus += damage;

        List<Gun> guns = player.GetAllGuns();

        foreach (var item in guns)
        {
            item.SetTrail(true);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        if (!powerUpApplied)
        {
            player.ProjectilePierce = false;
            player.ProjectilePierce = false;
        }

        player.DamageBonus -= damage;

        List<Gun> guns = player.GetAllGuns();

        foreach (var item in guns)
        {
            item.SetTrail(false);
        }
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();

        player.DamageBonus += secondaryDamage;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();

        player.DamageBonus -= secondaryDamage;
    }

}
