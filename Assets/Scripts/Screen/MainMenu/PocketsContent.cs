using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PocketsContent : MonoBehaviour
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
