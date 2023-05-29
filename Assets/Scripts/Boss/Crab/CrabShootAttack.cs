using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabShootAttack : MonoBehaviour
{
    [Header("Shoot Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private int count;
    [SerializeField] private float interval;
    [SerializeField] private float delayBeforeShoot;
    [SerializeField] private float delayAfterAnimation;

    private Crab _crab;

    private void Awake()
    {
        _crab = GetComponentInParent<Crab>();
    }

    public IEnumerator NormalShoot(List<Projectile> projectileList = null)
    {
        _crab.Animator.SetBool("Move", false);
        _crab.Animator.SetBool("Dive", false);

        yield return new WaitForSeconds(delayBeforeShoot);
        _crab.Animator.SetTrigger("Shoot");

        yield return new WaitForSeconds(delayAfterAnimation);

        for (int i = 0; i < count; i++)
		{
            Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            projectile.Setup(_crab.GetEnemy().GetTarget(), damage, speed, transform.parent);

            if (projectileList != null)
                projectileList.Add(projectile);

            yield return new WaitForSeconds(interval);
		}

        _crab.CurrentState = CrabState.Idle;
        _crab.CurrentCoroutine = null;
    }
}
