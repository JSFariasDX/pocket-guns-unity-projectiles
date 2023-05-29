using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletsSpecialBars : MonoBehaviour
{
    public SpecialBar specialBar;
    public BulletsIndicator bulletsBar;

    public void SetPocket(Pocket pocket)
	{
        specialBar.Setup(pocket.gameObject);
	}

    public void SetGun(Gun gun)
	{
        bulletsBar.gun = gun;
	}
}
