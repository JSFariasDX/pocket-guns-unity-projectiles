using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabRandomAttack : MonoBehaviour
{

    [Header("Attack4 Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Vector2 targetOffset;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float duration;
    [SerializeField] private float interval;
    [SerializeField] private float delayBeforeShoot;

    private float _attack4Counter;
    private bool _isAttacking;

    private Crab _crab;

    private void Awake()
    {
        _crab = GetComponentInParent<Crab>();
    }

    private void FixedUpdate()
    {
        if (_isAttacking)
            _attack4Counter += Time.deltaTime;
    }

    public IEnumerator RandomShoot()
    {
        _isAttacking = true;

        _crab.Animator.SetBool("Move", false);
        _crab.Animator.SetBool("Dive", false);

        yield return new WaitForSeconds(delayBeforeShoot);
        _crab.Animator.SetBool("ShootRandom", true);

        while (_attack4Counter <= duration)
        {
            Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            float randomX = Random.Range(-targetOffset.x, targetOffset.x);
            float randomY = Random.Range(-targetOffset.y, targetOffset.y);
            var offset = new Vector3(randomX, randomY, 0f);

            projectile.Setup(_crab.GetEnemy().GetTarget().position + offset, damage, speed, transform.parent);

            yield return new WaitForSeconds(interval);
        }

        _isAttacking = false;
        _attack4Counter = 0f;
        _crab.Animator.SetBool("ShootRandom", false);

        _crab.CurrentState = CrabState.Idle;
        _crab.CurrentCoroutine = null;
    }
}
