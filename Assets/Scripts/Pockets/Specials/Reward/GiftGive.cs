using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftGive : Special
{
    [SerializeField]
    public List<GameObject> powerUpPossibilities;

    public override void OnActivate()
    {
        base.OnActivate();
        int sorted = Random.Range(0, powerUpPossibilities.Count);
        GameObject pup = Instantiate(powerUpPossibilities[sorted], player.transform.position, Quaternion.identity);
        GameplayManager.Instance.clearOnDungeonEnd.Add(pup);

        if (specialParticle)
        {
            SpecialParticles particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(pup.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }
}
