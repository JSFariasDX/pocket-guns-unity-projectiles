using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCStatus
{
    Activated = 1,
    Deactivated = 2,
}

public class NPC : MonoBehaviour
{
    [SerializeField] protected NpcSO npcSettings;

    [Header("Slot Dependencies")]
    [SerializeField] private Transform sellSlotOne;

    [SerializeField] private Transform sellSlotTwo;

    [SerializeField] private Transform sellSlotThree;
    [SerializeField] GameObject puffPrefab;

    [Header("Prefab Dependencies")]
    [SerializeField] private GameObject sellableItemPrefab;
    [SerializeField] private GameObject dialogBaloon;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip successSellSound;
    [SerializeField] private AudioClip failSellSound;
    [SerializeField] private AudioClip breakIdleSound;

    private List<GameObject> _possibleItems;
    private List<GameObject> _currentItems;
    private bool _hasItemOnSale;

    private NPCStatus status = NPCStatus.Deactivated;
    private bool isBreakIdleCounting = false;
    private Coroutine breakIdleRoutine;

    protected Animator animator;
    private AudioSource audioSource;

    protected virtual void Start()
    {
        _possibleItems = new();

        foreach (GameObject item in npcSettings.PossibleItems)
            _possibleItems.Add(item);
        
        _currentItems = new();

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        List<Transform> sellSlots = GetSlotsList();

        if (!sellableItemPrefab) return;

        foreach (var sellSlot in sellSlots)
        {
            GameObject sellable = Instantiate(sellableItemPrefab, sellSlot);
            SellableItem sellableInstance = sellable.GetComponent<SellableItem>();
            Transform itemPosition = sellable.transform.Find("Item");

            int sortedItemIndex;
            GameObject item;

            var guaranteedSlot = sellSlot.gameObject.GetComponentInChildren<GuaranteedSellSlot>();
            if (guaranteedSlot != null)
                item = Instantiate(guaranteedSlot.EvaluateItem(out sortedItemIndex), itemPosition);
            else
            {
                sortedItemIndex = Random.Range(0, _possibleItems.Count);
                item = Instantiate(_possibleItems[sortedItemIndex], itemPosition);
            }
            

            Collectible collectible = item.GetComponent<Collectible>();
            sellableInstance.item = collectible;
            collectible.isCollectible = false;
            
            string itemName = collectible.GetName();
            sellableInstance.SetName(itemName);
            string itemDesc = collectible.GetDescription();
            sellableInstance.SetDescription(itemDesc);
            sellableInstance.SetRepeatable(npcSettings.IsRepeatable);

            foreach (Collider2D col in item.GetComponents<Collider2D>())
            {
                col.enabled = false;
            }

            int price;
            if (collectible.SellPrice != 0)
                price = collectible.SellPrice;
            else
            {
                price = npcSettings.GetPrice(_hasItemOnSale, out bool onSale);
                if (onSale)
                    _hasItemOnSale = true;
            }

            sellableInstance.SetPrice(npcSettings.PriceType, price);

            _currentItems.Add(sellable);
            sellableInstance.SetCurrentList(_currentItems);
            
            if (guaranteedSlot == null)
                _possibleItems.RemoveAt(sortedItemIndex);
        }

        CheckItemsPrice(GameplayManager.Instance.CurrentCoins);
        SetPricesActive(false);
    }

    private List<Transform> GetSlotsList()
    {
        List<Transform> slots = new List<Transform>();
        if (sellSlotTwo != null)
        {
            slots.Add(sellSlotTwo);
        }
        if (sellSlotOne != null)
        {
            slots.Add(sellSlotOne);
        }
        if (sellSlotThree != null)
        {
            slots.Add(sellSlotThree);
        }
        return slots;
    }

    private void Update()
    {
        int currentAnimatorStatus = animator.GetInteger("status");
        if ((int)status != currentAnimatorStatus)
        {
            animator.SetInteger("status", (int)status);
        }

        if (status == NPCStatus.Activated && !isBreakIdleCounting)
        {
            breakIdleRoutine = StartCoroutine(BreakIdle(10));
        }
    }

    public virtual void OnItemBought()
    {
        TriggerDialogue(npcSettings.BuySuccessDialogue);

        AbortBreakIdle();
        string buyAnimation = Random.value > .5 ? "buyOne" : "buyTwo";
        PlaySuccessSellSound();
        animator.SetTrigger(buyAnimation);

        if (npcSettings.IsSingleBuy)
        {
            foreach (Transform slot in GetSlotsList())
            {
                GameObject puff = Instantiate(puffPrefab, slot.position, Quaternion.identity);
                Destroy(puff, .5f);
                Destroy(slot.gameObject, .15f);
            }

            _currentItems.Clear();
        }
    }

    public void OnBuyFail()
    {
        TriggerDialogue(npcSettings.BuyFailDialogue);
        PlayFailSellSound();
    }

    public void EggTraining()
    {
        if (ScreenManager.currentScreen != Screens.Lobby) return;

        TriggerDialogue(npcSettings.NoItemDialogue);
        PlayFailSellSound();
    }

    public void OnActivate()
    {
        if (_currentItems.Count <= 0)
        {
            TriggerDialogue(npcSettings.NoItemDialogue);
            return;
        }

        TriggerDialogue(npcSettings.ActivationDialogue);
        status = NPCStatus.Activated;

        CheckItemsPrice(GameplayManager.Instance.CurrentCoins);
        SetPricesActive(true);
    }

    public void OnDeactivate()
    {
        TriggerDialogue(npcSettings.DeactivationDialogue);
        SetPricesActive(false);
        status = NPCStatus.Deactivated;
        AbortBreakIdle();
    }

    private void TriggerDialogue(DialogueListSO dialogueList)
    {
        if (dialogueList.List.Count == 0)
            return;

        var dialogue = dialogueList.GetRandomDialogueFromList();
        TriggerDialogue(dialogue);
    }

    protected void TriggerDialogue(Dialogue dialogue, bool lobby = false)
    {
        if (dialogBaloon == null)
            return;
        
        dialogBaloon.SetActive(true);
        dialogBaloon.GetComponent<DialogueController>().StartDialogue(dialogue, lobby);
    }

    IEnumerator BreakIdle(float secondsToFire)
    {
        isBreakIdleCounting = true;
        var instruction = new WaitForEndOfFrame();
        while (secondsToFire > 0)
        {
            secondsToFire -= Time.deltaTime;
            yield return instruction;
        }
        animator.SetTrigger("breakIdle");
        isBreakIdleCounting = false;
    }

    protected void AbortBreakIdle()
    {
        StopCoroutine(breakIdleRoutine);
        breakIdleRoutine = null;
        isBreakIdleCounting = false;
    }

    public NPCStatus GetStatus()
    {
        return status;
    }

    protected void PlaySuccessSellSound()
    {
        audioSource.PlayOneShot(successSellSound);
    }

    void PlayFailSellSound()
    {
        audioSource.PlayOneShot(failSellSound);
    }

    public void PlayBreakIdleSound()
    {
        audioSource.PlayOneShot(breakIdleSound);
    }

    public void CheckItemsPrice(int balance)
    {
        List <Transform> sellSlots = GetSlotsList();
        foreach (var sellSlot in sellSlots)
        {
            SellableItem item = sellSlot.GetComponentInChildren<SellableItem>();
            if (item != null)
            {
                item.CheckPlayerBalance(balance);
            }
        }
    }

    public void SetPricesActive(bool active)
    {
        var sellSlots = GetSlotsList();
        foreach (var sellSlot in sellSlots)
        {
            var item = sellSlot.GetComponentInChildren<SellableItem>();
            if (item != null)
            {
                item.SetPriceActive(active);
            }
        }
    }
}
