using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinisterShadowAnimationHelper : MonoBehaviour
{
    SinisterShadow _sinisterShadow;

    void Awake()
    {
        _sinisterShadow = GetComponentInParent<SinisterShadow>();
    }

    public void WakeUp()
    {
        _sinisterShadow.WakeUp();
    }

    public void Dive()
    {
        _sinisterShadow.SetDive(true);
    }

    public void Emerge()
    {
        _sinisterShadow.SetDive(false);
    }

    public void SetEyeTrackingTrue()
    {
        _sinisterShadow.SetEyeTracking(true);
    }

    public void SetEyeTrackingFalse()
    {
        _sinisterShadow.SetEyeTracking(false);
    }

    public void SetIsPerformingTrue()
    {
        _sinisterShadow.SetIsAttacking(true);
    }

    public void SetIsPerformingFalse()
    {
        if(_sinisterShadow.HowManyIllusions() < 1)
            _sinisterShadow.SetIsAttacking(false);
    }

    public void SetIsVisibleTrue()
    {
        _sinisterShadow.beVisible = true;
    }

    public void SetIsVisibleFalse()
    {
        _sinisterShadow.beVisible = false;
    }

    public void SetSpikeCollitionTrue()
    {
        _sinisterShadow.SpikeCollision(true);
    }

    public void SetSpikeCollisionFalse()
    {
        _sinisterShadow.SpikeCollision(false);
    }
}
