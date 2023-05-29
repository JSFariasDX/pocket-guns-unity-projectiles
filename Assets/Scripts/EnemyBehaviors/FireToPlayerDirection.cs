using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireToPlayerDirection : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5;
    public float projectileDamage = 15;
    public float cooldownTime = 3f;
    bool isActivated = true;

    bool isCooldown = true;

    Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        Invoke("ReleaseCooldown", cooldownTime);
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (isActivated && !isCooldown)
        {
            StartCoroutine(Fire());
        }
    }

    IEnumerator Fire()
    {
        Transform target = enemy.GetRandomPlayer().transform;

        if (target)
        {
            if (enemy) enemy.PlayAttackSound();
            isCooldown = true;

            GameObject pGO = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Projectile projectile = pGO.GetComponent<Projectile>();
            projectile.Setup(target.position, projectileDamage, projectileSpeed, transform);
            yield return new WaitForSeconds(cooldownTime);
            isCooldown = false;
        }
    }

    void ReleaseCooldown()
    {
        isCooldown = false;
    }

    public void SetActivation(bool value)
    {
        isActivated = value;
    }
}
