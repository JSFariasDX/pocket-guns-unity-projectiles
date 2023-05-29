using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerAttributes : MonoBehaviour
{
    [Header("Components")]
    Player player;
    Health playerHealth;

    [Header("Attributes")]
    public float lifePercentage = 100;
    public float speedPercentage = 100;
    public float dashPercentage = 100;
    public float damagePercentage = 100;
    public float reloadPercentage = 100;
    public float ammoPercentage = 100;
    public float bulletForcePercentage = 100;
    
    public enum SpecialAttribute
    {
        Zoy, Unk, Rosa, Irwin
    }

    [Header("Special")]
    public SpecialAttribute character;
    //[HideInInspector]
    public string specialDescription;

    [Header("Unk")]
    int neededEnemies = 10;
    int enemyCount;
    bool healthBonusAdded = false;

    [Header("Rosa")]
    [Range(0, 1)]
    public float damage;
    [Range(0, 50)]
    float increments = 0;
    float lastIncrement = 0;
    bool bonusCalculated = false;

    [Header("Starting PowerUp")]
    public GameObject powerUp;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerHealth = GetComponent<Health>();

        UpdateValues();
    }

    // Start is called before the first frame update
    void Start()
    {
        //print(SelectionManager.Instance.currentCharacter.name);

        if (powerUp)
        {
            Collectible powerUpitself = Instantiate(powerUp, transform.position, Quaternion.identity).GetComponent<Collectible>();
            powerUpitself.transform.GetChild(0).gameObject.SetActive(false);
            powerUpitself.GetComponentInChildren<Light2D>().enabled = false;
            powerUpitself.popUp = null;
            powerUpitself.collectFX = null;
            powerUpitself.SetupCollectibleInfo();
            powerUpitself.InstantCollect(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateValues();
        Special();
    }

    void UpdateValues()
    {
        // Update health
        player.MaxHPBonus += (lifePercentage / 100);

        // Update speed
        player.MoveSpeedBonus += (speedPercentage / 100);

        // Update dash
        player.DashSpeedModifier += (dashPercentage / 100);
        player.DashDurationModifier += (dashPercentage / 100);
        player.DashCooldownModifier += (dashPercentage / 100);
        player.DamageBonus += (damagePercentage / 100);
        player.GunRechargeBonus += (reloadPercentage / 100);
        player.AmmoBonus += (ammoPercentage / 100);
        player.BulletForceBonus += (bulletForcePercentage / 100);
    }

    void Special()
    {
        switch (character)
        {
            case SpecialAttribute.Zoy:
                // None
                break;
            case SpecialAttribute.Unk:
                // Increase health drops from crates

                if (!healthBonusAdded)
                {
                    player.HealthDropRateModifier += 0.3f;
                    healthBonusAdded = true;
                }

                break;
            case SpecialAttribute.Rosa:
                // Increse 0.5% every attribute from each 1% of damage

                damage = 1 - player.GetHealth().GetCurrentPercentage();

                if (damage >= .2f && damage < .5f)
                    RosaAttributes(.1f);
                else if (damage >= .5f && damage < .7f)
                    RosaAttributes(.25f);
                else if (damage >= .7f && damage < .9f)
                    RosaAttributes(.4f);
                else if (damage >= .9f)
                    RosaAttributes(.5f);
                else
                    RosaAttributes(0);

                break;
            case SpecialAttribute.Irwin:
                // No special attributes beyond what's already implemented in the Start method
                break;
            default:
                break;
        }
    }

    void RosaAttributes(float amount)
    {
        if (amount != lastIncrement)
        {
            float add = (amount - lastIncrement);

            player.GunRechargeBonus += add;
            player.DamageBonus += add;
            player.MoveSpeedBonus += add;
            player.BulletForceBonus += add;

            print("Amount: " + (amount).ToString() + "| Difference: " + add);
        }

        lastIncrement = amount;
    }

    public void AddEnemyCount(int amount)
    {
        enemyCount += amount;
    }
}
