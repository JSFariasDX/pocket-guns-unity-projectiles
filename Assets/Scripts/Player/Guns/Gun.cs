using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using System;

public enum GunSpritePerspective
{
    Side = 1,
    Top = 2,
}

public enum GunType
{
    Pistol = 0,
    Shotgun = 1,
    MachineGun = 2,
    Bazooka = 3,
    Special = 4
}

public enum Rarity
{
    Common = 0, Uncommon = 1, Rare = 2, SuperRare = 3, Legendary = 4
}

public class Gun : MonoBehaviour
{
    public string gunName;
    public GunType gunType;
    public Rarity rarity;
    [SerializeField] private bool randomizeValues;

    bool canRumble = true;

    [Header("Splash Damage")]
    public float splashRadius;

    [Header("References")]
    public LootGun lootPrefab;
    public Sprite sideSprite;
    public Sprite topSprite;
    public Transform firepoint;
    public GameObject bulletPrefab;
    public bool setBulletParticleColor = true;
    public GameObject bulletParticlePrefab;
    public GameObject shockwaveParticlePrefab;
    public GameObject enemyHitParticlePrefab;
    public Light2D fireFloorLight;
    public Collider2D backfire;

    [Header("Base parameters")]
    public float bulletSize = 1;
    [HideInInspector] public float baseFireRate;
    [HideInInspector] public float baseBulletForce;
    [HideInInspector] public float baseBulletLivingTime;

    [Header("Sounds")]
    public AudioClip shootSound;
    public AudioClip emptySound;
    public AudioClip reloadSound;
    public AudioClip bulletOnHitSolidSound;
    public AudioClip equipSound;

    [Header("Laser Sight")]
    public LayerMask laserLayers;
    LineRenderer laserSight;
    //public SpriteRenderer laserPointer;
    bool hasLaserSight = false;

    [Header("Current Run Settings")]
    public bool canShoot = true;
    public int damage = 10;
    public int maxBullets;
    public int currentBullets;
    public float rechargeRate = 0.8f;
    public float basePushback = 4;
    public float fireRate = 0.4f;
    public float bulletForce = 30f;
    public float bulletDistance = 0.16f;
    public float shootInstability;
    [Range(0, 100)] public float howManyBulletsPercentage = 50;
    [HideInInspector] public float actualInstability = 0;
    [SerializeField] [Range(0, 1)] float instabilityMultiplier = 0;
    int bulletBonus;

    float reloadRate;
    float reloadTimer;

    float actualReloadRate;

    public float recoilFactor = 0;

    protected Player player;
    protected AudioSource audioSource;
    public SpriteRenderer gunRenderer;
    PlayerControls controls;

    bool isShootHold;
    protected bool isShooting = false;
    protected bool isRecharging = false;

    float currentFireRateModifier = 0;

    [Header("Color")]
    public Color32 bulletTint;
    [HideInInspector] public Color32 currentTint;
    [HideInInspector] public GunSpritePerspective currentPerspective;

    private Coroutine rechargeRoutine;

    bool started;

    [Header("Gun Setup")]
    public GunConfig gunConfig;

    bool isCurrentDebug = false;

    List<GameObject> debugObjects = new List<GameObject>();

    float currentBulletBonus = 0;

    bool hasTrail = false;
    
    public void Awake()
    {
        if (!started)
        {
            if (gunConfig != null)
            {
                Setup(gunConfig);
            }
            audioSource = GetComponent<AudioSource>();
            currentBullets = maxBullets;

            laserSight = GetComponentInChildren<LineRenderer>();

            currentTint = bulletTint;
            started = true;
        }
    }

    private void Start()
    {
        if (gunConfig != null && gunConfig.hasSplash && shockwaveParticlePrefab != null)
            bulletParticlePrefab = shockwaveParticlePrefab;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.Instance.IsGamePaused())
        {
            SetShootHold(false);
            if (audioSource.isPlaying) audioSource.Pause();
            return;
        }
        else
        {
            if (!audioSource.isPlaying) audioSource.UnPause();
        }

        if (!player) return;

        fireFloorLight.color = currentTint;
        //isShootHold = controls.Gameplay.Fire.IsPressed();
        bool isFullCharged = currentBullets >= maxBullets;

        bool shouldShootCurrentWeapon = canShoot && isShootHold && !isShooting;
        if (shouldShootCurrentWeapon)
        {
            if (!player.isDashing && !player.IsExternalForcingMovement())
            {
                if (ScreenManager.currentScreen == Screens.Lobby && !GetPlayer().isOnTutorial) return;

                if (currentBullets > 0 && !isRecharging)
                {
                    StartCoroutine(Shoot());
                }
                else
                {
                    if (!audioSource.isPlaying)
                    {
                        PlaySfx(emptySound);
                    }
                }
            }
        }

        float reloadBonus = reloadRate * GetPlayer().GunRechargeBonus;
        actualReloadRate = reloadRate + reloadBonus;

        //bool shouldRechargeForBeingIdle = !canShoot && !isFullCharged && !isRecharging;
        //bool shouldRechargeForNotShooting = !isShootHold && !isFullCharged && !isRecharging;
        //if (shouldRechargeForBeingIdle || shouldRechargeForNotShooting)
        //{
        //    float rechargeBonus = rechargeRate * player.GunRechargeBonus;
        //    float finalRechargeRate = rechargeRate - rechargeBonus;
        //    rechargeRoutine = StartCoroutine(Recharge(finalRechargeRate));
        //}

        if (isRecharging)
        {
            if (reloadTimer < actualReloadRate)
            {
                reloadTimer += Time.deltaTime;

                GetPlayer().ReloadValues(reloadTimer / actualReloadRate);
            }
            else
            {
                currentBullets = maxBullets;

                PlaySfx(reloadSound);
                StartCoroutine(IndicateReload());

                isRecharging = false;
            }
        }

        //player.ControlReloadText(currentBullets <= (Mathf.RoundToInt(maxBullets * 0.10f)) ? true : false);
        #region Accuracy

        if (!isShooting)
        {
            if (instabilityMultiplier > 0.05f)
                instabilityMultiplier -= Time.deltaTime / 2;
            else
                instabilityMultiplier = 0.05f;
        }

        if (float.IsNaN(instabilityMultiplier))
            instabilityMultiplier = 0;

        actualInstability = shootInstability * instabilityMultiplier;

        //LaserSight(player.IsLaserSightOn());

        #endregion

        int rumble = PlayerPrefs.GetInt("Rumble", 1);
        canRumble = rumble == 1 ? true : false;
    }

    void LaserSight(bool hasLaser)
    {
        if (hasLaser)
        {
            RaycastHit2D hit = Physics2D.Raycast(laserSight.transform.position, transform.right, 1000, laserLayers);
            laserSight.SetPosition(0, laserSight.transform.position);
            laserSight.SetPosition(1, hit.point);
        }
        else
        {
            laserSight.SetPosition(0, firepoint.position);
            laserSight.SetPosition(1, firepoint.position);
        }

        laserSight.enabled = hasLaser;
    }

    public void SetLaserSight(bool set)
    {
        hasLaserSight = set;
    }

    private void FixedUpdate()
    {
        if (player != null && currentBulletBonus != player.AmmoBonus)
        {
            bulletBonus = Mathf.CeilToInt(maxBullets * player.AmmoBonus);
            int totalBullets = maxBullets + bulletBonus;
            //Debug.Log("BULLET BONUS");
            //Debug.Log(maxBullets);
            //Debug.Log(bulletBonus);
            //Debug.Log(totalBullets);
            maxBullets = totalBullets;
            currentBulletBonus = player.AmmoBonus;
        }
        if (player != null && currentFireRateModifier != player.FirerateBonus)
        {
            HandleFireRateChange(player.FirerateBonus);
        }
            if (DebugHelper.Instance != null && DebugHelper.Instance.isActivated)
        {
            HandleDebug();
        }
    }

    public void DeductBullets()
    {
        maxBullets -= bulletBonus;
    }

    private void HandleFireRateChange(float modifierValue)
    {
        float modifier = Mathf.Abs(modifierValue);
        float difference = baseFireRate * modifier;
        float result;
        if (modifierValue > 0)
        {
            result = baseFireRate - difference;
        }
        else
        {
            result = baseFireRate + difference;
        }
        fireRate = result;
        currentFireRateModifier = modifierValue;
    }

    void HandleDebug()
    {
        if (canShoot && !isCurrentDebug)
        {
            SetupDebug();
        } else if ((!canShoot || player == null) && isCurrentDebug)
        {
            ClearDebug();
        }
    }

    public bool ShootIsBlocked()
    {
        if (isEquipping) return true;
        return false;
    }

    public void Setup(GunConfig gc)
    {
        gunConfig = gc;
        gunName = gc.displayName;
        damage = randomizeValues ? Mathf.RoundToInt(gc.baseBulletDamage * UnityEngine.Random.Range(0.9f, 1.1f)) : Mathf.RoundToInt(gc.baseBulletDamage);

        if (gc.baseAmmoAmount > 1)
            maxBullets = randomizeValues ? Mathf.RoundToInt(gc.baseAmmoAmount * UnityEngine.Random.Range(0.9f, 1.1f)) : Mathf.RoundToInt(gc.baseAmmoAmount);
        else
            maxBullets = gc.baseAmmoAmount;
        
        currentBullets = maxBullets;

        rechargeRate = randomizeValues ? gc.baseRechargeTime * UnityEngine.Random.Range(0.9f, 1.1f) : gc.baseRechargeTime;
        recoilFactor = randomizeValues ? gc.basePushBack * UnityEngine.Random.Range(0.9f, 1.1f) : gc.basePushBack;
        baseFireRate = randomizeValues ? gc.baseFireRate * UnityEngine.Random.Range(0.9f, 1.1f) : gc.baseFireRate;
        baseBulletForce = randomizeValues ? gc.baseBulletForce * UnityEngine.Random.Range(0.9f, 1.1f) : gc.baseBulletForce;
        baseBulletLivingTime = randomizeValues ? gc.baseBulletDistance * UnityEngine.Random.Range(0.9f, 1.1f) : gc.baseBulletDistance;
        shootInstability = randomizeValues ? gc.baseInaccuracy * UnityEngine.Random.Range(0.9f, 1.1f) : gc.baseInaccuracy;
        splashRadius = randomizeValues ? gc.splashBaseRadius * UnityEngine.Random.Range(0.9f, 1.1f) : gc.splashBaseRadius;
        bulletSize = randomizeValues ? gc.baseBulletSize * UnityEngine.Random.Range(0.9f, 1.1f) : gc.baseBulletSize;

        topSprite = gc.topView;
        sideSprite = gc.sideView;

        howManyBulletsPercentage = gc.baseInaccuracyMaxBullets;

        reloadRate = rechargeRate;
        reloadTimer = 0;

        SetupDebug();
        RestoreBaseParameters();
    }

    public void SetTrail(bool active)
    {
        hasTrail = active;
    }

    protected virtual IEnumerator Shoot()
    {
        isShooting = true;

        backfire.enabled = true;
        GlobalData.Instance.shoots = GlobalData.Instance.shoots + 1;
        currentBullets--;
        PlaySfx(shootSound);
        fireFloorLight.enabled = true;

        InstantiateBullet();

        float pushbackReduce = recoilFactor * player.PushBackPrevention;
        float finalRecoil = recoilFactor + pushbackReduce;
        float pushback = bulletForce * finalRecoil;
        if (pushback > 0)
        {
            player.OnPushback(pushback);
        }
        //SetRumble(0.05f, fireRate);

        if (instabilityMultiplier < 1)
            instabilityMultiplier += 1 / (maxBullets * (howManyBulletsPercentage / 100));
        else
            instabilityMultiplier = 1;

        if (!GetPlayer().isOnTutorial)
        {
            int currentShots = PlayerPrefs.GetInt("SHOTS", 0);
            currentShots++;
            PlayerPrefs.SetInt("SHOTS", currentShots);
        }

        if (canRumble)
            player.input.Rumble(.25f, .25f);

        yield return new WaitForSeconds(0.02f);

        backfire.enabled = false;
        fireFloorLight.enabled = false;

        yield return new WaitForSeconds(0.03f);

        player.input.Rumble(0f, 0f);

        yield return new WaitForSeconds(fireRate - 0.03f);
        if (currentBullets <= 0)
        {
            if(canShoot)
                Reload();
            PlaySfx(reloadSound);
        }
        isShooting = false;
    }

    public void Reload()
    {
        if (isRecharging || currentBullets >= maxBullets) return;

        isRecharging = true;

        // Reset bullet bar
        //currentBullets = 0;

        reloadTimer = 0;
    }

    public void InstantReload()
    {
        currentBullets = maxBullets;
    }

    IEnumerator IndicateReload()
    {
        GetComponentInChildren<SpriteRenderer>().material.SetInt("_Hit", 1);

        yield return new WaitForSeconds(.2f);

        GetComponentInChildren<SpriteRenderer>().material.SetInt("_Hit", 0);
    }

    public void AbortRecharge()
    {
        reloadTimer = 0;
        GetPlayer().ReloadValues(1);

        isRecharging = false;
    }

    protected virtual void InstantiateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
        bullet.GetComponent<Bullet>().Setup(this, GetShootTargetPosition(), bulletOnHitSolidSound);

        if(hasTrail)
            bullet.GetComponentInChildren<ParticleSystem>().Play();
    }

    protected Vector2 GetShootTargetPosition()
    {
        Vector2 targetPosition = firepoint.position + (player.aim.transform.position - transform.position);

        return targetPosition;
    }

    public void UpdateGraphics(GunSpritePerspective perspective, int spriteLayer, int scaleY = 0)
    {
        currentPerspective = perspective;
        SetSpritePerspective(perspective);
        SetSpriteLayer(spriteLayer);
        if (scaleY != 0) SetSpriteScaleY(scaleY);
    }

    public int GetSpriteLayer()
    {
        return gunRenderer.sortingLayerID;
    }

    public int GetScaleY()
    {
        return (int)transform.localScale.y;
    }

    IEnumerator Recharge(float rechargeRate)
    {
        var instruction = new WaitForEndOfFrame();
        while (rechargeRate > 0)
        {
            rechargeRate -= Time.deltaTime;
            yield return instruction;
        }
        if (currentBullets < maxBullets)
        {
            currentBullets++;
        }
        if (currentBullets == maxBullets)
        {
            PlaySfx(reloadSound);
            isRecharging = false;
        }
    }

    protected void SetRumble(float rumbleForce, float rumbleTime)
    {
        if (canRumble)
        {
            FindObjectOfType<RumbleManager>().Rumble(rumbleForce, rumbleTime);
        }
    }

    public void SetSpritePerspective(GunSpritePerspective sP)
    {
        if (sP == GunSpritePerspective.Top)
        {
            gunRenderer.sprite = topSprite;
        }
        else
        {
            gunRenderer.sprite = sideSprite;
        }
    }

    public void SetSpriteLayer(int l)
    {
        if (gunType == GunType.Bazooka)
        {
            //spriteRenderer.sortingOrder = 6;
            gunRenderer.sortingOrder = l;
        }
        else
        {
            gunRenderer.sortingOrder = l;
        }
    }

    public void SetSpriteScaleY(int s)
    {
        transform.localScale = new Vector3(transform.localScale.x, s, transform.localScale.z);
    }

    public void SetCurrentTint(Color32 tint)
    {
        currentTint = tint;
    }

    public void EndSpecialIfActive()
    {
        BroadcastMessage("EndSpecial");
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetRotation(float angle)
    {
        if (!isEquipping)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public float GetAngle()
    {
        return transform.rotation.z;
    }

    public void StoreBaseParameters()
    {
        baseFireRate = fireRate;
        baseBulletForce = bulletForce;
        baseBulletLivingTime = bulletDistance;
    }

    public void RestoreBaseParameters()
    {
        fireRate = baseFireRate;
        bulletForce = baseBulletForce;
        bulletDistance = baseBulletLivingTime;

        SetCurrentTint(bulletTint);
    }

    public void PlayEquipVfx()
    {
        StartCoroutine(EquipAnimation());
    }

    bool isEquipping;
    IEnumerator EquipAnimation()
    {
        isEquipping = true;

        gunRenderer.GetComponent<Animator>().SetBool("isEquipping", true);

        yield return new WaitForSeconds(.2f);

        gunRenderer.GetComponent<Animator>().SetBool("isEquipping", false);

        isEquipping = false;

        player.aim.SetGunsLookToAim();
        player.OnFacingDirectionChange(player.facingDirection);
    }

    public void PlaySfx(AudioClip clip, bool stop = true)
    {
        if (stop) audioSource.Stop();

        audioSource.PlayOneShot(clip);
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public int GetCurrentBullets() { return currentBullets; }
    public int GetMaxBullets() { return maxBullets; }

    void SetupDebug()
    {
        if (DebugHelper.Instance == null || !DebugHelper.Instance.isActivated)
        {
            return;
        }
        ClearDebug();
        isCurrentDebug = true;

        debugObjects.Add(DebugHelper.Instance.SetupImage(sideSprite, true));

        debugObjects.Add(DebugHelper.Instance.SetupText($"{gunConfig.displayName.ToUpper()}<br>" +
            "<br>" +
            $"Bullet Damage: {damage}<br>"  +
            $"Max bullets: {maxBullets}<br>" +
            $"Recharge Rate: {rechargeRate}<br>" +
            $"Recoil factor: {recoilFactor}<br>" +
            $"Fire Rate: {fireRate}<br>" +
            $"Bullet Force: {bulletForce}<br>" +
            $"Bullet Distance: {bulletDistance}<br>" +
            $"Shoot Instability: {shootInstability}<br>" +
            $"Splash radius: {splashRadius}<br>" +
            $"Bullet Size: {bulletSize}"));
    }

    public string GetDebugText()
	{
        return $"{gunConfig.displayName.ToUpper()}<br>" +
             "<br>" +
             $"Bullet Damage: {damage}<br>" +
             $"Max bullets: {maxBullets}<br>" +
             $"Recharge Rate: {rechargeRate}<br>" +
             $"Recoil factor: {recoilFactor}<br>" +
             $"Fire Rate: {fireRate}<br>" +
             $"Bullet Force: {bulletForce}<br>" +
             $"Bullet Distance: {bulletDistance}<br>" +
             $"Shoot Instability: {shootInstability}<br>" +
             $"Splash radius: {splashRadius}<br>" +
             $"Bullet Size: {bulletSize}";

    }

    public bool IsShootHold()
	{
        return isShootHold;
	}

    public void SetShootHold(bool isShootHold)
	{
        if (this.isShootHold == isShootHold) return;

        this.isShootHold = isShootHold;

        //if (currentBullets <= 0 && isShootHold == true)
        //    Reload();
	}

    void ClearDebug()
    {
        if (DebugHelper.Instance == null || !DebugHelper.Instance.isActivated || debugObjects.Count == 0)
        {
            return;
        }
        foreach (var debug in debugObjects)
        {
            Destroy(debug);
        }
        isCurrentDebug = false;
        debugObjects.Clear();
    }
}