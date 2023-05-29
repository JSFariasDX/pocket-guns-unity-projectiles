using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FocusOnEnable : MonoBehaviour
{
    [SerializeField] GameObject elementToFocus;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(elementToFocus);
    }
}
