using Assets.SimpleLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSwitch : MonoBehaviour
{
    [SerializeField] Language startLanguage;
    void Start()
    {
        LocalizationManager.Read();

        LocalizationManager.AutoLanguage();
    }
}

public enum Language
{
    English,
}
