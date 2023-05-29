using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootPocket : Loot
{
    // Start is called before the first frame update
    protected override void Start()
    {
        canCatch = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ShowPopUp(Player player)
    {
        GetPopup().ShowPocketInfo(transform, GetComponent<Pocket>(), player);
    }
}
