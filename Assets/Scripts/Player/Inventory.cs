using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    List<GameObject> items;

    public void Add(GameObject go)
    {
        items.Add(go);
    }
}
