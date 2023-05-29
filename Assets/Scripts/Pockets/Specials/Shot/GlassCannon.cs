using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassCannon : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> bulletDamagePercent = new();
    [SerializeField] List<int> pushbackPercent = new();
    [SerializeField] List<int> charMaxHPPercent = new();
    [SerializeField] List<bool> contactInvulnerability = new();

    float bulletDamage;
    float pushback;
    float charMaxHP;

    float secondaryBulletDamage;
    float secondaryPushback;
    float secondaryCharMaxHP;

    bool powerUpApplied = false;

    private void Start()
    {
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercent[GetCurrentPet().level - 1]);
        charMaxHP = GetPercentValue(charMaxHPPercent[GetCurrentPet().level - 1]);

        secondaryBulletDamage = bulletDamage * .2f;
        secondaryPushback = pushback * .2f;
        secondaryCharMaxHP = charMaxHP * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.DamageBonus += bulletDamage;
        player.PushBackPrevention += pushback;
        player.MaxHPBonus += charMaxHP;

        if (player.ContactInvulnerability == false)
            player.ContactInvulnerability = contactInvulnerability[GetCurrentPet().level - 1];
        else
            powerUpApplied = true;

        List<Gun> guns = player.GetAllGuns();

        foreach (var item in guns)
        {
            item.SetTrail(true);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.DamageBonus -= bulletDamage;
        player.PushBackPrevention -= pushback;
        player.MaxHPBonus -= charMaxHP;

        if (!powerUpApplied)
            player.ContactInvulnerability = true;

        List<Gun> guns = player.GetAllGuns();

        foreach (var item in guns)
        {
            item.SetTrail(false);
        }
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.DamageBonus += secondaryBulletDamage;
        player.PushBackPrevention -= secondaryPushback;
        player.MaxHPBonus += secondaryCharMaxHP;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= secondaryBulletDamage;
        player.PushBackPrevention += secondaryPushback;
        player.MaxHPBonus -= secondaryCharMaxHP;
    }
}
