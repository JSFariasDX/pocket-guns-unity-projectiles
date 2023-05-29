using System;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType
{
    Default,
    Egg,
    PowerUp,
}

public class Collectible : Loot
{
    [field:SerializeField] public int SellPrice { get; private set; }
    [SerializeField]
    string displayName;
    [SerializeField]
    [TextArea]string description;
    [SerializeField]
    protected float primaryReferenceAmount;
    AudioSource audioSource;
    public AudioClip collectSound;
    protected bool isCollected = false;
    public CollectibleType type = CollectibleType.Default;
    public int amount = 1;
    [SerializeField] CollectibleDisplayInfo collectibleInfo;
    [SerializeField] bool isPowerUp;

    [Header("Animation Helpers")]
    protected SpriteRenderer spriteRenderer;

    [Header("Animation")]
    public AnimationCurve idleCurve;
    public AnimationCurve dropCurve;
    [Range(0, 2)]
    float curveSpeed = .5f;
    float dropTime;
    float curveTime;

    public bool canFloat = true;

    public bool isCollectible = true;

    [Header("FX")]
    public GameObject collectFX;
    public PickUpPopUp popUp;

    public bool canBeCollected = true;

    protected virtual void Awake()
	{
        spriteRenderer = GetSpriteRenderer();

        if (type == CollectibleType.Egg)
		{
            GetComponent<Pocket>().enabled = false;
            GetComponent<Special>().enabled = false;
            GetComponent<Health>().enabled = false;
        }
    }

	protected override void Start()
    {
        audioSource = GetComponent<AudioSource>();

        SetupCollectibleInfo();

        var shadowTransform = transform.Find("Shadow");
        if (shadowTransform != null)
            shadowSprite = shadowTransform.GetComponent<SpriteRenderer>();

        dropTime = 0;

        startLootSpriteY = spriteRenderer.transform.position.y;
        spriteMoveDirection = Vector2.up;
        PlayDropAnimation();

        GameplayManager.Instance.clearOnDungeonEnd.Add(gameObject);

        popup = FindObjectOfType<ItemPopup>();
    }

    private SpriteRenderer GetSpriteRenderer()
    {
        Transform sprite = transform.Find("Sprite");
        if (sprite) {
            return sprite.GetComponent<SpriteRenderer>();
        } else
        {
            return GetComponentInParent<SpriteRenderer>();
        }
    }

    protected void Update()
    {
        if (animationState == AnimationState.Idle) IdleAnimation();
        else if (animationState == AnimationState.Dropping) DropAnimation();
    }

    protected override void IdleAnimation()
    {
        if (!spriteRenderer) return;

        if (canFloat)
        {
            if (!canCatch)
                canCatch = true;

            curveTime += Time.deltaTime * curveSpeed;

            if (curveTime >= 1)
                curveTime -= 1;

            spriteRenderer.transform.localPosition = new Vector2(0, idleCurve.Evaluate(curveTime));
        }
    }

    protected override void DropAnimation()
    {
        if (dropTime < 1)
            dropTime += Time.deltaTime;
        else
        {
            animationState = AnimationState.Idle;
            canCatch = true;
        }

        spriteRenderer.transform.localPosition = new Vector2(0, dropCurve.Evaluate(dropTime));
    }

    private void PlayDropAnimation()
    {
        animationState = AnimationState.Dropping;
    }

    public override void ShowPopUp(Player player)
    {
        if (type != CollectibleType.PowerUp)
            return;
        
        GetPopup().ShowItemInfo(shadowSprite.transform, this, player);
    }

    public override void GetLoot(Player player)
    {
        if (isCollected || !isCollectible)
        {
            return;
        }
        
        base.GetLoot(player);

        float destroyTime = 0.15f;

        onPlayerCollect(player);

        if (collectSound != null && audioSource)
        {
            audioSource.PlayOneShot(collectSound);
        }

        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeCollected) return;

        float destroyTime = 0.15f;

        if (type == CollectibleType.PowerUp)
            return;
        
        if (!isCollected && collision.CompareTag("CollectCol") && isCollectible)
        {
            Player player = collision.GetComponentInParent<Player>();
            onPlayerCollect(player);
            if (collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

            if (type == CollectibleType.Default)
                Destroy(gameObject, destroyTime);
                
            else if (type == CollectibleType.Egg)
			{
                Transform slot = player.pocketParent;
                gameObject.transform.SetParent(slot);
                gameObject.transform.position = slot.position;

                Pocket thisPocket = GetComponent<Pocket>();
                thisPocket.enabled = true;
                thisPocket.SetupPocket(player);
                player.AddPocket(thisPocket);

                player.SetCurrentPocket(thisPocket);

                if (GetComponent<TutorialEvents>())
                    GetComponent<TutorialEvents>().CompleteTutorial();

                Destroy(this);
            }
        }
    }

    public void InstantCollect(Player p)
    {
        if (type == CollectibleType.PowerUp)
        {
            GetLoot(p);
            print(displayName + " Collected by " + p.characterName);
        }
    }

    public virtual void onPlayerCollect(Player player)
    {
        if (isPowerUp)
        {
            collectibleInfo = new CollectibleDisplayInfo(displayName, spriteRenderer.sprite);
            player.AddPowerUp(collectibleInfo);
        }

        if (isCollected || !isCollectible)
        {
            return;
        }

        SpawnFX(player);

        isCollected = true;

        if (type == CollectibleType.Default)
        {
            int coins = PlayerPrefs.GetInt("COINS", 0);
            coins++;
            PlayerPrefs.SetInt("COINS", coins);
        } else if (type == CollectibleType.PowerUp)
        {
            int powerups = PlayerPrefs.GetInt("POWER UP", 0);
            powerups++;
            PlayerPrefs.SetInt("POWER UP", powerups);
        }

    }

    public virtual void SpawnFX(Player player)
    {
        if (collectFX)
        {
            GameObject particle = Instantiate(collectFX, transform.position, Quaternion.identity);
            particle.GetComponentInChildren<Animator>().speed = 1.5f;
        }

        if (popUp != null)
        {
            PickUpPopUp pop = Instantiate(popUp, player.transform.position, Quaternion.identity);
            pop.Setup(spriteRenderer.sprite, .75f);
        }
    }

    public string GetName()
    {
        return displayName;
    }

    internal string GetDescription()
    {
        return description;
    }

    public void SetName(string name)
    {
        displayName = name;
    }

    public void SetDescription(string desc)
    {
        description = desc;
    }

    public string GetDisplayName() { return displayName; }
    public CollectibleDisplayInfo GetCollectibleInfo()
	{
        return collectibleInfo;
	}
    public void SetupCollectibleInfo()
	{
        collectibleInfo = new CollectibleDisplayInfo(displayName, spriteRenderer.sprite);
    }
}
