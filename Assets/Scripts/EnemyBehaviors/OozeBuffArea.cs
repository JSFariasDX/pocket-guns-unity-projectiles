using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OozeBuffArea : MonoBehaviour
{
    [SerializeField] private GameObject buffAreaObject;
    [SerializeField, Range(0f, 5f)] private float buffMultiplier;
    [SerializeField] private SpriteRenderer areaSpriteRenderer;
    [SerializeField] private float blendChangeDuration;

    private const string GHOST_BLEND = "_GhostBlend";
    private const float MAX_BLEND = 1f;
    private const float MIN_BLEND = 0.7f;

    private float _blendChangeTimer;
    private bool _reduceBlend;

    private EnemyBehaviourController _controller;

    private void Awake()
    {
        buffAreaObject.SetActive(false);

        _controller = GetComponentInParent<EnemyBehaviourController>();

        _blendChangeTimer = 0f;
        _reduceBlend = false;

        areaSpriteRenderer.material.SetFloat(GHOST_BLEND, MIN_BLEND);
    }

    private void Update()
    {
        if (!_controller.isAggressive)
            return;

        if (!buffAreaObject.activeInHierarchy)
            buffAreaObject.SetActive(true);

        float newBlend;

        if (!_reduceBlend)
        {
            newBlend = Mathf.Lerp(MIN_BLEND, MAX_BLEND, _blendChangeTimer / blendChangeDuration);
            _blendChangeTimer += Time.deltaTime;

            areaSpriteRenderer.material.SetFloat(GHOST_BLEND, newBlend);

            if (_blendChangeTimer > blendChangeDuration)
            {
                _reduceBlend = !_reduceBlend;
                areaSpriteRenderer.material.SetFloat(GHOST_BLEND, MAX_BLEND);
                return;
            }

            return;
        }

        newBlend = Mathf.Lerp(MIN_BLEND, MAX_BLEND, _blendChangeTimer / blendChangeDuration);
        _blendChangeTimer -= Time.deltaTime;

        areaSpriteRenderer.material.SetFloat(GHOST_BLEND, newBlend);

        if (_blendChangeTimer < 0f)
        {
            _reduceBlend = !_reduceBlend;
            areaSpriteRenderer.material.SetFloat(GHOST_BLEND, MIN_BLEND);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent<Enemy>(out Enemy enemy))
            return;
        
        enemy.ActivateOozeBuff(buffMultiplier);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<Enemy>(out Enemy enemy))
            return;
        
        enemy.DeactivateOozeBuff(1f);
    }
}
