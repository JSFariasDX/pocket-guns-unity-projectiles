using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflektor : Special
{
    [Header("Material")]
    [SerializeField] private Material playerInvulnerableMaterial;

    Material playerDefaultMaterial;

    public override void OnActivate()
    {
        base.OnActivate();
        player.ContactInvulnerability = true;
        player.ProjectileInvulnerability = true;
        player.GetCurrentPocket().GetHealth().SetInvulnerability(true);

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
        player.ContactInvulnerability = false;
        player.ProjectileInvulnerability = false;
        player.GetCurrentPocket().GetHealth().SetInvulnerability(false);

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
}
