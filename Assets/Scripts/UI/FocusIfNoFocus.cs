using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusIfNoFocus : MonoBehaviour
{
    void Update()
    {
        if (PauseManager.Instance.IsGamePaused())
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }
    }
}
