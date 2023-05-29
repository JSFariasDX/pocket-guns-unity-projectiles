using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropList", menuName = "Drop List")]
public class DropList : ScriptableObject
{
	public enum DropType
    {
		Coin, Healing, Weapons
    }
	public DropType dropType;

    [SerializeField] private List<Drop> Drops;
	[SerializeField, Range(0f, 1f)] private float DropChance;

    public Drop EvaluateDrop(Player player = null)
	{
        if (Random.value <= DropChance + GetMultiplier(player))
		{
			var randomDrop = Drops[Random.Range(0, Drops.Count)];
			return randomDrop;
		}

        return null;
	}

    float GetMultiplier(Player p)
    {
        float value = 0;

        switch (dropType)
        {
            case DropType.Coin:
                value = p.CoinDropRateModifier;
                break;
            case DropType.Healing:
                value = p.HealthDropRateModifier;
                break;
            case DropType.Weapons:
                value = p.WeaponDropRateModifier;
                break;
            default:
                value = 0;
                break;
        }

        return value;
    }
}

[System.Serializable]
public class Drop
{
	public List<GameObject> Prefabs;
	public int Count;
}