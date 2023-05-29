using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfBottle : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        Health playerHealth = player.GetComponent<Health>();
        float maxHealth = playerHealth.GetMaxHealth();
        float healAmount = maxHealth * base.primaryReferenceAmount;
        playerHealth.Increase(healAmount);

        Pocket playerPocket = player.GetCurrentPocket();
        if (playerPocket != null)
        {
            Health pocketHealth = playerPocket.GetComponent<Health>();
            float pocketMaxHealth = pocketHealth.GetMaxHealth();
            float pocketHealAmount = pocketMaxHealth * base.primaryReferenceAmount;
            pocketHealth.Increase(pocketHealAmount);
        }

        base.onPlayerCollect(player);
    }
}
