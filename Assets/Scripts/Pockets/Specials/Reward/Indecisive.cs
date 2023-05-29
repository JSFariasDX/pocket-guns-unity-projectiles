using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indecisive : Special
{
    [SerializeField]
    public List<GameObject> gunPossibilities;
    [SerializeField]
    public List<GameObject> powerUpPossibilities;
    [SerializeField] List<int> shields = new();

    public override void OnActivate()
    {
        base.OnActivate();
        player.AddShield(shields[GetCurrentPet().level - 1]);
        DropCoins dropCoin = GetComponentInParent<DropCoins>();
        dropCoin.DropCoin(player.transform.position, true);
        // drop 1 memento
        int sorted = Random.Range(0, gunPossibilities.Count);
        GameObject gun = Instantiate(gunPossibilities[sorted], player.transform.position, Quaternion.identity);
        GameplayManager.Instance.clearOnDungeonEnd.Add(gun);
        int sortedPup = Random.Range(0, powerUpPossibilities.Count);
        GameObject pup = Instantiate(powerUpPossibilities[sortedPup], player.transform.position, Quaternion.identity);
        GameplayManager.Instance.clearOnDungeonEnd.Add(pup);

        if (specialParticle)
        {
            SpecialParticles particle1 = Instantiate(specialParticle, transform.position, Quaternion.identity);
            SpecialParticles particle2 = Instantiate(specialParticle, transform.position, Quaternion.identity);
            SpecialParticles particle3 = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle1.Setup(gun.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
            particle2.Setup(pup.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
            particle3.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }
}
