using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Unlockable : MonoBehaviour
{
    [SerializeField] string displayName;
    [SerializeField] int price;
    [SerializeField] bool forShow;
    bool isUnlocked = false;
    [SerializeField] bool unlock;

    [Header("Component")]
    public GameObject respectiveCharacter;
    public GameObject attributesScreen;

    [Header("UI")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI dashText;

    [Header("Special (Chartacter)")]
    public TextMeshProUGUI specialText;
    string description;

    [Header("Pocket")]
    [TextArea]
    public string specialDescription;

    bool isHighlighted = false;

    private void Start()
    {
        if (unlock) PlayerPrefs.SetInt(displayName, 1);

        CheckUnlocked();
        transform.Find("DisplayName").GetComponentInChildren<TextMeshProUGUI>().text = displayName;

        if (respectiveCharacter != null)
        {
            ShowAttributes();
        }

        attributesScreen.SetActive(false);

        //if (attributesScreen != null)
        //{
        //    //if (GetComponent<Toggle>().isOn)
        //    //{
        //    //    attributesScreen.SetActive(true);
        //    //}
        //    //else
        //    //{
        //    //    attributesScreen.SetActive(false);
        //    //}

        //    attributesScreen.SetActive(false);
        //}

        
    }

    private void Update()
    {
        if(attributesScreen != null)
            GetComponent<Toggle>().interactable = isUnlocked;

        //if (respectiveCharacter != null)
        //{
        //    if (respectiveCharacter.GetComponent<PlayerAttributes>() != null)
        //    {

        //        PlayerAttributes att = respectiveCharacter.GetComponent<PlayerAttributes>();

        //        switch (att.attributes)
        //        {
        //            case PlayerAttributes.SpecialAttribute.None:

        //                description = "PERK: None";

        //                break;
        //            case PlayerAttributes.SpecialAttribute.Enemies:

        //                description = "PERK: Kill 10 enemies to restore 10% of the total health";

        //                break;
        //            case PlayerAttributes.SpecialAttribute.Health:

        //                description = "PERK: When health is lower than 10%, all attributes are increased";

        //                break;
        //            case PlayerAttributes.SpecialAttribute.Translate:

        //                description = "PERK: Grants the ability to read alien transcrips";

        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        if(specialText != null)
            specialText.text = description;

        if (Gamepad.current != null)
        {
            if (!isHighlighted) attributesScreen.SetActive(false);

            if (EventSystem.current.currentSelectedGameObject != gameObject)
                attributesScreen.SetActive(false);

            print(Gamepad.current);
        }
    }

    public void Unlock()
    {
        if (GlobalData.Instance.hasPGUNSBalance(price))
        {
            GlobalData.Instance.RemovePGUNS(price);
            PlayerPrefs.SetInt(displayName, 1);
            SetUnlocked(true);
        }
    }

    public void CheckUnlocked()
    {
        bool isUnlocked = PlayerPrefs.GetInt(displayName, 0) > 0;
        if (isUnlocked)
        {
            SetUnlocked(true);
        }
    }

    void SetUnlocked(bool value)
    {
        isUnlocked = value;
        if (value)
        {
            if (!forShow)
            {
                transform.Find("Lock").gameObject.SetActive(false);
                GetComponent<Toggle>().interactable = true;
            }
        }
    }

    void ShowAttributes()
    {
        if (respectiveCharacter.GetComponent<PlayerAttributes>() != null)
        {

            PlayerAttributes attributes = respectiveCharacter.GetComponent<PlayerAttributes>();

            healthText.text = "Health: " + attributes.lifePercentage.ToString() + "%";
            speedText.text = "Speed: " + attributes.speedPercentage.ToString();
            dashText.text = "Dash: " + attributes.dashPercentage.ToString();
        } else if(respectiveCharacter.GetComponent<Pocket>() != null)
        {
            Health petHealth = respectiveCharacter.GetComponent<Health>();

            healthText.text = "HEALTH: " + petHealth.maxValue.ToString();
            speedText.text = "SPECIAL: " + specialDescription;
        }
    }

    public void ShowHideAttributes(bool show)
    {
        if (!isUnlocked) return;

        //if (!GetComponent<Toggle>().IsInteractable()) return;

        //if (!GetComponent<Toggle>().isOn)
        //{
        //    attributesScreen.SetActive(show);
        //}

        attributesScreen.SetActive(show);

        isHighlighted = show;
    }

    public void ChangeVilibility(bool toggle)
    {
        attributesScreen.SetActive(toggle);
    }
}
