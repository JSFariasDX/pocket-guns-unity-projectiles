using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinToParent : MonoBehaviour
{
    Transform target;
    Vector3 offset;

    // Start is called before the first frame update
    public void Setup()
    {
        target = transform.parent;
        offset = transform.localPosition;
        transform.eulerAngles = Vector3.zero;
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        }
    }
}
