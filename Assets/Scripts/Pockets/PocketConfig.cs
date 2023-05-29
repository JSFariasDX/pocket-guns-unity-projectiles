using UnityEngine;

[CreateAssetMenu(fileName = "PocketConfig_", menuName = "PocketGuns/Pockets/PocketConfig")]
public class PocketConfig : ScriptableObject
{
    public string pocketName;
    public PetEssence pocketType;
    public int hitPoints;
    public float charHealPercentage;
    public float primaryPocketHealPercentage;
    public float secondaryPocketHealPercentage;

    public bool projectileInvulnerability;
    public bool contactInvulnerability;
    public bool projectileObstaclesBypass;
    public bool projectileKeepAlive;
    public bool charObstacleBypass;
    public bool isGlobal;
    public float globalDamage;
    public int hitBlocks;

    public string specialName;
    public string specialDescription;
    public SpecialUseType specialUseType;
    public int specialCharges;
    public int specialDuration;

    public float coinDropRateBonus;
    public float mementoDropRateBonus;
    public float weaponDropRateBonus;
    public float powerUpDropRateBonus;
    public float powerUpEmpoweringRateBonus;
    public float secondaryPocketEffectEmpoweringRateBonus;
    public int forceCoinDropAmount;
    public string forceMementoDropID;
    public string forceWeaponDropID;
    public string forcePowerUpDropID;

    public float bulletDamageBonus;
    public float ammoBonus;
    public float rechargeRateBonus;
    public float bulletForceBonus;
    public float fireRateBonus;
    public float pushBackBonus;
    public float bulletDistanceBonus;
    public float recoilPreventBonus;
    public float bulletSizeBonus;

    public float charHealthBonus;
    public float charSpeedBonus;
    public float charDashDistanceBonus;
    public float charDashSpeedBonus;
    public float charDashTimeBonus;
    public float charCooldownBonus; //??


}