using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEgg : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        GameObject bossSpawn = GameObject.FindGameObjectWithTag("BossSpawn");
        bossSpawn.transform.Find("Portal").gameObject.SetActive(true);

        if (GetComponent<TutorialEvents>())
            GetComponent<TutorialEvents>().CompleteTutorial();

        base.onPlayerCollect(player);
    }
}
