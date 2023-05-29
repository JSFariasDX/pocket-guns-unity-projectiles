using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerFantasy : Special
{
    [SerializeField]
    public List<GameObject> powerUpPossibilities;

    [SerializeField]
    public List<GameObject> gunPossibilities;

    public override void OnActivate()
    {
        base.OnActivate();
        int sortedGun = Random.Range(0, gunPossibilities.Count);
        GameObject gun = Instantiate(gunPossibilities[sortedGun], player.transform.position, Quaternion.identity);
        GameplayManager.Instance.clearOnDungeonEnd.Add(gun);
        int sorted = Random.Range(0, powerUpPossibilities.Count);
        GameObject pup = Instantiate(powerUpPossibilities[sorted], player.transform.position, Quaternion.identity);
        GameplayManager.Instance.clearOnDungeonEnd.Add(pup);

        if (specialParticle)
        {
            SpecialParticles particle1 = Instantiate(specialParticle, transform.position, Quaternion.identity);
            SpecialParticles particle2 = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle1.Setup(gun.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
            particle2.Setup(pup.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }
}
