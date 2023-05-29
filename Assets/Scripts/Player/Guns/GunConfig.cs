using Assets.SimpleLocalization;
using UnityEngine;

[CreateAssetMenu(fileName = "GunConfig_", menuName = "PocketGuns/Guns/GunConfig")]
public class GunConfig : ScriptableObject
{
    public GunType gunType;
    public string displayName;
    public string specialName;
    public Sprite sideView;
    public Sprite topView;
    public string description;
    public int baseBulletDamage;
    public int baseAmmoAmount;
    public float baseRechargeTime;
    public int basePushBack;
    public float baseFireRate;
    public int baseBulletForce;
    public int baseBulletDistance;
    public int baseInaccuracy;
    public int baseInaccuracyMaxBullets;
    public float baseBulletSize;
    public int bulletAmountPerFire = 1;
    public bool isRadialFire;
    public int radialAngle;
    public bool scatteredShots;
    public bool hasSplash;
    public float splashBaseDamagePercent;
    public float splashBaseRadius;
    public Gradient particleGradient;

    [Header("Visual Informations")]
    public int damageInfo;
    public int ammoInfo;
    public int rechargeInfo;
    public int fireRateInfo;

    public string GetLocalizedName()
	{
        return LocalizationManager.Localize("Weapon." + displayName);
    }
}
