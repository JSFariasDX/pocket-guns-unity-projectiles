using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulwark : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> bulletDamagePercent = new();
    [SerializeField] List<int> bulletForcePercent = new();
    [SerializeField] List<int> pushbackPercent = new();
    [SerializeField] List<int> charSpeedPercent = new();
    [SerializeField] List<bool> projectilePierce = new();

    float bulletDamage;
    float bulletForce;
    float pushback;
    float charSpeed;

    float secondaryBulletDamage;
    float secondaryBulletForce;
    float secondaryPushback;
    float secondaryCharSpeed;

    bool powerUpApplied = false;

    void Start()
    {
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        bulletForce = GetPercentValue(bulletForcePercent[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercent[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);

        secondaryBulletDamage = bulletDamage * .2f;
        secondaryBulletForce = bulletForce * .2f;
        secondaryPushback = pushback * .2f;
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        player.DamageBonus += bulletDamage;
        player.BulletForceBonus += bulletForce;
        player.PushBackPrevention += pushback;
        player.MoveSpeedBonus += charSpeed;

        if (player.ProjectilePierce == false)
            player.ProjectilePierce = projectilePierce[GetCurrentPet().level - 1];
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
        player.BulletForceBonus -= bulletForce;
        player.PushBackPrevention -= pushback;
        player.MoveSpeedBonus -= charSpeed;

        if (!powerUpApplied)
            player.ProjectilePierce = false;

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
        player.MoveSpeedBonus += secondaryCharSpeed;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= secondaryBulletDamage;
        player.BulletForceBonus -= secondaryBulletForce;
        player.PushBackPrevention += secondaryPushback;
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
