using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tab : MonoBehaviour
{
    [SerializeField] MainMenuTabs tab = MainMenuTabs.Play;

    public void OnSelect()
    {
        return;
        MainMenu mm = GetComponentInParent<MainMenu>();
        mm.SelectTab(tab);
    }
}
