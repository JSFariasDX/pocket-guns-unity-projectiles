using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Pocket : MonoBehaviour
{
    [Header("Pet params")]
    public int pocketInventoryIndex;
    public int pocketIndex;
    public string pocketName;
    public PetType pocketType;
    [SerializeField] public PetEssence essence;
    public float speed = 15f;
    public Vector2Int hatchCountRange = new Vector2Int(10, 16);
    public int hatchCount = 2;
    [SerializeField]
    public bool flipY = false;
    public UiSpriteAnimationData eggAnimation;
    public RuntimeAnimatorController pocketAnimatorController;
    public int level = 1;
    public List<int> healthValues = new List<int>();

    [Header("References")]
    Transform target;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    Special special;
    Health health;
    Player player;

    [Header("Bar")]
    public GameObject pocketBar;
    public GameObject pocketSpecialBar;

    [Header("Sound clips")]
    public AudioClip hittedSound;
    [SerializeField] private AudioClip hatchSfx;
    private GameObject hitFilter;
    [SerializeField] public AudioClip activateAudio;
    [SerializeField] public AudioClip endAudio;
    [SerializeField] public AudioClip chargedAudio;
    [SerializeField] public AudioClip specialAudio;

    [Header("Specials")]
    public string specialTitle;
    [TextArea] public string specialDescription;
    [TextArea] public string passiveDescription;
    public List<PocketMod> mods = new List<PocketMod>();

    [Header("Pulsate")]
    public bool isPulsating = false;
    float pulseRate = 0;
    public AnimationCurve pulseCurve;

    [Header("Special VFX")]
    [SerializeField] private Light2D specialLight;
    [SerializeField] private Light2D eggLight;
    [HideInInspector] public Material defaultMaterial;
    public GameObject specialVfx;
    [SerializeField] private Material specialMaterial;
    public Material greyscaleMaterial;
    public bool hatched = false;
    [SerializeField] public GameObject fogMask;

    [Header("Gui References")]
    public Sprite pocketDefaultIcon;
    public Sprite pocketEggIcon;
    public Sprite pocketIcon;
    public Sprite specialIcon;
    public Color specialBarDefaultColor;
    public Gradient specialBarGradient;
    public Material pocketHudMaterial;

    [SerializeField]
    AudioClip collectSound;

    bool collected;

    // Hatch helpers
    private int defeatedEnemies;

    bool isFlashing = false;
    bool isAlive = true;
    [HideInInspector] public bool shouldFlash = true;

    public void Start()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        special = GetComponent<Special>();
        hatchCount = Random.Range(hatchCountRange.x, hatchCountRange.y);
        defaultMaterial = GetComponent<Renderer>().material;
        isAlive = true;

        if (healthValues.Count > 0 && level > 0)
		{
            health.SetMaxHealth(healthValues[level - 1]);
		}
    }

    private void Update()
    {
        pulseRate = (float) defeatedEnemies / hatchCount;

        float velocity = speed * Time.fixedDeltaTime;
        if (target)
        {
            //transform.position = Vector3.Lerp(transform.position, target.position, velocity);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            float distance = Vector2.Distance(transform.position, target.position);

            rb.MovePosition(Vector3.Lerp(transform.position, target.position, velocity));

            GameObject wallCollider = transform.GetChild(5).gameObject;

            if (distance > 2)
            {
                wallCollider.SetActive(false);
            }

            if (wallCollider.activeSelf == false && distance < 1)
                wallCollider.SetActive(true);
        }

        if (pocketBar == null)
            pocketBar = GameObject.FindGameObjectWithTag("PetHealth");

        if (pocketSpecialBar == null)
            pocketSpecialBar = GameObject.FindGameObjectWithTag("SpecialBar");

        if (health.isInvulnerable && !isFlashing && shouldFlash)
        {
            StartCoroutine(FlashPocket());
        }

        if (IsActive())
        {
            if (pulseRate > 0.2f && pocketType == PetType.Egg)
            {
                if (transform.localScale.x >= .85f)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(.8f, .8f, .8f), Time.unscaledDeltaTime * pulseCurve.Evaluate(pulseRate));
                    isPulsating = true;
                }
                else
                {
                    transform.localScale = new Vector3(.8f, .8f, .8f);
                    isPulsating = false;
                }

                Pulsate();
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            if (pocketType == PetType.Egg)
            {
                if (defeatedEnemies >= hatchCount && !player.inCombat)
                {
                    if (hatched) return;

                    var eggHelper = FindObjectOfType<TutorialHelper>(true);
                    eggHelper.IsHatchingEgg = true;
                    eggHelper.transform.parent.gameObject.SetActive(false);

                    if (GetPlayer())
                    {
                        var tut = FindObjectOfType<TutorialsController>(true);
                        tut.gameObject.SetActive(true);
                        tut.Setup(this);
                    }

                    //PauseManager.Instance.SimplePause();
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    hatched = true;

                    //Hatch();
                }
            }
        }

        if (pocketType == PetType.Egg && !collected && ScreenManager.currentScreen != Screens.Lobby)
            eggLight.gameObject.SetActive(true);
        else
            eggLight.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        int currentState = GetComponent<Animator>().GetInteger("state");
        if (pocketType == PetType.Egg && currentState != 0)
        {
            GetComponent<Animator>().SetInteger("state", 0);
            spriteRenderer.flipY = false;
        }

        if (pocketType == PetType.Default && currentState != 1)
        {
            GetComponent<Animator>().SetInteger("state", 1);
            spriteRenderer.flipY = flipY;
        }
        //HandleEffects();
    }

    private void HandleEffects()
    {
        bool isSecondary = !IsActive() && pocketType == PetType.Default;
        bool isPrimary = IsActive() && pocketType == PetType.Default;
        if (isSecondary && !special.isSecondaryEffectApplied)
        {
            //Debug.Log($"Applying {special.displayName} secondary effect");
            special.ApplySecondaryEffect();
        } else if (isPrimary && special.isSecondaryEffectApplied)
        {
            //Debug.Log($"Removing {special.displayName} secondary effect");
            special.RemoveSecondaryEffect();
        }
    }

    public void Pulsate()
    {
        if (pocketType != PetType.Egg) return;
        if (isPulsating) return;

        transform.localScale = Vector3.one;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player && pocketType == PetType.Egg && collision.CompareTag("CollectCol"))
        {
            Player p = collision.GetComponentInParent<Player>();
            HandleEggPickup(p);

            collected = true;

            StartCoroutine(EnableDisableWind(p));

            return;
        }
        if (IsActive() && pocketType != PetType.Egg)
        {
            bool isAggressor = collision.CompareTag("Enemy") || collision.CompareTag("EnemyProjectile");
            if (!player.IsDashing() && isAggressor && !health.isInvulnerable)
            {
                float damage = 0;
                if (collision.CompareTag("Enemy")) damage = collision.GetComponent<Enemy>().damage;

                else if (collision.CompareTag("EnemyProjectile")) 
                {
                    if (collision.GetComponent<Projectile>()) damage = collision.GetComponent<Projectile>().damage;
                    else if (collision.GetComponent<Laser>()) damage = collision.GetComponent<Laser>().GetDamage();
                }

                health.Decrease(damage, null, null, 2);
                PlaySfx(hittedSound, true);

                if (collision.TryGetComponent<Projectile>(out Projectile projectile))
                    projectile.OnTrigger();
            }
        }
    }

    IEnumerator EnableDisableWind(Player p)
    {
        if (!p.hasLeafBlower)
            p.GetComponentInChildren<WindObject>().enabled = false;

        yield return new WaitForSeconds(1);

        if (!p.hasLeafBlower)
            p.GetComponentInChildren<WindObject>().enabled = true;
    }

    public void HandleEggPickup(Player p)
    {
        if (ScreenManager.currentScreen == Screens.Lobby) return;
        if (p.pockets.Count > 7) return;

        if (collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        if (p.GetCurrentPocket().GetSpecial().IsActivated()) 
            p.GetCurrentPocket().special.OnEnd();

        p.AddPocket(GetComponent<Pocket>());
        if (!p.GetCurrentPocket().special.isActivated)
        {
            p.SetCurrentPocket(GetComponent<Pocket>());
        }

        if (p.pockets.Count > 1)
        {
            p.entryPanel.GetUnlock().AddPocket(this);
            GameObject bossSpawn = GameObject.FindGameObjectWithTag("BossSpawn");
            bossSpawn.transform.Find("Portal").gameObject.SetActive(true);
        }

        transform.SetParent(p.pocketParent);

        p.Cheer();

        //GetComponentInParent<Player>().GetComponentInChildren<PocketRadialMenu>().UpdateUi();

    }

    public void OnExperienceGain()
	{
        defeatedEnemies++;
	}

    public void Hatch()
	{
        pocketType = PetType.Default;
        GetComponent<Health>().enabled = true;
        Special special = GetComponent<Special>();
        special.Setup(player);

        GetComponent<Animator>().runtimeAnimatorController = pocketAnimatorController;

        player.entryPanel.GetUnlock().AddPocket(this, true);

        //PauseManager.Instance.SimpleResume();

        //player.pocketHealthBar.transform.parent.parent.gameObject.SetActive(true);
        //player.specialBarGO.transform.parent.parent.gameObject.SetActive(true);
        //player.specialBarGO.GetComponent<SpecialBar>().Setup(gameObject);
    }

    public void SetupPocket(Player player)
	{
        this.player = player;
        transform.SetParent(player.pocketParent);
        SetTargetPosition(player.PetPositionLeft);

        health = GetComponent<Health>();
        health.toTrack = gameObject;
        special.Setup(player);
	}

    public void UnSetup()
    {
        player = null;
        SetTargetPosition(transform);

        //health.toTrack = null;
        //health = null;
        special.UnSetup();
        special.enabled = false;
    }

    public void SetTargetPosition(Transform t)
    {
        target = t;
    }

    public void onDamage()
    {
        audioSource.PlayOneShot(hittedSound);
        StartCoroutine(DamageFeedback());
    }

    HitFilterManager GetHitFilterManager()
    {
        return FindObjectOfType<HitFilterManager>();
    }

    IEnumerator DamageFeedback()
    {
        GetHitFilterManager().ActivePocketHitFilter(.3f);
        spriteRenderer.material.SetInt("_HitEffectBlend", 1);
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.material.SetInt("_HitEffectBlend", 0);
    }

    public void SetInvulnerability(bool value)
    {
        health.SetInvulnerability(value);
    }

    IEnumerator FlashPocket()
    {
        isFlashing = true;

        SetSpriteAlpha(.2f);
        yield return new WaitForSeconds(0.2f);
        SetSpriteAlpha(1);
        yield return new WaitForSeconds(0.2f);
        isFlashing = false;
    }

    public void onHealthEnd()
    {
        if (special.IsActivated())
        {
            special.OnEnd();
        }

        if (pocketBar != null)
            pocketBar.SetActive(false);
        if (pocketSpecialBar != null)
            pocketSpecialBar.SetActive(false);

        spriteRenderer.material = greyscaleMaterial;
        GetHitFilterManager().ActivePocketHitFilter(false);
        StartCoroutine(PocketDies(.35f));
    }

    IEnumerator PocketDies(float time)
	{
        yield return new WaitForSeconds(time);
        player.OnPocketDies();
        EnableColliders(false);
        SetSpritesVisibility(false);
        isAlive = false;
	}

    public void EndSpecialIfActive()
    {
        BroadcastMessage("EndSpecial");
    }

    void SetSpriteAlpha(float value)
    {
        spriteRenderer.material.SetFloat("_Alpha", value);
    }

    public Sprite GetHudSprite()
    {
        if (pocketType == PetType.Default) return pocketDefaultIcon;
        else return pocketEggIcon;
    }

    public Health GetHealth()
	{
        // print("Pet health: " + health);
        return health;
	}

    public void PlaySfx(AudioClip clip, bool stop = true)
    {
        if (stop) audioSource.Stop();

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void ActivePocket(bool active)
	{
        if (active)
		{
            transform.localScale = Vector3.one;
            EnableColliders(true);
        }
		else
		{
            transform.localScale = Vector3.zero;
            EnableColliders(false);
        }
	}

    List<Collider2D> colliders = new List<Collider2D>();
    private void EnableColliders(bool enable)
	{
        if (colliders.Count == 0) 
        {
            foreach (Collider2D col in GetComponents<Collider2D>())
            {
                colliders.Add(col);
            }
            foreach (Collider2D col in GetComponentsInChildren<Collider2D>(true))
            {
                colliders.Add(col);
            } 
        }

        foreach(Collider2D col in colliders)
		{
            col.enabled = enable;
		}
    }

    public bool IsActive()
	{
        return player != null && player.currentPocket == this && isAlive;
	}

    public Player GetPlayer()
    {
        return player;
    }
    
    public void SetSpecialVfxActive(bool active)
	{
        if (specialLight)
        {
            specialLight.gameObject.SetActive(active);

            if(GameplayManager.Instance.currentDungeonType == DungeonType.Forest)
                specialLight.pointLightOuterRadius = 3;
            else if(GameplayManager.Instance.currentDungeonType == DungeonType.Glacier)
                specialLight.pointLightOuterRadius = 3;
            else if(ScreenManager.currentScreen == Screens.Lobby && essence == PetEssence.Protect)
                specialLight.pointLightOuterRadius = 6;
            else
                specialLight.pointLightOuterRadius = 10;
        }
        
        if (active)
		{
            spriteRenderer.material = specialMaterial;
            spriteRenderer.sortingOrder = 350;
		}
		else
		{
            spriteRenderer.material = defaultMaterial;
            spriteRenderer.material.SetInt("_HitEffectBlend", 0);
            SetSpriteAlpha(1f);
            spriteRenderer.sortingOrder = 6;
        }
    }
    
    public void EnableSpecial()
    {
        special.enabled = true;
    }

    public void SetSpritesVisibility(bool value)
    {
        SpriteRenderer[] All = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprRender in All)
        {
            sprRender.enabled = value;
        }
    }

    public void Ressurect()
    {
        if (isAlive)
        {
            return;
        }
        spriteRenderer.material = defaultMaterial;
        SetSpecialVfxActive(false);
        float maxHealth = health.GetMaxHealth();
        health.SetHealth(maxHealth);
        isAlive = true;
        SetSpritesVisibility(true);
        EnableColliders(true);
    }

    public Special GetSpecial()
	{
        return special;
	}

    public SpriteRenderer GetSpriteRenderer()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>(); 
        return spriteRenderer;
    }

    public void Evolve()
	{
        level++;
	}
}

public enum PetType
{
    Egg, Default
}

public enum PocketModType
{
    Damage, Hp, MoveSpeed,
}

[System.Serializable]
public struct PocketMod
{
    public PocketModType modType;
    public float modValue;
}