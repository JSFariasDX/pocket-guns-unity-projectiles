using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunDebugs : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] Image gunImage;
    [SerializeField] TextMeshProUGUI gunText;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gunImage.sprite = player.GetCurrentGun().sideSprite;
        gunImage.SetNativeSize();
        gunText.text = player.GetCurrentGun().GetDebugText();
    }
}
