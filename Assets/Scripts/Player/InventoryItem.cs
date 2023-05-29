using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
// https://www.youtube.com/watch?v=SGz3sbZkfkg
public class InventoryItem : MonoBehaviour
{
    public InventoryItemData data { get; private set; }
    public int stackSize { get; private set; }

    public InventoryItem(InventoryItemData source)
    {
        data = source;
        AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}
