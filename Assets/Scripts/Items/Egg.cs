using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Collectible
{
    [SerializeField]
    public override void onPlayerCollect(Player player)
    {
        GameObject bossSpawn = GameObject.FindGameObjectWithTag("BossSpawn");
        bossSpawn.transform.Find("Portal").gameObject.SetActive(true);
        base.onPlayerCollect(player);
    }
}
