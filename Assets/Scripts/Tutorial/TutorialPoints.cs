using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPoints : MonoBehaviour
{
    public Transform target;
    public float followTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!target)
            TryGetPlayer();
        else
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y, 0);

            transform.position = Vector3.Lerp(transform.position, newPos, followTime * Time.deltaTime);
        }
    }

    void TryGetPlayer()
    {
        target = Camera.main.transform.Find("TargetGroup");
    }
}
