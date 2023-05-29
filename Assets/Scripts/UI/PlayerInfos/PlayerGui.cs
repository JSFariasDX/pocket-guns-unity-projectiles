using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D.Animation;

public class PlayerGui : MonoBehaviour
{
    [SerializeField] int version;
    [SerializeField] Image frame;
    [SerializeField] PlayerIndexSettings[] playerIndexSettings;
    [SerializeField] Image characterImage;
    [SerializeField] Image gunImage;
    [SerializeField] Image pocketImage;

    [Header("Bars")]
    public Vector2 radialLimits;
    [SerializeField] Image bulletsBar;
    [SerializeField] Image specialBar;
    [SerializeField] Image pocketBar;
    [SerializeField] Image characterBar;
    [SerializeField] Color32 healthBarColor;
    [SerializeField] Color32 shieldedHealthBarColor;
    [SerializeField] List<Image> shieldIcons = new List<Image>();
    public TextMeshProUGUI specialText;
    [SerializeField] GameObject shieldBarHolder;

    [Header("Blink")]
    [SerializeField] Image characterBlinkBar;
    [SerializeField] AnimationCurve blinkCurve;
    [SerializeField, Range(0, 1)] float blinkIntensity = .5f;
    [SerializeField] float blinkSpeed = 1f;
    bool canBlink = false;
    float curveTime = 0;

    string _currentPocket = null;
    PetType _currentPocketType = PetType.Egg;
    private Gun _currentGun = null;


    Player player;

    [Header("Shields")]
    public AnimationCurve shieldsCurve;
    float shieldCurveTime;
    bool shieldsMoving = false;
    public float shieldCurveSpeed = 1;
    //[Range(0, 6)]
    int armorAmount = 0;
    int previousArmorAmount;

    public void Setup(Player player)
    {
        this.player = player;

        if (FindObjectsOfType<PlayerInputController>().Length > 1) 
        {
            PlayerIndexSettings settings = playerIndexSettings[player.playerIndex];
            frame.sprite = settings.frameSprite;
            player.aim.SetupAim(settings);
        } 
    }

    private void Update()
	{
        UpdateInfos();
        Blink();
        ShieldBarFunctionality();

        if (shieldsMoving) MoveShields();
    }

    void UpdateInfos()
    {
        if (!player.GetCurrentPocket()) return;

        // Player HP 
        characterBar.fillAmount = player.GetHealth().GetCurrentPercentage();
        // Pocket HP
        pocketBar.fillAmount = player.GetCurrentPocket().GetHealth().GetCurrentPercentage();
        // Special Charge
        specialBar.fillAmount = player.GetCurrentPocket().GetSpecial().GetSpecialAmount();
        // Gun charge
        bulletsBar.fillAmount = (float)player.GetCurrentGun().GetCurrentBullets() / (float)player.GetCurrentGun().GetMaxBullets();

        UpdatePocketHudMaterial();

        var samePocket = _currentPocket == player.GetCurrentPocket().pocketName;
        var samePocketType = _currentPocketType == player.GetCurrentPocket().GetComponent<Pocket>().pocketType;
        var sameGun = _currentGun == player.GetCurrentGun();

        if (samePocket && samePocketType && sameGun)
            return;

        UpdateHudImages();
    }

    private void UpdateHudImages()
    {
        _currentPocket = player.GetCurrentPocket().pocketName;
        _currentPocketType = player.GetCurrentPocket().pocketType;
        _currentGun = player.GetCurrentGun();

        // Player icon
        characterImage.sprite = player.characterIcon;
            
        // Gun icon
        gunImage.sprite = player.GetCurrentGun().sideSprite;

        // Pocket Icon
        pocketImage.sprite = player.GetCurrentPocket().GetHudSprite();

        if (player.GetCurrentPocket().flipY && player.GetCurrentPocket().pocketType != PetType.Egg)
            pocketImage.transform.localScale = new Vector3(pocketImage.transform.localScale.x, -pocketImage.transform.localScale.y, pocketImage.transform.localScale.z);
        else
            pocketImage.transform.localScale = new Vector3(pocketImage.transform.localScale.x, Mathf.Abs(pocketImage.transform.localScale.y), pocketImage.transform.localScale.z);
    }

    private void UpdatePocketHudMaterial()
    {
        if (!player.GetCurrentPocket().GetHealth().isAlive)
        {
            pocketImage.material = player.GetCurrentPocket().greyscaleMaterial;
            return;
        }

        if (player.GetCurrentPocket().pocketHudMaterial != null)
            pocketImage.material = player.GetCurrentPocket().pocketHudMaterial;
        else
            pocketImage.material = null;
    }

    // Used in radial bars
    float GetFillAmountBar(float currentValue, float maxValue)
	{
        float fillAmount;
        float interval = radialLimits.y - radialLimits.x;

        float percentage = currentValue / maxValue;
        fillAmount = radialLimits.x + (interval * percentage);

        return fillAmount;
	}

    void Blink()
    {
        if (!canBlink) return;

        curveTime += Time.deltaTime * blinkSpeed;

        if (curveTime > 1)
            curveTime = 0;

        characterBlinkBar.color = new Color(1, 0, 0, blinkCurve.Evaluate(curveTime) * blinkIntensity);
    }

    public void SetBlink(bool value)
    {
        canBlink = value;
        if(!value) curveTime = 0;
    }

    void ShieldBarFunctionality()
    {
        if (!player)
        {
            shieldBarHolder.SetActive(false);
            return;
        }

        armorAmount = player.HitBlocker;

        shieldBarHolder.SetActive(armorAmount > 0);
        if (armorAmount > 0)
            characterBar.color = shieldedHealthBarColor;
        else
            characterBar.color = healthBarColor;

        for (int i = 0; i < shieldIcons.Count; i++)
        {
            if (i >= armorAmount)
                shieldIcons[i].enabled = false;
            else
                shieldIcons[i].enabled = true;
        }

        if(armorAmount != previousArmorAmount)
        {
            // Shake shields
            shieldsMoving = true;
            shieldCurveTime = 0;
        }

        previousArmorAmount = armorAmount;
    }

    void MoveShields()
    {
        RectTransform shields = shieldBarHolder.transform.GetChild(0).GetComponent<RectTransform>();

        shieldCurveTime += Time.deltaTime * shieldCurveSpeed;
        shields.anchoredPosition = new Vector2(shields.anchoredPosition.x, shieldsCurve.Evaluate(shieldCurveTime));
        if(shieldCurveTime > 1)
        {
            shieldsMoving = false;
        }
    }
}

[System.Serializable]
public struct PlayerIndexSettings
{
    public Sprite frameSprite;
    public Sprite shootingCrosshair;
    public Sprite pistolCrosshair;
    public Sprite machinegunCrosshair;
    public Sprite shotgunCrosshair;
    public Sprite bazookaCrosshair;

    public Sprite GetAimSprite(Gun gun)
    {
        switch (gun.gunType)
		{
            case GunType.Pistol: return pistolCrosshair;
            case GunType.MachineGun: return machinegunCrosshair;
            case GunType.Shotgun: return shotgunCrosshair;
            default: return bazookaCrosshair;
		}
    }
}