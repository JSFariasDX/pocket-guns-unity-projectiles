using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEntrance : MonoBehaviour
{
    [SerializeField]
    GameObject topping;
    [SerializeField]
    Transform slot;
    bool isLocked = true;
    [SerializeField]
    Animator animator;
    GameObject currentLoot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLocked && collision.CompareTag("PlayerCollider"))
        {
            topping.transform.localPosition = new Vector3(0, 0.6f, 0);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            topping.transform.localPosition = Vector3.zero;
        }
    }

    public void Open()
    {
        isLocked = false;
        animator.SetTrigger("open");
        currentLoot.transform.localScale = new Vector3(1, 1, 0);
        bool isLoot = currentLoot.GetComponentInChildren<Loot>() != null;
        bool isCollectible = currentLoot.GetComponentInChildren<Collectible>() != null;
        if (isLoot)
        {
            currentLoot.GetComponentInChildren<Loot>().canCatch = true;
        } else if (isCollectible)
        {
            currentLoot.GetComponentInChildren<Collectible>().canCatch = true;
        }
    }

    public void SetLoot(GameObject loot)
    {
        currentLoot = loot;
        currentLoot.transform.localScale = new Vector3(.5f, .5f, 0);
        currentLoot.transform.SetParent(slot);
        bool isLoot = currentLoot.GetComponentInChildren<Loot>() != null;
        bool isCollectible = currentLoot.GetComponentInChildren<Collectible>() != null;
        if (isLoot)
        {
            currentLoot.GetComponentInChildren<Loot>().canCatch = !isLocked;
        } else if (isCollectible)
        {
            currentLoot.GetComponentInChildren<Collectible>().canCatch = !isLocked;
        }
        
    }
}
