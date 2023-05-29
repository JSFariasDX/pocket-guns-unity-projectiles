using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starman : Special
{
    [SerializeField] List<int> charSpeedPercent = new();

    float charSpeed;

    float secondaryCharSpeed;

    [Header("Material")]
    [SerializeField] private Material playerInvulnerableMaterial;

    Material playerDefaultMaterial;

    private void Start()
    {
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);

        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.ProjectileInvulnerability = true;
        player.ContactInvulnerability = true;
        player.MoveSpeedBonus += charSpeed;

        var pocket = GetCurrentPet();

        player.GetMaterial().SetInt("_Hit", 0);
        player.GetHealth().keepInvulnerable = true;
        player.SetInvulnerability(true);
        player.shouldFlash = false;
        playerDefaultMaterial = player.GetMaterial();
        player.SetMaterial(playerInvulnerableMaterial);

        pocket.GetHealth().keepInvulnerable = true;
        pocket.shouldFlash = false;
        pocket.SetInvulnerability(true);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.ProjectileInvulnerability = false;
        player.ContactInvulnerability = false;
        player.MoveSpeedBonus -= charSpeed;

        var pocket = GetCurrentPet();

        player.GetHealth().keepInvulnerable = false;
        player.SetInvulnerability(false);
        player.SetMaterial(playerDefaultMaterial);
        player.SetSpriteAlpha(1f);
        player.GetMaterial().SetInt("_HitEffectBlend", 0);
        player.shouldFlash = true;

        pocket.GetHealth().keepInvulnerable = false;
        pocket.shouldFlash = true;
        pocket.SetInvulnerability(false);
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
