using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzeGames.Effects;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Player Params")]
    [HideInInspector] public int playerIndex;
    public string characterName;
    [HideInInspector] public PlayerFacingDirections facingDirection = PlayerFacingDirections.Right;
    // Dash
    public float dashSpeed;
    float currentDashSpeed;
    float currentDashSpeedModifier = 0;
    public float dashTime;
    float currentDashTime;
    float currentDashTimeModifier = 0;
    public float dashCooldown;
    float currentDashCooldown;
    float currentDashCooldownModifier = 0;
    [SerializeField] GameObject dashVfx;
    [SerializeField] GameObject clawDashVfxPrefab;
    public PlayerEntryPanel entryPanel;
    bool isDead = false;
    public bool isInWater = false;
    [SerializeField] bool isExternalForcingMovement; 

    [Header("Health")]
    [HideInInspector] public bool shouldFlash = true;

	#region Attributes
	[Header("Player Attributes")]
    public float moveSpeed = 4f;
    [SerializeField] private float minSpeedMultiplier = 0.06f;
    float currentMoveSpeed = 4f;
    float currentMoveSpeedModifier = 0f;
    float moveSpeedBonus = 0f;

    [HideInInspector] public bool hasLeafBlower = false;
    [HideInInspector] public bool hasDegravitizer = false;

    [SerializeField]
    public float MoveSpeedBonus { get => moveSpeedBonus; set => moveSpeedBonus = value; }

    [SerializeField]
    float damageBonus = 0f;
    public float DamageBonus { get => damageBonus; set => damageBonus = value; }

    [SerializeField]
    // will be reworked
    float gunRechargeBonus = 0f;
    public float GunRechargeBonus { get => gunRechargeBonus; set => gunRechargeBonus = value; }

    [SerializeField]
    float bulletForceBonus = 0f;
    public float BulletForceBonus { get => bulletForceBonus; set => bulletForceBonus = value; }

    [SerializeField]
    float bulletDistanceBonus = 0f;
    public float BulletDistanceBonus { get => bulletDistanceBonus; set => bulletDistanceBonus = value; }

    [SerializeField]
    float bulletSizeBonus = 0f;
    public float BulletSizeBonus { get => bulletSizeBonus; set => bulletSizeBonus = value; }

    [SerializeField]
    float ammoBonus = 0f;
    public float AmmoBonus { get => ammoBonus; set => ammoBonus = value; }

    [SerializeField]
    // 0-1 is less pushback, 1 is no pushback, -1 is double pushback
    float pushBackPrevention = 0f;
    public float PushBackPrevention { get => pushBackPrevention; set => pushBackPrevention = value; }

    [SerializeField]
    float recoilStabilization = 0f;
    public float RecoilStabilization { get => recoilStabilization; set => recoilStabilization = value; }

    [SerializeField]
    // Low is faster/better
    float firerateBonus = 0f;
    public float FirerateBonus { get => firerateBonus; set => firerateBonus = value; }

    [SerializeField]
    int hitBlocker = 0;
    public int HitBlocker { get => hitBlocker; set => hitBlocker = value; }

    [SerializeField]
    bool oneHitKill = false;
    public bool OneHitKill { get => oneHitKill; set => oneHitKill = value; }

    [SerializeField]
    float coinDropRateModifier = 0f;
    public float CoinDropRateModifier { get => coinDropRateModifier; set => coinDropRateModifier = value; }
    [SerializeField]
    float healthDropRateModifier = 0;
    public float HealthDropRateModifier { get => healthDropRateModifier; set => healthDropRateModifier = value; }

    [SerializeField]
    float mementoDropRateModifier = 0f;
    public float MementoDropRateModifier { get => mementoDropRateModifier; set => mementoDropRateModifier = value; }

    [SerializeField]
    float weaponDropRateModifier = 0f;
    public float WeaponDropRateModifier { get => weaponDropRateModifier; set => weaponDropRateModifier = value; }

    [SerializeField]
    float powerUpDropRateModifier = 0f;
    public float PowerUpDropRateModifier { get => powerUpDropRateModifier; set => powerUpDropRateModifier = value; }

    [SerializeField]
    float maxHPBonus = 0f;
    public float MaxHPBonus { get => maxHPBonus; set => maxHPBonus = value; }

    [SerializeField]
    bool projectileFlight = false;
    public bool ProjectileFlight { get => projectileFlight; set => projectileFlight = value; }

    [SerializeField]
    bool projectilePierce = false;
    public bool ProjectilePierce { get => projectilePierce; set => projectilePierce = value; }

    [SerializeField]
    bool projectileInvulnerability = false;
    public bool ProjectileInvulnerability { get => projectileInvulnerability; set => projectileInvulnerability = value; }

    [SerializeField]
    bool contactInvulnerability = false;
    public bool ContactInvulnerability { get => contactInvulnerability; set => contactInvulnerability = value; }

    [SerializeField]
    float dashSpeedModifier = 0f;
    public float DashSpeedModifier { get => dashSpeedModifier; set => dashSpeedModifier = value; }

    [SerializeField]
    float dashDurationModifier = 0f;
    public float DashDurationModifier { get => dashDurationModifier; set => dashDurationModifier = value; }

    [SerializeField]
    // less is better, -1 is no cooldown, +1 is double cooldown
    float dashCooldownModifier = 0f;
    public float DashCooldownModifier { get => dashCooldownModifier; set => dashCooldownModifier = value; }

    [SerializeField]
    bool flight = false;
    public bool Flight { get => flight; set => HandleFlight(value); }

    public bool isPocketFlight = false;
    public bool isPowerUpDash = false;
    public bool damageOnCollision = false;
    public bool healOnKill = false;
    public bool homingProjectiles = false;

    float previousMoveSpeed = 0f;

    [Header("Temperature Settings")]
    [SerializeField] private float temperatureSpeed;
    [SerializeField, Range(0f, 4f)] private float heatingMultiplier = 1.5f;
    [SerializeField] private float minTemperaturePercentage;
    [SerializeField] private Material temperatureVfxBaseMaterial;
    [SerializeField] private SpriteRenderer temperatureVfxRenderer;
    public bool IsHeating { get; set; }
    private bool _changeTemperature;
    private float _temperaturePercentage;
    private float _dashModifier;
    private Material _temperatureVfxMaterial;

    #endregion

    [Header("Player References")]
    public GameObject stepEffect;
    public Transform stepPoint;
    public Transform gunSlot;
    public Collider2D hurtboxCollider;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    private Health health;
    private Rigidbody2D rb;
    [HideInInspector]
    public Vector2 movement;
    private Interactable interactable;
    private PlayerControls controls;
    [HideInInspector]
    public CinemachineTargetGroup.Target camTarget;
    bool isSlipping = false;
    [SerializeField]
    public float slipFactor = 0.7f;
    public Light2D playerLight;

    [Header("Gun References")]
    public List<Gun> startingGunsPrefabs = new List<Gun>();
    public List<Gun> currentGuns = new List<Gun>();
    public Aim aim;
    [HideInInspector] public Loot currentLoot;
    [SerializeField] Transform reloadBar;
    [SerializeField] TMPro.TextMeshProUGUI reloadText;
    [SerializeField] bool isLaserSightOn;
    [SerializeField] LineRenderer laser;
    [SerializeField] LayerMask laserMask;

    [Header("Pocket References")]
    public Transform pocketParent;
    public Pocket currentPocket;
    public List<Pocket> pockets = new List<Pocket>();
    public Transform PetPositionUp;
    public Transform PetPositionDown;
    public Transform PetPositionLeft;
    public Transform PetPositionRight;
    public PocketRadialMenu pocketMenu;

    [Header("Player States")]
    public Room currentRoom;
    private bool dashEnabled = true;
    [HideInInspector] public bool isDashing;
    private bool stunnedByDash;
    private bool isRecoiling;
    private bool _isOnKnockback;
    private bool isInGateActivationArea;
    public bool IsSpeedDecreased => _isSpeedDecreased;

    private bool _isSpeedDecreased;
    private Coroutine _resetMoveSpeed;
    List<CollectibleDisplayInfo> powerUps = new List<CollectibleDisplayInfo>();

    [Header("UI References")]
    public PlayerGui playerGuiPrefab;
    public PlayerGui playerGui;
    public Sprite characterIcon;
    public Sprite characterAlternativeIcon;
    public Sprite cutsceneSprite;
    private GameObject hitFilter;

    bool isFlashing = false;

    [Header("Sound clips")]
    public AudioClip hittedSound;
    public AudioClip dashSound;
    public AudioClip ninjaTrickSound;
    public AudioClip stepSound;
    public PlayersFootsteps footsteps;
    bool isPlayingStep = false;

    int currentGunIndex = 0;

    CanvasGroup blackFade;

    [HideInInspector] public bool inCombat = false;

    [Header("FX")]
    public GameObject armorHitParticlePrefab;
    public AudioClip armorHitClip;

    RigidbodyConstraints2D originalConstraints;
    bool isFreeze = false;
    private bool _isInverted;
    private Coroutine _disableInverted;

    // Infos
    int defeatedEnemies;

    [HideInInspector] public bool isOnTutorial = false;

    private void Awake()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize pocket and gun
        //StartPocket();

        // Setup managers
        GlobalData.Instance.SetCoin(0);
        originalConstraints = rb.constraints;
        currentDashSpeed = dashSpeed;
        currentDashTime = dashTime;
        currentDashCooldown = dashCooldown;
    }

    private void Start()
    {
        GlobalData.Instance.StartRun();

        SetMoveSpeed(moveSpeed);
        Flight = false;
        dashVfx.SetActive(false);

        _changeTemperature = true;
        _temperatureVfxMaterial = new Material(temperatureVfxBaseMaterial);
        temperatureVfxRenderer.sharedMaterial = _temperatureVfxMaterial;
        ResetTemperature(true);

        blackFade = GameObject.FindWithTag("Fade").GetComponent<CanvasGroup>();

        blackFade.alpha = 1;

        InitializeGuns();
        aim.gameObject.SetActive(true);

        // GUI
        playerGui = Instantiate(playerGuiPrefab, FindObjectOfType<PlayerManager>().GetPlayerGuiParent());
        playerGui.Setup(this);

        SetCamTarget(FindObjectOfType<CameraManager>().AddTarget(transform, 1.75f, 1f));

        reloadBar.parent.gameObject.SetActive(false);

        if (ScreenManager.currentScreen == Screens.Lobby)
            FindObjectOfType<LobbyController>().SpawnPockets(currentPocket);

        bool hasAudioListener = GameObject.FindObjectOfType<AudioListener>();

        if (!hasAudioListener && ScreenManager.currentScreen != Screens.Lobby)
            gameObject.AddComponent<AudioListener>();
        else if (ScreenManager.currentScreen == Screens.Lobby)
        {
            List<Player> homies = GameplayManager.Instance.GetPlayers(false);
            if (homies.Contains(this)) homies.Remove(this);

            bool someoneHas = false;
            foreach (var item in homies)
            {
                if (item.GetComponent<AudioListener>()) someoneHas = true;
                else someoneHas = false;
            }
            
            if(!someoneHas)
                gameObject.AddComponent<AudioListener>();
        }
    }

    public void StopMovement()
	{
        rb.velocity = Vector2.zero;
	}

    public Pocket StartPocket(Pocket pocketPrefab, int level)
	{
        currentPocket = Instantiate(pocketPrefab, pocketParent).GetComponent<Pocket>();
        currentPocket.Start();
        currentPocket.level = level;
        if (level > 0)
        {
            currentPocket.SetupPocket(this);
            currentPocket.GetSpecial().Setup(this);
        }
        else
        {
            currentPocket.pocketType = PetType.Egg;
            currentPocket.SetupPocket(this);
        }
        pockets.Add(currentPocket);

        print("<color=cyan>Pocket: " + currentPocket.pocketName + "</color> |<color=yellow> Level: " + currentPocket.level + "</color>");

        return currentPocket;
    }

    public void AddPocket(Pocket pocket)
	{
        pocket.SetupPocket(GetComponent<Player>());
        pockets.Add(pocket);
        pocket.pocketInventoryIndex = pockets.Count;

        currentPocket = pocket;
    }

    public void RemovePocket(Pocket pocket)
    {
        pocket.UnSetup();
        pockets.Remove(pocket);
        pocket.pocketInventoryIndex = 0;
    }

    public void SetCurrentPocket(Pocket pocket)
    {
        var playerHasPocket = false;
        Pocket p = null;

        foreach (Pocket ownedPocket in pockets)
        {
            if (ownedPocket == pocket)
            {
                playerHasPocket = true;
                p = ownedPocket;
            }
            else
                ownedPocket.ActivePocket(false);
        }

        if (playerHasPocket)
        {
            currentPocket = p;
            currentPocket.ActivePocket(true);
        }
    }

    public void SetCurrentPocket(int index)
    {
        // Disable all pockets
        foreach (Pocket pocket in pockets)
        {
            pocket.ActivePocket(false);
        }

        // Active index pocket
        try
        {
            currentPocket = pockets[index];
        }
        catch { }

        // If have current pocket, active it
        if (currentPocket)
        {
            currentPocket.ActivePocket(true);
        }
    }

    public void SwitchToNextPocket()
    {
        if (!GetCurrentPocket().GetSpecial().IsActivated())
        {
            SetCurrentPocket(GetNextAlivePocketIndex());
            OnFacingDirectionChange(facingDirection);
        }
    }

    void LaserSight(bool hasLaser)
    {
        if (hasLaser)
        {
            RaycastHit2D hit = Physics2D.Raycast(GetCurrentGun().firepoint.transform.position, GetCurrentGun().transform.right, 1000, laserMask);
            laser.SetPosition(0, GetCurrentGun().firepoint.transform.position);
            laser.SetPosition(1, hit.point);
        }
        else
        {
            laser.SetPosition(0, GetCurrentGun().firepoint.position);
            laser.SetPosition(1, GetCurrentGun().firepoint.position);
        }

        laser.enabled = hasLaser;
    }

    public void SetPlayerIndex(int index)
	{
        playerIndex = index;
	}

    public int GetNextAlivePocketIndex()
	{
        // Get enable pocket index
        int currentIndex = GetPocketIndex(GetCurrentPocket());

        for (int i = 0; i < pockets.Count; i++)
        {
            currentIndex++;
            if (currentIndex >= pockets.Count) currentIndex = 0;

            Pocket pocket = pockets[currentIndex];
            if (pocket.GetHealth().GetCurrentHealth() > 0 || pocket.pocketType == PetType.Egg)
            {
                if (currentIndex != GetPocketIndex(GetCurrentPocket()))
                {
                    return currentIndex;
                }
            }
        }

        return currentIndex;
    }

    public Pocket GetNextPocket(Pocket pocket)
	{
        if (GetPocketIndex(pocket) + 1 >= pockets.Count) return pockets[0];
        else return pockets[GetPocketIndex(pocket) + 1];
	}

    public Pocket GetPreviousPocket(Pocket pocket)
	{
        if (GetPocketIndex(pocket) - 1 < 0) return pockets[pockets.Count - 1];
        else return pockets[GetPocketIndex(pocket) - 1];
	}

    public int GetPocketIndex(Pocket pocket)
	{
        for (int i = 0; i < pockets.Count; i++)
		{
            if (pocket == pockets[i])
			{
                return i;
			}
		}

        return 0;
	}

    Vector2 GetScreenFormattedMovementVector(Vector2 move)
    {
        Vector2 movementVector = move;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        float mod = 50;

        if ((screenPosition.x < mod && movementVector.x < 0) || (screenPosition.x > Screen.width - mod && movementVector.x > 0)) movementVector.x = 0;
        if ((screenPosition.y < mod && movementVector.y < 0) || (screenPosition.y > Screen.height - mod && movementVector.y > 0)) movementVector.y = 0;

        return movementVector;
    }

    Vector2 GetCameraModifierVector()
	{
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 modVector = Vector2.one;
        float mod = 50;

        if ((screenPosition.x < mod && movement.x < 0) || (screenPosition.x > Screen.width - mod && movement.x > 0)) modVector.x = 0;
        if ((screenPosition.y < mod && movement.y < 0) || (screenPosition.y > Screen.height - mod && movement.y > 0)) modVector.y = 0;

        return modVector;
    }

    Vector2 GetCameraModifierVectorRb()
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 modVector = Vector2.one;
        float mod = 50;

        if ((screenPosition.x < mod && rb.velocity.x < 0) || (screenPosition.x > Screen.width - mod && rb.velocity.x > 0)) modVector.x = 0;
        if ((screenPosition.y < mod && rb.velocity.y < 0) || (screenPosition.y > Screen.height - mod && rb.velocity.y > 0)) modVector.y = 0;

        return modVector;
    }

    public Material GetMaterial()
	{
        return spriteRenderer.material;
	}

    public void SetMaterial(Material material)
	{
        spriteRenderer.material = material;
	}

    public void OnPocketDies()
	{
        // Get enable pocket index
        int enableIndex = 0;
        for (int i = 0; i < pockets.Count; i++)
		{
            Pocket pocket = pockets[i];
            if (pocket.GetHealth().GetCurrentHealth() > 0 || pocket.pocketType == PetType.Egg)
			{
                enableIndex = i;
                break;
			}
		}

        SetCurrentPocket(enableIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.Instance.IsGamePaused())
        {
            return;
        }

        if (isDead) return;

        if (health.isInvulnerable && !isFlashing && shouldFlash)
        {
            StartCoroutine(FlashPlayer());
        }

        //HandleDangerSFX();

        LaserSight(isLaserSightOn);

        if (blackFade)
        {
            if (blackFade.alpha > 0)
            {
                blackFade.alpha -= Time.unscaledDeltaTime;
            }
            else
            {
                blackFade = null;
            }
        }

        if (currentRoom != null)
        {
            if (currentRoom.currentState == RoomEventState.Running)
                inCombat = true;
            else
                inCombat = false;
        }

        ControlReloadText(GetCurrentGun().currentBullets <= Mathf.RoundToInt(GetCurrentGun().maxBullets * 0.10f));

        if (GameplayManager.Instance.currentDungeonType == DungeonType.Glacier)
        {
            HandleFreezing();
        }

        if (health.GetCurrentPercentage() <= .05f)
            playerGui.SetBlink(true);
        else
            playerGui.SetBlink(false);
    }

    private void HandleDangerSFX()
    {
        if (health.GetCurrentPercentage() < 0.3 && ThemeMusicManager.Instance.IsDangerMuted())
        {
            ThemeMusicManager.Instance.UnmuteDanger();
        } else if (health.GetCurrentPercentage() > 0.3 && !ThemeMusicManager.Instance.IsDangerMuted())
        {
            ThemeMusicManager.Instance.MuteDanger();
        }
    }

    private void FixedUpdate()
    {
        if (isFreeze)
        {
            return;
        }

        // if (isSlipping)
        // {
        //     HandleSlipping();
        // }

        if (currentMoveSpeedModifier != MoveSpeedBonus)
        {
            HandleMoveSpeedChange(MoveSpeedBonus);
        }
        if (currentDashSpeedModifier != DashSpeedModifier)
        {
            HandleDashSpeedChange(DashSpeedModifier);
        }
        if (currentDashCooldownModifier != DashCooldownModifier)
        {
            HandleDashCooldownChange(DashCooldownModifier);
        }
        if (currentDashTimeModifier != DashDurationModifier)
        {
            HandleDashDurationChange(DashDurationModifier);
        }

        if (isExternalForcingMovement)
            return;

        if (!isDashing)
        {
            float speedMod = 1;
            if (stunnedByDash) speedMod = 0;
            var multipler = Mathf.Max(minSpeedMultiplier, speedMod * (currentMoveSpeed) * _temperaturePercentage * Time.fixedDeltaTime);
            // Debug.Log(multipler);
            var movementDirection = _isInverted ? movement * -1f : movement;
            Vector2 finalMoveSpeed = movementDirection * multipler;
            //Debug.Log($"Current Speed {currentMoveSpeed}, Final MS {finalMoveSpeed}");
            if(!stunnedByDash)
                rb.MovePosition(rb.position + finalMoveSpeed * GetCameraModifierVector());
		}
		else
		{
            rb.velocity = dashDirection.normalized * currentDashSpeed;
        }

        if (isRecoiling || _isOnKnockback)
		{
            rb.velocity = GetScreenFormattedMovementVector(rb.velocity);
		}

    }

    private void HandleSlipping()
    {
        // if (movement == Vector2.zero)
        // {
        //     isSlipping = false;
        // }
        // float delta = slipFactor * Time.deltaTime;
        // movement = Vector2.MoveTowards(movement, Vector2.zero, delta);
    }

    private void HandleMoveSpeedChange(float newModifier)
    {
        float modifier = Mathf.Abs(newModifier);
        float difference = moveSpeed * modifier;
        if (newModifier > 0)
        {
            currentMoveSpeed = moveSpeed + difference;
        }
        else
        {
            currentMoveSpeed = moveSpeed - difference;
        }
        currentMoveSpeedModifier = newModifier;
    }

    private void HandleDashSpeedChange(float newModifier)
    {
        float modifier = Mathf.Abs(newModifier);
        float difference = dashSpeed * modifier;
        if (newModifier > 0)
        {
            currentDashSpeed = dashSpeed + difference;
        }
        else
        {
            currentDashSpeed = dashSpeed - difference;
        }
        currentDashSpeedModifier = newModifier;
    }

    private void HandleDashCooldownChange(float newModifier)
    {
        float modifier = Mathf.Abs(newModifier);
        float difference = dashCooldown * modifier;
        if (newModifier > 0)
        {
            currentDashCooldown = dashCooldown + difference;
        }
        else
        {
            currentDashCooldown = dashCooldown - difference;
        }
        currentDashCooldownModifier = newModifier;
    }

    private void HandleDashDurationChange(float newModifier)
    {
        float modifier = Mathf.Abs(newModifier);
        float difference = dashTime * modifier;
        if (newModifier > 0)
        {
            currentDashTime = dashTime + difference;
        }
        else
        {
            currentDashTime = dashTime - difference;
        }
        currentDashTimeModifier = newModifier;
    }

    private void LateUpdate()
    {
        if (isExternalForcingMovement)
		{
            animator.SetBool("IsWalking", false);
        }
		animator.SetBool("IsWalking", movement != Vector2.zero/* && !isSlipping*/);
	}

    #region Input System

    public void HandleWalkInput(InputAction.CallbackContext ctx)
    {
        // if (isSlipping)
        // {
        //     isSlipping = false;
        // }
        movement = ctx.ReadValue<Vector2>();
    }

    public void HandleResetWalk(InputAction.CallbackContext ctx)
    {
        // if (GameplayManager.Instance.currentDungeonType == DungeonType.Glacier)
        // {
        //     isSlipping = true;
        // } else
        // {
            movement = Vector2.zero;
        // }
    }

    public void HandleDashInput(InputAction.CallbackContext ctx)
    {
        if (isExternalForcingMovement) return;

        if (dashEnabled && !isDashing)
        {
            if (movement == Vector2.zero)
                return;

            StartCoroutine(Dash());
        }
    }

    public void HandleChangeWeaponInput(InputAction.CallbackContext ctx)
    {
        int next = GetNextGunIndex(ctx.ReadValue<float>());
        if (next == currentGunIndex) return;
        
        SetGun(next);
        OnFacingDirectionChange(facingDirection);

        DeactivateReloadBar();
    }

    public void HandleInteract(InputAction.CallbackContext ctx)
    {
        if (currentLoot)
        {
            currentLoot.GetLoot(this);
        }
        if (interactable)
        {
            interactable.OnInteract(this);
        }
    }

    #endregion

    public bool IsInGateActivationArea() { return isInGateActivationArea; }
    public void SetIsInGateActivationArea(bool isInGateActivationArea) { this.isInGateActivationArea = isInGateActivationArea; }

    private void OnTriggerStay2D(Collider2D collision)
    {
        bool isTouchedByEnemy = collision.CompareTag("Enemy");
        bool isTouchedByProjectile = collision.CompareTag("EnemyProjectile");
        bool isAggressor = isTouchedByEnemy || isTouchedByProjectile || collision.CompareTag("Shockwave") || collision.GetComponent<Laser>();
        
        if (isAggressor && !health.isInvulnerable && !isDashing)
        {
            HandleDamage(collision, isTouchedByProjectile, isTouchedByEnemy);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isTouchedByEnemy = collision.CompareTag("Enemy");
        bool isAggressor = isTouchedByEnemy;

        if (isAggressor && isDashing && damageOnCollision)
        {
            Health health = collision.GetComponent<Health>();

            if (health != null && !health.isInvulnerable)
            {
                health.Decrease(10, this, null, dashTime);

                GameObject clawDashVfx = Instantiate(clawDashVfxPrefab, collision.transform.position, Quaternion.identity);
                Toolkit2D.RotateAt(clawDashVfx.transform, new Vector2(transform.position.x, transform.position.y) + dashDirection);

                Destroy(clawDashVfx, 0.8f);
            }
        }
    }

    private static float GetDamage(Collider2D collision)
    {
        if (collision.TryGetComponent<Projectile>(out Projectile projectile))
            return collision.GetComponent<Projectile>().damage;
        else if (collision.TryGetComponent<Laser>(out Laser laser))
            return laser.GetDamage();
        else if (collision.TryGetComponent<ShockwaveController>(out ShockwaveController shockwave))
            return shockwave.damage;
        else if (collision.TryGetComponent<Enemy>(out Enemy enemy))
            return enemy.damage;

        return collision.GetComponent<DamagePlayerOnContact>().Damage;
    }

    public void HandleDamage(Collider2D collision, bool isTouchedByProjectile, bool isTouchedByEnemy)
    {
        if (isTouchedByProjectile && projectileInvulnerability)
        {
            return;
        }
        if (isTouchedByEnemy && contactInvulnerability)
        {
            return;
        }

        if (hitBlocker > 0)
        {
            health.Decrease(0, null, null, 2);

            GameObject armorParticle = Instantiate(armorHitParticlePrefab, transform.position, Quaternion.identity);
            audioSource.PlayOneShot(armorHitClip);

            hitBlocker--;

            GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            return;
        }
        if (oneHitKill)
        {
            health.Decrease(health.GetCurrentHealth());
            return;
        }

        var damage = GetDamage(collision);
        Projectile projectile = collision.GetComponent<Projectile>();
        if (projectile != null)
        {
            Vector2 knockbackForce = Vector2.zero;

            var rubberProjectile = projectile as RubberProjectile;
            if (rubberProjectile != null)
            {
                knockbackForce = rubberProjectile.GetKnockbackForce(transform, GetCameraModifierVector());
                ApplyKnockBack(knockbackForce);
            }

            var rubberBouncyProjectile = projectile as RubberBouncyProjectile;
            if (rubberBouncyProjectile != null)
            {
                knockbackForce = rubberBouncyProjectile.GetKnockbackForce(transform, GetCameraModifierVector());
                ApplyKnockBack(knockbackForce);
            }

            var invertProjectile = projectile as InvertProjectile;
            if (invertProjectile != null)
                SetInverted(true);

            if (projectile.dashStun && !stunnedByDash) StartDashStun(projectile.hitPlayerParticle);
            projectile.OnTrigger();
        }

            var lavaProjectile = projectile as LavaProjectile;
            if (collision.transform.GetComponentInChildren<SwarmController>() || lavaProjectile != null)
                health.Decrease(damage, null, null, .25f);

            else if (collision.transform.GetComponent<ShockwaveController>())
            {
                if (collision.transform.GetComponent<ShockwaveController>().canDamage)
                    health.Decrease(collision.transform.GetComponent<ShockwaveController>().damage, null, null, 2);
            }
            else
            {
                health.Decrease(damage, null, null, 2);
            }
    }

    public void SetCurrentLoot(Loot loot)
    {
        currentLoot = loot;
    }

    Vector2 dashDirection;

    private IEnumerator Dash()
	{
        EndDashStun();


        if (!isPowerUpDash)
        {
            audioSource.PlayOneShot(dashSound);
            dashVfx.SetActive(true);
        }
        else
        {
            audioSource.PlayOneShot(ninjaTrickSound);
            foreach (var item in currentGuns)
            {
                item.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            }
        }
		dashEnabled = false;
        isDashing = true;
        animator.SetBool("IsDashing", true);

        // Get dash direction
        dashDirection = _isInverted ? movement * -1f : movement;

        // Start dash
        rb.velocity = dashDirection.normalized * currentDashSpeed * _dashModifier;

        if (isPowerUpDash) DashFlight(true);

        yield return new WaitForSeconds(currentDashTime * _dashModifier);

        // End dash
        isDashing = false;
        dashVfx.SetActive(false);
        if (isPowerUpDash)
        {
            foreach (var item in currentGuns)
            {
                item.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
        animator.SetBool("IsDashing", false);

        if (isPowerUpDash) DashFlight(false);

        // Dash cooldown
        yield return new WaitForSeconds(currentDashCooldown - currentDashTime);
        dashEnabled = true;
    }

    public void SetFacingDirection(PlayerFacingDirections fD)
    {
        if (facingDirection != fD)
        {
            facingDirection = fD;
            OnFacingDirectionChange(fD);
        }
    }

    private void HandleFreezing()
    {
        if (!_changeTemperature)
            return;

        if (IsHeating)
        {
            _temperaturePercentage = Mathf.MoveTowards(_temperaturePercentage, 1f, temperatureSpeed * heatingMultiplier * Time.deltaTime);
            _dashModifier =_temperaturePercentage;
            SetTemperatureVfxMaterialValues();
            return;
        }

        var speedMultiplier = (currentMoveSpeed) * _temperaturePercentage * Time.fixedDeltaTime;
        if (speedMultiplier < minSpeedMultiplier)
            return;

        if (!IsHeating && _temperaturePercentage > minTemperaturePercentage)
        {
            _temperaturePercentage = Mathf.MoveTowards(_temperaturePercentage, minTemperaturePercentage, temperatureSpeed * Time.deltaTime);
            _dashModifier =_temperaturePercentage;
        }

        SetTemperatureVfxMaterialValues();
    }

    public void ResetTemperature(bool stopTemperatureChange = false)
    {
        _temperaturePercentage = 1f;
        _dashModifier = _temperaturePercentage;
        SetTemperatureVfxMaterialValues();
        _temperatureVfxMaterial.SetFloat("_FadeAmount", 1f);

        _changeTemperature = !stopTemperatureChange;
    }

    public void ChangeTemperature(float value)
    {
        var newPercentage = _temperaturePercentage;
        newPercentage += value;

        var speedMultiplier = (currentMoveSpeed) * newPercentage * Time.fixedDeltaTime;
        if (speedMultiplier < minSpeedMultiplier)
        {
            var division = currentMoveSpeed * Time.fixedDeltaTime;
            var clampedPercentage = minSpeedMultiplier / division;
            newPercentage = clampedPercentage;
        }

        _temperaturePercentage = newPercentage;
        _temperaturePercentage = Mathf.Clamp(_temperaturePercentage, minTemperaturePercentage, 1f);
        _dashModifier =_temperaturePercentage;

        SetTemperatureVfxMaterialValues();
    }

    private void SetTemperatureVfxMaterialValues()
    {
        var totalMultiplier = (currentMoveSpeed) * Time.fixedDeltaTime;
        var currentMultiplier = totalMultiplier * _temperaturePercentage;
        var minMultiplier = Mathf.Max(minSpeedMultiplier, totalMultiplier * minTemperaturePercentage);

        var tempPercentage = Mathf.InverseLerp(minMultiplier, totalMultiplier, currentMultiplier);
        var fadeAmount = Mathf.Lerp(0.5f, 0.8f, tempPercentage);
        _temperatureVfxMaterial.SetFloat("_FadeAmount", fadeAmount);
    }

    public void ApplyKnockBack(Vector2 force)
    {
        if (force == Vector2.zero)
            return;

        if (stunnedByDash || GetHealth().isInvulnerable)
            return;
        
        SetMoveSpeed(0.5f);
        rb.AddForce(force, ForceMode2D.Force);
        _isOnKnockback = true;
        Invoke("EndKnockback", 0.1f);
    }

    private void EndKnockback()
    {
        _isOnKnockback = false;
        SetMoveSpeed(moveSpeed);
    }

    public void OnPushback(float force)
    {
        if (stunnedByDash) return;

        previousMoveSpeed = MoveSpeedBonus;
        MoveSpeedBonus = -.95f;
        Vector2 pushbackDirection = -((GetCurrentGun().firepoint.position - transform.position) * GetCameraModifierVector());
        rb.velocity = Vector3.zero;
        rb.AddForce(pushbackDirection.normalized * force, ForceMode2D.Force);
        MoveSpeedBonus = previousMoveSpeed;
    }

    public void OnFacingDirectionChange(PlayerFacingDirections fD)
    {
        if (fD == PlayerFacingDirections.Right)
        {
            if(currentPocket)
                currentPocket.SetTargetPosition(PetPositionLeft);
            RotateGuns(GunSpritePerspective.Side, 6, 1);
            animator.SetFloat("FacingDirection", (float)AnimationFacing.SideRight);
        }
        else if (fD == PlayerFacingDirections.Down)
        {
            if (currentPocket)
                currentPocket.SetTargetPosition(PetPositionUp);
            RotateGuns(GunSpritePerspective.Top, 6);
			animator.SetFloat("FacingDirection", (float)AnimationFacing.Front);
        }
        else if (fD == PlayerFacingDirections.Left)
        {
            if (currentPocket)
                currentPocket.SetTargetPosition(PetPositionRight);
            RotateGuns(GunSpritePerspective.Side, 5, -1);
            animator.SetFloat("FacingDirection", (float)AnimationFacing.Side);
        }
        else
        {
            if (currentPocket)
                currentPocket.SetTargetPosition(PetPositionDown);
            RotateGuns(GunSpritePerspective.Top, 4);
			animator.SetFloat("FacingDirection", (float)AnimationFacing.Back);
        }
    }

    public void RotateGuns(GunSpritePerspective perspective, int spriteLayer, int scaleY = 0)
	{
        foreach(Gun gun in currentGuns)
		{
            //Gun gun = gunObject.GetComponent<Gun>();
            gun.UpdateGraphics(perspective, spriteLayer, scaleY);
        }
	}

    private GameObject dashStunVfx;
    public void StartDashStun(GameObject vfx)
	{
        GetComponent<WindObject>().enabled = false;
        stunnedByDash = true;
        StopMovement();
        if (vfx)
		{
            dashStunVfx = Instantiate(vfx, transform.position, Quaternion.identity);
		}
	}

    public void EndDashStun()
	{
        if(!hasLeafBlower)
            GetComponent<WindObject>().enabled = true;

        stunnedByDash = false;
        if (dashStunVfx) Destroy(dashStunVfx);
    }

    public void onDamage(float valueDecreased)
    {
        if (valueDecreased > 0)
        {
            StartCoroutine(DamageFeedback());
            GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }

    }

    public void onHealthEnd()
    {
        isDead = true;

        //FindObjectOfType<CameraManager>().DisableTarget(camTarget);
        //gameObject.SetActive(false);
        animator.SetTrigger("Die");
        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
        dashDirection = Vector2.zero;
        GetHitFilterManager().ActiveCharacterHitFilter(false);

        hurtboxCollider.transform.parent.gameObject.SetActive(false);

        gunSlot.gameObject.SetActive(false);

        int alivePlayers = 0;
        Player lastActivePlayer = null;
        foreach (Player p in GameplayManager.Instance.GetPlayers(false))
		{
            if (!p.GetIsDead())
			{
                alivePlayers++;
                lastActivePlayer = p;
            }
        }

        if (alivePlayers == 0)
        {
            if (isOnTutorial)
            {
                Invoke("Revive", 3);
            }
            else
            {
                GlobalData.Instance.EndRun("Game Over");
                GlobalData.Instance.SetPause(false);
            }
        }
		else
		{
            FindObjectOfType<CameraManager>().DisableTarget(camTarget);

            if (lastActivePlayer)
			{
				//CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();
				//if (!cam.m_Follow.gameObject.activeSelf)
				//{
                //    cam.m_Follow = lastActivePlayer.transform;
                //}
			}

		}

        entryPanel.GetUnlock().SaveData();

        input.Rumble(0, 0);
        //Destroy(gameObject);
    }

    public void Revive()
    {
        gunSlot.gameObject.SetActive(true);
        SetCamTarget(FindObjectOfType<CameraManager>().AddTarget(transform, 1.75f, 1f));
        animator.Play("Idle");
        health.SetHealth(health.GetMaxHealth() / 2);
        hurtboxCollider.transform.parent.gameObject.SetActive(true);

        if(isOnTutorial) FindObjectOfType<EnterTutorial>().ExitTutorial();

        isDead = false;
    }

    public void Cheer()
    {
        animator.SetTrigger("Pick Up");

        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
        dashDirection = Vector2.zero;

        stunnedByDash = true;
        gunSlot.gameObject.SetActive(false);
    }

    public void StopCheer()
    {
        stunnedByDash = false;
        gunSlot.gameObject.SetActive(true);

        print("Stop CHEERING");
    }

    void OnDestroy()
    {
        

        Destroy(GameObject.FindGameObjectWithTag("PocketMenu"));
    }

    IEnumerator FlashPlayer()
    {
        isFlashing = true;

        SetSpriteAlpha(.2f);
        yield return new WaitForSeconds(0.2f);
        SetSpriteAlpha(1);
        yield return new WaitForSeconds(0.2f);
        isFlashing = false;
    }

    public void SetSpriteAlpha(float value)
    {
        //Color currentColor = spriteRenderer.color;
        //currentColor.a = value;
        //spriteRenderer.color = currentColor;

        spriteRenderer.material.SetFloat("_Alpha", value);
    }

    public void OnStepHandler() 
    {
        GlobalData.Instance.stepsMade += 1;
        GameObject step = Instantiate(stepEffect, stepPoint.position, Quaternion.identity, transform.parent);
        if (isPlayingStep)
        {
            return;
        }
        isPlayingStep = true;
        float volume = Random.Range(.4f, 1f);
        if (!isExternalForcingMovement) 
            audioSource.PlayOneShot(currentFootsteps()[Random.Range(0, currentFootsteps().Count)], (volume / 20));
        isPlayingStep = false;

        step.GetComponent<Animator>().SetBool("InWater", isInWater);
    }

    List<AudioClip> currentFootsteps()
    {
        List<AudioClip> footsteps = new List<AudioClip>();

        switch (GameplayManager.Instance.currentDungeonType)
        {  
            case DungeonType.Lab:
                footsteps = this.footsteps.labFootsteps;
                break;
            case DungeonType.Forest:
                footsteps = this.footsteps.forestFootsteps;
                break;
            case DungeonType.Cave:
                footsteps = this.footsteps.caveFootsteps;
                break;
            case DungeonType.Glacier:
                footsteps = this.footsteps.glacierFootsteps;
                break;
            case DungeonType.Volcano:
                footsteps = this.footsteps.labFootsteps;
                break;
            case DungeonType.Swamp:
                footsteps = this.footsteps.labFootsteps;
                break;
            case DungeonType.SinisterLab:
                footsteps = this.footsteps.sinisterLabFootsteps;
                break;
            default:
                footsteps = this.footsteps.labFootsteps;
                break;
        }

        return footsteps;
    }

    public void SetGun(int gunIndex)
    {
        Gun oldGun = null;
        for (int i = 0; i < currentGuns.Count; i++)
        {
            // Current gun
            if (i == gunIndex)
            {
                oldGun = currentGuns[i];
                oldGun.canShoot = true;
                oldGun.SetShootHold(false);
                oldGun.transform.localScale = new Vector3(1, 1, 1);
                oldGun.transform.eulerAngles = currentGuns[currentGunIndex].transform.eulerAngles;
            }
            // Others guns
            else
            {
                currentGuns[i].GetComponent<Gun>().canShoot = false;
                currentGuns[i].transform.localScale = Vector3.zero;
                currentGuns[i].GetComponent<Gun>().AbortRecharge();
            }
        }

        // Update current gun
        try
        { 
            currentGunIndex = gunIndex;
            Gun currentGun = currentGuns[currentGunIndex].GetComponent<Gun>();
            currentGun.PlaySfx(currentGun.equipSound);
            currentGun.PlayEquipVfx();
            currentGun.UpdateGraphics(oldGun.currentPerspective, oldGun.GetSpriteLayer(), oldGun.GetScaleY());
            currentGun.SetPlayer(this);
            if (currentGun.currentBullets <= 0)
                currentGun.Reload();

            aim.SetGunsLookToAim();
        }
		catch { }
        //OnFacingDirectionChange(facingDirection);
    }

    public int GetNextGunIndex(int currentIndex)
    {
        int nextGun = currentIndex + 1;
        if (nextGun > currentGuns.Count - 1)
        {
            nextGun = 0;
        }

        return nextGun;
    }

    public int GetPreviousGunIndex(int currentIndex)
	{
        int prevGun = currentIndex - 1;
        if (prevGun < 0)
        {
            prevGun = currentGuns.Count - 1;
        }

        return prevGun;
    }

    int GetNextGunIndex(float ctx)
    {
        int index = 0;
        if (ctx > 0) index = currentGunIndex + 1;
        else if (ctx < 0) index = currentGunIndex - 1;

        if (index > currentGuns.Count - 1)
            index = 0;
        else if (index < 0)
            index = currentGuns.Count - 1;

        return index;
    }

    HitFilterManager GetHitFilterManager()
	{
        return FindObjectOfType<HitFilterManager>();
    }

    public void PlaySfx(AudioClip clip, bool stop = true)
    {
        if (stop) audioSource.Stop();

        audioSource.PlayOneShot(clip);
    }

    public void SetInvulnerability(bool value)
    {
        health.SetInvulnerability(value);
    }

    [HideInInspector] public PlayerInputController input;
    public void SetInputController(PlayerInputController inputController)
	{
        this.input = inputController;
	}

    public PlayerInputController GetInputController()
	{
        return input;
	}

    IEnumerator DamageFeedback()
    {
        audioSource.PlayOneShot(hittedSound);
        GetHitFilterManager().ActiveCharacterHitFilter(.3f);

        input.Rumble(.3f, .3f);

        spriteRenderer.material.SetInt("_HitEffectBlend", 1);
        yield return new WaitForSeconds(0.3f);

        input.Rumble(0, 0);

        spriteRenderer.material.SetInt("_HitEffectBlend", 0);
    }

    public Gun GetCurrentGun()
    {
        if (currentGuns.Count > 0) return currentGuns[currentGunIndex].GetComponent<Gun>();
        else return null;
    }

    void InitializeGuns()
    {
        Gun gunInstance = null;

        if (ScreenManager.currentScreen != Screens.Tutorial)
        {
            if (characterName.Contains("Zoy"))
                gunInstance = Instantiate(FindObjectOfType<StartingGuns>().zoyGun, gunSlot);
            else if (characterName.Contains("Unk"))
                gunInstance = Instantiate(FindObjectOfType<StartingGuns>().unkGun, gunSlot);
            else if (characterName.Contains("Rosa"))
                gunInstance = Instantiate(FindObjectOfType<StartingGuns>().rosaGun, gunSlot);
            else if (characterName.Contains("Yellowish"))
                gunInstance = Instantiate(FindObjectOfType<StartingGuns>().irwinGun, gunSlot);
        }
        else
        {
            gunInstance = Instantiate(FindObjectOfType<StartingGuns>().irwinGun, gunSlot);
        }

        currentGuns.Add(gunInstance);
        gunInstance.SetPlayer(this);

        SetGun(0);
        OnFacingDirectionChange(facingDirection);
    }

    public int GetGunIndexByType(GunType gunType)
	{
        for (int i = 0; i < currentGuns.Count; i++)
		{
            if (currentGuns[i])
			{
                if (currentGuns[i].GetComponent<Gun>().gunType == gunType)
				{
                    return i;
				}
			}
		}

        return currentGuns.Count;
	}

    void SetMoveSpeed(float ms)
    {
        currentMoveSpeed = ms;
    }

    public void SetPosition(Vector3 position)
    {
        // Debug.Log("SetPosition");
        // isSlipping = false;
        movement = Vector2.zero;
        transform.position = position;
    }

	public bool IsDashing()
	{
        return isDashing;
	}

    public List<Gun> GetAllGuns()
    {
        return currentGuns;
    }

    public Vector2Int GetIntPosition()
	{
        return new Vector2Int((int)transform.position.x, (int)transform.position.y);
	}

    public Pocket GetCurrentPocket()
    {
        return currentPocket;
    }

    public List<Pocket> GetSecondaryPockets()
    {
        List<Pocket> secondarys = new List<Pocket>();
        foreach (var pocket in pockets)
        {
            if (!pocket.IsActive())
            {
                secondarys.Add(pocket);
            }
        }
        return secondarys;
    }

    public void SetAllSpriteRenderers(bool value = true)
    {
        SpriteRenderer[] All = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprRender in All)
        {
            sprRender.enabled = value;
        }
        GameObject.FindGameObjectWithTag("PlayerCollider").GetComponent<Collider2D>().enabled = value;
        if (value == false)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            isFreeze = true;
        } else
        {
            rb.constraints = originalConstraints;
            isFreeze = false;
        }
    }

    public Health GetHealth()
	{
        return health;
	}

    private void UpdateNPCsPrice(int balance)
    {
        var npcsList = FindObjectsOfType<NPC>();
        foreach (var npc in npcsList)
        {
            npc.CheckItemsPrice(balance);
        }
    }

    public void DeactivateReloadBar()
    {
        reloadBar.parent.gameObject.SetActive(false);
    }

    public void ReloadValues(float value)
    {
        reloadBar.parent.gameObject.SetActive(true);

        reloadBar.localScale = new Vector3(Mathf.Lerp(0, 1, value), 1, 1);

        if(value >= 1)
            reloadBar.parent.gameObject.SetActive(false);
    }

    public void ControlReloadText(bool active)
    {
        reloadText.gameObject.SetActive(active);
    }

    public Aim GetAim()
	{
        return aim;
	}

    public PocketRadialMenu GetPocketMenu()
	{
        return pocketMenu;
	}

    public void OnPauseInput(InputAction.CallbackContext ctx)
    {
        //if (PauseManager.Instance.ReturnPauseDelay() > 0) return;
        if (GetInputController().GetPlayerEntryPanel().selectedSpot != null)
        {
            FindObjectOfType<LobbyPocketSelection>().CloseSelection();
            return;
        }
        // This method is called after game pause action 
        if (PauseManager.Instance.IsGamePaused())
        {
            PlayerInputController.UpdateAllPlayersMapInputs("UI");
        }
        else
        {
            PlayerInputController.UpdateAllPlayersMapInputs("Gameplay");
        }
    }

    public int GetPlayerIndex()
	{
        return GetInputController().GetInput().playerIndex;
    }

    public SpriteRenderer GetSpriteRenderer()
	{
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        return spriteRenderer;
	}

    public void SetInteractable(Interactable interactable)
    {
        this.interactable = interactable;
    }

    public void SetCamTarget(CinemachineTargetGroup.Target target)
    {
        camTarget = target;
    }

    public void IncreaseDefeatedEnemies()
	{
        defeatedEnemies++;

        if (isOnTutorial) return;
        int currentDefeated = PlayerPrefs.GetInt("ENEMIES", 0);
        currentDefeated++;

        PlayerPrefs.SetInt("ENEMIES", currentDefeated);
	}

    public int GetDefeatedEnemies()
	{
        return defeatedEnemies;
	}

    public string GetStatusText()
    {
        return $"Ammo Bonus: {AmmoBonus}<br>" +
             $"Bullet Distance Bonus: {bulletDistanceBonus}<br>" +
             $"Bullet Force Bonus: {BulletForceBonus}<br>" +
             $"Bullet Size Bonus: {BulletSizeBonus}<br>" +
             $"Damage Bonus: {DamageBonus}<br>" +
             $"Firerate Bonus: {FirerateBonus}<br>" +
             $"Gun Recharge Bonus: {GunRechargeBonus}<br>" +
             $"MaxHP Bonus: {MaxHPBonus}<br>" +
             $"Pushback Prevention: {PushBackPrevention}<br>" +
             $"Recoil: {RecoilStabilization}<br>" +
             $"Dash Duration Bonus: {DashDurationModifier}<br>" +
             $"Dash Speed Bonus: {DashSpeedModifier}<br>" +
             $"Dash Cooldown Bonus: {DashCooldownModifier}<br>" +
             $"Hitblocks: {HitBlocker}<br>" +
             $"OneHit Kills: {OneHitKill}<br>" +
             $"Projectile flight: {ProjectileFlight}<br>" +
             $"Projectile invulnerability: {ProjectileInvulnerability}<br>" +
             $"Contact invulnerability: {ContactInvulnerability}<br>" +
             $"Projectile pierce: {ProjectilePierce}<br>" +
             $"MoveSpeed Bonus: {MoveSpeedBonus}";
    }

    public void SetLightRadius(float radius, float innerRadius)
    {
        transform.Find("PlayerLight").GetComponent<Light2D>().pointLightOuterRadius = radius;
        transform.Find("PlayerLight").GetComponent<Light2D>().pointLightInnerRadius = innerRadius;
    }

    public void SetLightIntensity(float intensity)
    {
        transform.Find("PlayerLight").GetComponent<Light2D>().intensity = intensity;
    }

    public void SetLightColor(Color color)
    {
        transform.Find("PlayerLight").GetComponent<Light2D>().color = color;
    }

    void HandleFlight(bool value)
    {
        // player layer
        int defaultLayer = LayerMask.NameToLayer("IgnoreEnemies");
        // ignored layers
        int holesLayer = LayerMask.NameToLayer("Holes");
        int obstaclesLayer = LayerMask.NameToLayer("Obstacles");
        Physics2D.IgnoreLayerCollision(defaultLayer, holesLayer, value);
        Physics2D.IgnoreLayerCollision(defaultLayer, obstaclesLayer, value);
        flight = value;
    }

    public void DashFlight(bool flight)
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, flight ? 0 : 1);
        if (isPocketFlight) return;

        Flight = flight;
    }

    public void ResetPlayerAfterDungeon()
    {
        SetInverted(false);
        spriteRenderer.material.SetFloat("_GlitchAmount", 0f);

        if (GameplayManager.Instance.currentDungeonType ==  DungeonType.Glacier)
            ResetTemperature();
        else
            ResetTemperature(true);

        List<Gun> guns = GetAllGuns();
        for (int i = 0; i < guns.Count; i++)
        {
            guns[i].AbortRecharge();
            guns[i].GetPlayer().ReloadValues(1);
            guns[i].InstantReload();
        }
    }

    public void SetInverted(bool value)
    {
        _isInverted = value;

        if (!value)
            return;
        
        if (_disableInverted != null)
            StopCoroutine(_disableInverted);
        
        _disableInverted = StartCoroutine(DisableInvertedState());
        spriteRenderer.material.SetFloat("_GlitchAmount", 6f);
    }

    private IEnumerator DisableInvertedState()
    {
        yield return new WaitForSeconds(3.5f);
        SetInverted(false);
        spriteRenderer.material.SetFloat("_GlitchAmount", 0f);
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public void AddShield(int amount)
    {
        int amountLeft = 6 - HitBlocker;

        if (amount < amountLeft)
            HitBlocker += amount;
        else
            HitBlocker = 6;
    }

    public bool IsLaserSightOn() { return isLaserSightOn; }
    public void SetLaserSightOn(bool isLaserSightOn) { this.isLaserSightOn = isLaserSightOn; }

    public void AddPowerUp(CollectibleDisplayInfo info)
	{
        powerUps.Add(info);
	}
    public List<CollectibleDisplayInfo> GetPowerUps() { return powerUps; }

    public void EnablePlayerLight(bool enable)
    {
        transform.Find("PlayerLight").gameObject.SetActive(enable);

        foreach (Pocket pocket in pockets)
        {
            pocket.GetComponentInChildren<Light2D>().enabled = enable;
        }
    }

    public bool IsExternalForcingMovement() { return isExternalForcingMovement; }

    public void SetExternalForcedMovement(bool activated, Vector2 direction, float speed)
	{
        isExternalForcingMovement = activated;

        if (activated)
		{
            rb.velocity = direction * speed;
		}
	}
}

public enum AnimationFacing
{
    Front = 1,
    Back = 2,
    Side = 3,
    SideRight = 4,
}

public enum PlayerFacingDirections
{
    Right = 1,
    Down = 2,
    Left = 3,
    Up = 4,
};

public enum PlayerStates
{
    Idle = 1,
    Running = 2,
};
