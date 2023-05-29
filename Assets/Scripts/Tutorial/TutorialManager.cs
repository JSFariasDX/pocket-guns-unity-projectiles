using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public ControllerType type;
    public Player player;
    public Player playerPrefab;

    public List<KeyboardButons> keyboard = new List<KeyboardButons>();
    public List<GamepadButtons> gamepadButtons = new List<GamepadButtons>();

    [HideInInspector]
    public PlayerControls control;

    public bool isGamepad;

    public bool deviceSet;

    public TMP_SpriteAsset DSSprites;
    public TMP_SpriteAsset XboxSprites;
    public TMP_SpriteAsset SwitchSprites;
    public TMP_SpriteAsset KeyboardSprites;

    [Space(15)]
    public TextMeshProUGUI controllerName;

    private void Awake()
    {
        control = new PlayerControls();

        control.UI.Any.performed += ctx => DetectDevice(ctx);
        control.UI.Submit.performed += ctx => DetectDevice(ctx);
    }

    void DetectDevice(InputAction.CallbackContext ctx)
    {
        isGamepad = ctx.control.device is Gamepad;

        //string buttonName = InputControlPath.ToHumanReadableString(control.FindAction("Interact").bindings[isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        string buttonName = InputControlPath.ToHumanReadableString(ctx.action.bindings[isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        //print(buttonName);

        DetectControllerBrand(ctx.control.device.name);

        deviceSet = true;
    }

    void DetectControllerBrand(string name)
    {
        if (name.Contains("DualShock"))
        {
            type = ControllerType.DualShock;
            //print("is a DualShock");
        }
        else if (name.Contains("XInput"))
        {
            type = ControllerType.Xbox;
            //print("Is an Xbox Controller");
        }
        else if (name.Contains("Switch"))
        {
            type = ControllerType.Switch;
            //print("Is a Switch Controller");
        }
        else
        {
            type = ControllerType.Keyboard;
            //print("Is a keyboard or something else");
        }

        if (controllerName != null)
            controllerName.text = name;
    }

    private void OnEnable()
    {
        control.Enable();
    }

    private void OnDisable()
    {
        control.Disable();
    }
}

[System.Serializable]
public class KeyboardButons
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class GamepadButtons
{
    public string name;
    public Sprite PlayStationIcon;
    public Sprite XboxIcon;
    public Sprite SwitchIcon;
}
