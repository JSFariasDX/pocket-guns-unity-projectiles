using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellableGun : Collectible
{
    [SerializeField] private GunConfig gunConfig;
    [SerializeField] private Gun gunPrefab;
    private Gun _spawnedGun;

    protected override void Awake()
    {
        base.Awake();
		_spawnedGun = Instantiate(gunPrefab, transform);
		_spawnedGun.Setup(gunConfig);

		SetName(gunConfig.displayName);

        var description = $"Damage: {_spawnedGun.damage}<br>" +
        $"Bullets: {_spawnedGun.maxBullets}<br>" +
        $"Fire Rate: {_spawnedGun.fireRate.ToString("F")}";
        SetDescription(description);
    }

    protected override void Start()
    {
        base.Start();

		_spawnedGun.gameObject.SetActive(true);

        _spawnedGun.transform.localScale = Vector3.zero;
        _spawnedGun.canShoot = false;
        _spawnedGun.Awake();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.sprite = gunConfig.sideView;
    }

    public override void onPlayerCollect(Player player)
    {
        // Find respective cateogy gun index
		int gunIndex = player.GetGunIndexByType(_spawnedGun.gunType);
		if (gunIndex == player.currentGuns.Count)
		{
			player.currentGuns.Add(null);
		}

		// Get current player gun
		Gun currentGun = player.currentGuns[gunIndex];

		// Drop current player gun
		if (currentGun != null)
		{
			LootGun dropped = Instantiate(currentGun.lootPrefab, transform.position, Quaternion.identity);
			dropped.isDroppedByPlayer = true;
			dropped.spawnedGun = currentGun;
			dropped.name = currentGun.gunName;
			dropped.spriteRenderer.sprite = currentGun.sideSprite;
			dropped.spawnedGun.canShoot = false;
			dropped.spawnedGun.transform.localScale = Vector3.zero;
			dropped.spawnedGun.transform.parent = dropped.transform;
		}

		// Active new gun
		_spawnedGun.transform.parent = player.gunSlot;
		_spawnedGun.transform.position = player.gunSlot.position;
		_spawnedGun.gameObject.SetActive(true);
		player.aim.SetGunLookToAim(_spawnedGun);
		player.currentGuns[gunIndex] = _spawnedGun;
		_spawnedGun.SetPlayer(player);
		player.SetGun(gunIndex);
		player.OnFacingDirectionChange(player.facingDirection);

        base.onPlayerCollect(player);
    }
}
