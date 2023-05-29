using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerConcot : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        Health playerHealth = player.GetComponent<Health>();
        float maxHealth = playerHealth.GetMaxHealth();
        float healAmount = maxHealth * base.primaryReferenceAmount;
        playerHealth.Increase(healAmount);

        base.onPlayerCollect(player);
    }
}
