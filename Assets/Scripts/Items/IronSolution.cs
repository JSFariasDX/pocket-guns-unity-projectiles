using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSolution : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.MaxHPBonus += primaryReferenceAmount;

        Health playerHealth = player.GetComponent<Health>();
        float maxHealth = playerHealth.GetMaxHealth();
        //playerHealth.SetHealth(maxHealth);

        base.onPlayerCollect(player);
    }
}
