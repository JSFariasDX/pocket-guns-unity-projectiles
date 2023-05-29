using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    bool opened = false;
    [Header("Components")]
    public BoxCollider2D trigger;

    [Header("FX")]
    public GameObject collectFX;
    public PickUpPopUp popUpPrefab;

    [Header("Sprite")]
    public Sprite openedChest;
    public Sprite closedChest;
    SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        base.Start();

        SetName("Open Chest");
        SetDescription("");
    }

    public override void OnInteract(Player player)
    {
        if (player.inCombat) return;
        OpenChest();
    }

    public void OpenChest()
    {
        if (opened) return;

        spriteRenderer.sprite = openedChest;

        BossKey thisKey = RandomUnusedKey();
        GiveKeyToPlayers(thisKey);

        GameObject particle = Instantiate(collectFX, transform.position, Quaternion.identity);
        particle.GetComponentInChildren<Animator>().speed = 1.5f;

        PickUpPopUp pop = Instantiate(popUpPrefab, transform.position, Quaternion.identity);
        pop.Setup(thisKey.keySprite, 1f);

        trigger.enabled = false;
        popUp.gameObject.SetActive(false);
        opened = true;

        GetComponent<AudioSource>().Play();
    }

    void GiveKeyToPlayers(BossKey key)
    {
        GameplayManager.Instance.AddKeys(key);
        FindObjectOfType<CoinGui>().ShowKey(key.keySprite);
    }

    BossKey RandomUnusedKey()
    {
        List<BossKey> keys = new List<BossKey>();

        for (int i = 0; i < GameplayManager.Instance.GetDungeonConfig().possibleKeys.Count; i++)
        {
            if(!GameplayManager.Instance._keys.Contains(GameplayManager.Instance.GetDungeonConfig().possibleKeys[i]))
                keys.Add(GameplayManager.Instance.GetDungeonConfig().possibleKeys[i]);
        }

        BossKey key = keys[Random.Range(0, keys.Count)];

        return key;
    }
}
