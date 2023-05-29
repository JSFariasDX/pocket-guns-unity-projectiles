using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinGui : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinText;
    [Header("Keys")]
    [SerializeField] List<Image> keyIcons = new List<Image>();

    void Update()
    {
        coinText.text = GameplayManager.Instance.CurrentCoins.ToString("000");
    }

    public void ShowKey(Sprite icon)
    {
        for (int i = 0; i < keyIcons.Count; i++)
        {
            if(keyIcons[i].gameObject.activeSelf == false)
            {
                keyIcons[i].sprite = icon;
                keyIcons[i].gameObject.SetActive(true);
                break;
            }
        }
    }

    public void RemoveKeys()
    {
        foreach (var item in keyIcons)
        {
            item.sprite = null;
            item.gameObject.SetActive(false);
        }
    }
}
