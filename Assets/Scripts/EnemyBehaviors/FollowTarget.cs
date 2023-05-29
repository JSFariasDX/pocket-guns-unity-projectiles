using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    GameObject target;
    Enemy enemy;
    bool isActivated = true;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActivated && target != null)
        {
            Vector2 currentPosition = target.transform.position;
            float velocity = enemy.speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, currentPosition, velocity);
        }
    }

    public void SetTarget(GameObject t)
    {
        target = t;
    }

    public void SetActivation(bool value)
    {
        isActivated = value;
    }
}
