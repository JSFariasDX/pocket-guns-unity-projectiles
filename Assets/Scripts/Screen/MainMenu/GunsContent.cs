using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GunsContent : MonoBehaviour
{
    [SerializeField] GameObject elementToFocus;

    private void OnEnable()
    {
        if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(elementToFocus);
        }
    }
}
