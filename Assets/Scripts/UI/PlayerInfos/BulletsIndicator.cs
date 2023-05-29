using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletsIndicator : MonoBehaviour
{
    Image bar;
    public Gun gun;
    // Start is called before the first frame update
    void Start()
    {
        bar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gun != null)
            bar.fillAmount = (float)gun.currentBullets / (float)gun.maxBullets;
    }
}
