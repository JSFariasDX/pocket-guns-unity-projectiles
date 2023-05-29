using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWhenStart : MonoBehaviour
{
    public GameObject prefab;
    public bool removeParent;
    public float destroyAfter;

    // Start is called before the first frame update
    void Start()
    {
        GameObject spawned = Instantiate(prefab, transform);
        if (removeParent)
        {
            spawned.transform.parent = null;
        }

        if (destroyAfter > 0)
        {
            Destroy(spawned, destroyAfter);
        }
    }
}
