using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinisterTrapAttack : MonoBehaviour
{
    Animator anim;

    public float attackTime = .5f;
    float timer;
    bool attacked = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        timer = attackTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            if (!attacked)
                Attack();
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        GetComponent<BoxCollider2D>().enabled = true;
        Invoke("Death", 1.5f);
        attacked = true;
    }

    void Death()
    {
        gameObject.SetActive(false);
    }
}
