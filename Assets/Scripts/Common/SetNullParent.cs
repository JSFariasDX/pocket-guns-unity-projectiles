using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNullParent : MonoBehaviour
{
    void Start()
    {
        transform.parent = null;
    }
}
