using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabLaserAttack : MonoBehaviour
{
    public bool IsFiringLaser { get; set; }

    [Header("Attack Settings")]
    [SerializeField] private Laser laser;
    [SerializeField] private float delay;
    [SerializeField] private float interval;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip attackClip;

    private Crab _crab;

    private void Awake()
    {
        _crab = GetComponentInParent<Crab>();
    }

    private void Start()
    {
        ActiveLaser(false);
    }

    public void ActiveLaser(bool value)
    {
        laser.ActiveLaser(value);

        if (value)
            _crab.AudioSource.PlayOneShot(attackClip);
    }

    public IEnumerator LaserAttack()
    {
        _crab.Animator.SetBool("Dive", false);
        _crab.Animator.SetBool("Move", false);

        yield return new WaitForSeconds(delay);
        IsFiringLaser = true;
        _crab.Animator.SetTrigger("Laser");

        yield return new WaitUntil(() => !IsFiringLaser);
        laser.ActiveLaser(false);

        yield return new WaitForSeconds(interval);
        _crab.CurrentState = CrabState.Idle;
        _crab.CurrentCoroutine = null;
    }
}
