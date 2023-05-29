using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenStatistics : Interactable
{
    [Header("Stats")]
    public StatsController stats;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SetName("Stats");
        SetDescription("Do you want to check your stats?");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnInteract(Player player)
    {
        if(!stats.GetStatsEnabled())
            stats.EnableStats(true);
    }
}
