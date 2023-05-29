using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoStepsProjectile : Projectile
{
    [Header("Steps Settings")]
    public float stepTime = 2;
    float timer = 0;
    bool stepTook = false;

    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = shooterTransform.GetComponent<Boss>().GetEnemy().GetTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stepTook)
        {
            if (timer < stepTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                rb.velocity = Vector2.zero;

                Setup(target.position, damage, actualSpeed, shooterTransform);

                stepTook = true;
            }
        }
    }

    //Transform ClosestPlayer()
    //{

    //}
}
