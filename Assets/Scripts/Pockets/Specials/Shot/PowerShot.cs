using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PowerShot : Special
{
    Gun gun;

    bool isChangingColor = false;

    Color32[] flashColors =
    {
        new Color32(255, 0, 255, 255),
        new Color32(255, 255, 0, 255),
        new Color32(0, 255, 165, 255),
    };
    int currentFlash = 0;

    [Header("Special custom parameters")]
    [SerializeField] List<int> bulletDamagePercent = new();
    [SerializeField] List<int> bulletForcePercent = new();
    [SerializeField] List<int> pushbackPercent = new();

    float bulletDamage;
    float bulletForce;
    float pushback;

    float secondaryBulletDamage;
    float secondaryBulletForce;
    float secondaryPushback;

    private void Start()
    {
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        bulletForce = GetPercentValue(bulletForcePercent[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercent[GetCurrentPet().level - 1]);

        secondaryBulletDamage = bulletDamage * .2f;
        secondaryBulletForce = bulletForce * .2f;
        secondaryPushback = pushback * .2f;

        
    }

    public override void Update()
    {
        base.Update();
        if (IsActivated() && !isChangingColor)
        {
            gun = GetCurrentPlayer().GetCurrentGun();

            if (gun != null)
                StartCoroutine(FlashColors());
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.DamageBonus += bulletDamage;
        player.BulletForceBonus += bulletForce;
        player.PushBackPrevention += pushback;

        List<Gun> guns = player.GetAllGuns();

        foreach (var item in guns)
        {
            item.SetTrail(true);
        }
    }

    public override void OnEnd()
    {
        //filter = GameObject.FindGameObjectWithTag("PowerShot");
        //filter.GetComponent<SpriteRenderer>().enabled = false;
        player.DamageBonus -= bulletDamage;
        player.BulletForceBonus -= bulletForce;
        player.PushBackPrevention -= pushback;
        currentFlash = 0;

        List<Gun> guns = player.GetAllGuns();

        foreach (var item in guns)
        {
            item.SetTrail(false);
        }

        base.OnEnd();
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.DamageBonus += secondaryBulletDamage;
        player.BulletForceBonus += secondaryBulletForce;
        player.PushBackPrevention -= secondaryPushback;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= secondaryBulletDamage;
        player.BulletForceBonus -= secondaryBulletForce;
        player.PushBackPrevention += secondaryPushback;
    }

    void SetNextColor()
    {
        if (currentFlash < flashColors.Length - 1)
        {
            currentFlash++;
        } else
        {
            currentFlash = 0;
        }

        gun.SetCurrentTint(flashColors[currentFlash]);
    }

    IEnumerator FlashColors()
    {
        isChangingColor = true;
        SetNextColor();
        yield return new WaitForSeconds(0.125f);
        isChangingColor = false;
    }
}
