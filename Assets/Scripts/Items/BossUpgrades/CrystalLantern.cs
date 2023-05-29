using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalLantern : Collectible
{
    [Header("Lantern Settings")]
    [SerializeField] private Vector2 lightRadius;
    [SerializeField] private float lightIntesity;
    [SerializeField] private Color lightColor;

    public override void onPlayerCollect(Player player)
    {
        foreach (Player p in GameplayManager.Instance.GetPlayers(false))
        {
            p.SetLightRadius(lightRadius.y, lightRadius.x);
            p.SetLightIntensity(lightIntesity);
            p.SetLightColor(lightColor);
        }

        base.onPlayerCollect(player);
    }
}
