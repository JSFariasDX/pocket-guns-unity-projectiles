using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> bulletDamagePercent = new();
    [SerializeField] List<int> bulletForcePercent = new();
    [SerializeField] List<int> pushbackPercent = new();
    [SerializeField] List<int> charMaxHPPercent = new();
    [SerializeField] List<int> charSpeedPercent = new();

    float bulletDamage;
    float bulletForce;
    float pushback;
    float charMaxHP;
    float charSpeed;

    float secondaryBulletDamage;
    float secondaryBulletForce;
    float secondaryPushback;
    float secondaryMaxHP;
    float secondaryCharSpeed;

    private void Start()
    {
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        bulletForce = GetPercentValue(bulletForcePercent[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercent[GetCurrentPet().level - 1]);
        charMaxHP = GetPercentValue(charMaxHPPercent[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);

        secondaryBulletDamage = bulletDamage * .2f;
        secondaryBulletForce = bulletForce * .2f;
        secondaryPushback = pushback * .2f;
        secondaryMaxHP = charMaxHP * .2f;
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();

        Start();
        player.DamageBonus += bulletDamage;
        player.BulletForceBonus += bulletForce;
        player.PushBackPrevention += pushback;
        player.MaxHPBonus += charMaxHP;
        player.MoveSpeedBonus += charSpeed;

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
        player.BulletForceBonus -= bulletForce;
        player.PushBackPrevention -= pushback;
        player.MaxHPBonus -= charMaxHP;
        player.MoveSpeedBonus -= charSpeed;

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
        player.BulletForceBonus += secondaryBulletForce;
        player.PushBackPrevention -= secondaryPushback;
        player.MaxHPBonus += secondaryMaxHP;
        player.MoveSpeedBonus += secondaryCharSpeed;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= secondaryBulletDamage;
        player.BulletForceBonus -= secondaryBulletForce;
        player.PushBackPrevention += secondaryPushback;
        player.MaxHPBonus -= secondaryMaxHP;
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
