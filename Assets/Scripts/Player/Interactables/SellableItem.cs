using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum PriceType
{
    COIN,
    BLOOD,
    NONE
}

public class SellableItem : Interactable
{

    public Collectible item;
    [SerializeField] private Transform priceLabel;
    [SerializeField] private TMPro.TextMeshProUGUI itemValue;

    [SerializeField] private GameObject itemSlot;

    private int price = 0;
    private PriceType priceType = PriceType.COIN;

    private List<GameObject> _npcList;

    public void SetPriceActive(bool active)
    {
        itemValue.gameObject.SetActive(active);
    }

    public void SetPrice(PriceType priceType, int value)
    {
        this.priceType = priceType;

        switch (this.priceType)
        {
            case PriceType.COIN:
                itemValue.text = value.ToString();
                break;

            case PriceType.BLOOD:
                priceLabel.gameObject.SetActive(false);
                break;

            case PriceType.NONE:
                priceLabel.gameObject.SetActive(false);
                break;
        }

        price = value;
    }

    public void SetCurrentList(List<GameObject> list)
    {
        _npcList = list;
    }

    public override void OnInteract(Player player)
    {
        if (players.Count == 0)
        {
            return;
        }

        bool isSuccess = false;

        switch (priceType)
        {
            case PriceType.COIN:
                if (GameplayManager.Instance.CurrentCoins >= price)
                {
                    GameplayManager.Instance.RemoveCoins(price);
                    isSuccess = true;
                }
                break;

            case PriceType.BLOOD:
                Health playerHealth = player.GetComponent<Health>();
                float currentHealth = playerHealth.GetCurrentHealth();
                float maxHealth = playerHealth.GetMaxHealth();
                float calculatedPrice = maxHealth * price * 0.01f;
                if (currentHealth > calculatedPrice)
                {
                    playerHealth.Decrease(calculatedPrice);
                    isSuccess = true;
                }
                break;

            case PriceType.NONE: isSuccess = true; break;
        }

        if (isSuccess)
        {
            itemSlot.GetComponentInChildren<Collider2D>().enabled = isSuccess;
            item.onPlayerCollect(player);
            item.SpawnFX(player);
            SendMessageUpwards("OnItemBought");

            if (!_isRepeatable)
            {
                _npcList.Remove(gameObject);
                Destroy(gameObject);
            }
        } else
        {
            SendMessageUpwards("OnBuyFail");
        }
    }

    public void CheckPlayerBalance(int balance)
    {
        if (price > balance)
        {
            itemValue.color = Color.red;
        } else
        {
            itemValue.color = Color.white;
        }
    }
}
