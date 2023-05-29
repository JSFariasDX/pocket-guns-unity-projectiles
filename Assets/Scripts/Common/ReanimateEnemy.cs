using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReanimateEnemy : MonoBehaviour
{
    [SerializeField] private float reanimationTime;

    private float _timer;
    private bool _isReanimated;

    private Behaviour _behaviour;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

        _behaviour = GetComponent<EnemyBehaviourController>();

        if (_behaviour == null)
            _behaviour = GetComponent<WallSpiderController>();
    }

    private void Update()
    {
        if (_timer <= reanimationTime)
        {
            _timer += Time.deltaTime;
            return;
        }

        Reanimate();
    }

    private void Reanimate()
    {
        if (_isReanimated)
            return;
        
        _animator.SetTrigger("Reanimate");
        _behaviour.enabled = true;

        var health = GetComponent<Health>();
        health.SetHealth(health.GetMaxHealth());

        _isReanimated = true;
    }
}
