using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGun : Special
{
    [SerializeField]
    public List<GameObject> gunPossibilities;

    public override void OnActivate()
    {
        base.OnActivate();
        int sorted = Random.Range(0, gunPossibilities.Count);
        GameObject gun = Instantiate(gunPossibilities[sorted], player.transform.position, Quaternion.identity);
        GameplayManager.Instance.clearOnDungeonEnd.Add(gun);

        if (specialParticle)
        {
            SpecialParticles particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(gun.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }
}
