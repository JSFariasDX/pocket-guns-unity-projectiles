using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public enum PetEssence
{
    Accuracy,
    Movement,
    Shot,
    GlobalDamage,
    Reward,
    Health,
    Protect
}

public enum SpecialUseType
{
    TimeBased,
    NBased,
}

public class Special : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    public string displayName;
    [TextArea, SerializeField] string description;
    [SerializeField] public SpecialUseType useType;
    [SerializeField] int maxUses = 0;
    float tickTime = 1f;
    [SerializeField] public List<float> totalTime;
    [SerializeField] List<float> rechargeTime;
    [SerializeField] private bool timeRecoverEachDungeon = false;
    float charge;
    public bool isActivated = false;
    bool isTicking = false;
    bool isRecharging = false;
    bool isFlashing = false;
    bool isFlashingName = false;
    [SerializeField] public SpecialParticles specialParticle;
    [SerializeField] public GameObject specialExplosion;
    GameObject mask;

    int currentUses;

    AudioSource audioSource;
    protected Player player;
    Pocket pocket;

    public bool isSecondaryEffectApplied = false;

    [Header("Audio")]
    public AudioMixerGroup sfxMixer;
    public List<AudioClip> specialClips = new List<AudioClip>();
    public void Setup(Player player)
	{
        this.player = player;
        pocket = GetComponent<Pocket>();
        pocket.GetSpecial().RestoreUses();
    }

    public void UnSetup()
    {
        player = null;
        pocket = null;
    }

    void Start()
    {
        RestoreUses();
        if (useType == SpecialUseType.TimeBased)
        {
            charge = totalTime[GetCurrentPet().level - 1];
        } else if (useType == SpecialUseType.NBased)
        {
            charge = maxUses;
            currentUses = maxUses;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!player || PauseManager.Instance.IsGamePaused())
        {
            return;
        }

        HandleRecharge();
        HandleTick();

        if (isActivated && !isFlashingName)
        {
            StartCoroutine("FlashName");
        }

        if(!audioSource)
            audioSource = GetComponent<AudioSource>();
    }

    private void HandleRecharge()
    {
        if (useType != SpecialUseType.TimeBased)
        {
            return;
        }
        if (!isActivated && charge < totalTime[GetCurrentPet().level - 1] && !isRecharging)
        {
            StartCoroutine("Recharge");
        }
    }

    private void HandleTick()
    {
        if (useType != SpecialUseType.TimeBased)
        {
            return;
        }
        if (isActivated && charge > 0 && !isTicking)
        {
            StartCoroutine("Tickdown");
        }
    }

    public void TryActivate(InputAction.CallbackContext ctx)
    {
        if (pocket.pocketType == PetType.Egg || (ScreenManager.currentScreen == Screens.Lobby && !GetCurrentPlayer().isOnTutorial))
            return;
        
        PocketRadialMenu pocketMenu = player.GetPocketMenu();
        bool isRadialMenuOpen = pocketMenu != null && pocketMenu.isOpen;
        if (isRadialMenuOpen)
        {
            pocketMenu.Close();
        }
        bool hasCharge = useType == SpecialUseType.TimeBased && charge >= totalTime[GetCurrentPet().level - 1];
        bool hasUses = useType == SpecialUseType.NBased && currentUses > 0;
        if ((hasCharge || hasUses) && pocket.IsActive())
        {
            OnActivate();
        } else
        {
            audioSource.PlayOneShot(pocket.endAudio);
        }
    }

    public virtual void OnActivate()
    {
        audioSource.PlayOneShot(pocket.activateAudio);
        audioSource.PlayOneShot(pocket.specialAudio);
        GameObject vfx = Instantiate(GetCurrentPet().specialVfx, transform.position, Quaternion.identity);
        float vfxDestroyTime = 5;
        if (useType == SpecialUseType.NBased)
        {
            currentUses--;
            charge--;
            vfxDestroyTime = 1;
        } else
        {
            StartCoroutine(Tickdown());

            SetDisplayText(displayName);
            SetNameDisplaying(true);

            pocket.SetSpecialVfxActive(true);
            isActivated = true;
        }

        if (useType == SpecialUseType.TimeBased)
        {
            if (GameplayManager.Instance.currentDungeonType == DungeonType.Glacier)
            {
                mask = Instantiate(pocket.fogMask, transform.position, Quaternion.identity, transform);
                mask.transform.position = transform.position;
            }
        }

        Destroy(vfx, vfxDestroyTime);
    }

    public virtual void OnEnd()
    {
        charge = 0;
        audioSource.PlayOneShot(pocket.endAudio);
        isActivated = false;
        pocket.SetSpecialVfxActive(false);
        SetNameDisplaying(false);

        if (useType == SpecialUseType.TimeBased)
        {
            if (mask != null)
                Destroy(mask);
        }
    }

    public virtual void ApplySecondaryEffect(){
        isSecondaryEffectApplied = true;
    }

    public virtual void RemoveSecondaryEffect(){
        isSecondaryEffectApplied = false;
    }

    IEnumerator Recharge()
    {
        isRecharging = true;
        float chargeAmount = totalTime[GetCurrentPet().level - 1] / rechargeTime[GetCurrentPet().level - 1];
        charge += chargeAmount;
        yield return new WaitForSeconds(tickTime);
        isRecharging = false;
        if (charge >= totalTime[GetCurrentPet().level - 1])
        {
            audioSource.PlayOneShot(pocket.chargedAudio);
        }
    }

    IEnumerator Tickdown()
    {
        isTicking = true;
        charge -= tickTime;
        yield return new WaitForSeconds(tickTime);
        if (charge <= 0)
        {
            OnEnd();
        }
        isTicking = false;
    }

    IEnumerator FlashName()
    {
        isFlashingName = true;
        TMPro.TextMeshProUGUI t = GetDisplayNameComponent();
        t.color = new Color32(0, 255, 255, 255);
        yield return new WaitForSeconds(0.6f);
        t.color = new Color32(255, 0, 255, 255);
        yield return new WaitForSeconds(0.6f);
        isFlashingName = false;
    }

    TMPro.TextMeshProUGUI GetDisplayNameComponent()
    {
        TMPro.TextMeshProUGUI t = player.playerGui.specialText;
        return t;
    }

    void SetNameDisplaying(bool value)
    {
        TMPro.TextMeshProUGUI t = GetDisplayNameComponent();
        t.enabled = value;
    }

    void SetDisplayText(string text)
    {
        TMPro.TextMeshProUGUI t = GetDisplayNameComponent();
        t.text = text;
    }

    public bool IsActivated()
    {
        return isActivated;
    }

    public void EndSpecial()
    {
        if (isActivated)
        {
            OnEnd();
        }
    }

    public float GetCharge()
    {
        return charge;
    }

    public float GetTotalTime()
    {
        if (GetCurrentPet().level > 0)
            return totalTime[GetCurrentPet().level - 1];
        else return totalTime[1];
    }

    public float GetSpecialAmount()
    {
        if (useType == SpecialUseType.TimeBased)
        {
            return GetCharge() / GetTotalTime();
        } else
        {
            float amount = (float)currentUses / (float)maxUses;
            return amount;
        }
    }

    public Player GetCurrentPlayer()
    {
        return player;
    }

    public Pocket GetCurrentPet()
    {
        return pocket;
    }

    protected float GetPercentValue(float percent)
    {
        return percent / 100;
    }

    protected void GlobalDamage(float damageAmount)
    {
        GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log(currentEnemies.Length);
        //foreach (var enemy in currentEnemies)
        //{
        //    enemy.GetComponent<Health>().Decrease(damageAmount, null, null, 0);
        //}

        for (int i = 0; i < currentEnemies.Length; i++)
        {
            currentEnemies[i].GetComponent<Health>().Decrease(damageAmount, null, null, 0);

            if (specialExplosion)
            {
                GameObject explosion = Instantiate(specialExplosion, currentEnemies[i].transform.position, Quaternion.identity);
                AudioSource explosionSource = explosion.AddComponent<AudioSource>();
                explosionSource.outputAudioMixerGroup = sfxMixer;
                explosionSource.spatialBlend = .8f;
                explosionSource.PlayOneShot(specialClips[Random.Range(0, specialClips.Count)]);
            }
        }
    }

    public void RestoreUses()
    {
        if (useType == SpecialUseType.NBased)
            currentUses = maxUses;
        if (useType == SpecialUseType.TimeBased && timeRecoverEachDungeon)
            charge = totalTime[GetCurrentPet().level - 1];

        print("<color=red> RESET SPECIAL </color>");
    }
}

[System.Serializable]
public class Levels
{
    public int value;
}