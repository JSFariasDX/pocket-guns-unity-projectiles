using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuaranteedSellSlot : MonoBehaviour
{
    [SerializeField] private List<GameObject> guaranteedItems;

    public GameObject EvaluateItem(out int index)
    {
        index = Random.Range(0, guaranteedItems.Count);
        return guaranteedItems[index];
    }
}
