using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootGun : Loot
{
	public Gun gunPrefab;
	public Gun spawnedGun;
	public GunType gunType;
	public SpriteRenderer spriteRenderer;

	public bool isDroppedByPlayer;

	protected override void Start()
	{
		base.Start();

		if (!isDroppedByPlayer)
		{
			spawnedGun = Instantiate(gunPrefab, transform);
			spawnedGun.transform.localScale = Vector3.zero;
			spawnedGun.canShoot = false;
			spawnedGun.Awake();
			GunConfig[] possibleGuns = Resources.LoadAll<GunConfig>("Data/Guns/" + GetFolderName(gunType));
			GunConfig sortedGC = SortGunConfig(possibleGuns);
			spawnedGun.name = sortedGC.displayName;
			spawnedGun.Setup(sortedGC);
			spriteRenderer.sprite = sortedGC.sideView;
		}
		
		GameplayManager.Instance.clearOnDungeonEnd.Add(gameObject);
	}

	public override void GetLoot(Player player)
	{
		base.GetLoot(player);

		// Find respective cateogy gun index
		int gunIndex = player.GetGunIndexByType(spawnedGun.gunType);
		if (gunIndex == player.currentGuns.Count)
		{
			player.currentGuns.Add(null);
		}

		// Get current player gun
		Gun currentGun = player.currentGuns[gunIndex];

		// Drop current player gun
		if (currentGun != null)
		{
			currentGun.AbortRecharge();
			LootGun dropped = Instantiate(currentGun.lootPrefab, transform.position, Quaternion.identity, player.isOnTutorial ? transform.parent : null);
			dropped.isDroppedByPlayer = true;
			dropped.spawnedGun = currentGun;
			dropped.name = currentGun.gunName;
			dropped.spriteRenderer.sprite = currentGun.sideSprite;
			dropped.spawnedGun.canShoot = false;
			dropped.spawnedGun.transform.localScale = Vector3.zero;
			dropped.spawnedGun.transform.parent = dropped.transform;
		}

		// Active new gun
		spawnedGun.transform.parent = player.gunSlot;
		spawnedGun.transform.position = player.gunSlot.position;
		spawnedGun.gameObject.SetActive(true);
		player.aim.SetGunLookToAim(spawnedGun);
		player.currentGuns[gunIndex] = spawnedGun;
		spawnedGun.SetPlayer(player);
		player.SetGun(gunIndex);
		player.OnFacingDirectionChange(player.facingDirection);

		// Destroy loot
		Destroy(gameObject);
	}

	public override void ShowPopUp(Player player)
	{
		GetPopup().ShowGunInfo(shadowSprite.transform, spawnedGun, player);
	}

	string GetFolderName(GunType t)
    {
		switch (t)
        {
			case GunType.Shotgun:
				return "Shotgun";
			case GunType.Bazooka:
				return "Bazooka";
			case GunType.MachineGun:
				return "Machinegun";
			default:
				return "Pistol";
        }
    }

	GunConfig SortGunConfig(GunConfig[] options)
    {
		int sorted = Random.Range(0, options.Length);
		return options[sorted];
    }
}
