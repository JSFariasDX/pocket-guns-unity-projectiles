using UnityEngine;

public class PowerDefense : Special
{
    [SerializeField] private Material playerInvulnerableMaterial;

    Material playerDefaultMaterial;
    public override void OnActivate()
    {
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
        base.OnActivate();
    }

    public override void OnEnd()
    {
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
        base.OnEnd();
    }
}
