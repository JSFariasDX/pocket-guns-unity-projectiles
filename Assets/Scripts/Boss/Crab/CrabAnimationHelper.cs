using UnityEngine;

public class CrabAnimationHelper : MonoBehaviour
{
    private Crab _crab;

    private void Awake()
    {
        _crab = GetComponentInParent<Crab>();
    }

    public void SetAboveWater(int value)
    {
        var boolean = value != 0 ? true : false;
        _crab.SetAboveWater(boolean);
    }

    public void SetIsFiringLaser(int value)
    {
        var boolean = value != 0 ? true : false;
        _crab.SetIsFiringLaser(boolean);
    }

    public void ActivateLaser(int value)
    {
        var boolean = value != 0 ? true : false;
        _crab.ActivateLaser(boolean);
    }

    public void RisingAttack()
    {
        _crab.RisingAttack();
    }

    public void Die()
    {
        _crab.Die();
    }
}
