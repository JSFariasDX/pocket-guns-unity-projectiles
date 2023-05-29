using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected CanvasGroup popUp;
    [SerializeField] protected bool _isRepeatable = false;
    [SerializeField] protected TMPro.TextMeshProUGUI itemName;
    [SerializeField] protected TMPro.TextMeshProUGUI itemDescription;
    [SerializeField] protected Image buttonIcon;

    protected List<Player> players;
    protected TutorialManager tutorial;

    protected Player _currentPlayer;

    protected virtual void Start()
    {
        players = new();
        tutorial = FindObjectOfType<TutorialManager>();
    }

    public void SetName(string value)
    {
		if (!itemName) return;

        itemName.text = value.ToString();
    }

    public void SetDescription(string value)
    {
		if (!itemDescription) return;

        itemDescription.text = value.ToString();
    }

    public void SetRepeatable(bool value)
    {
        _isRepeatable = value;
    }

	public virtual void UpdateTexts()
	{

	}

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            if (!players.Contains(collision.GetComponentInParent<Player>()))
                players.Add(collision.GetComponentInParent<Player>());

			if (!collision.GetComponentInParent<Player>().inCombat)
			{
				SetIcon();
				popUp.alpha = 1;
				collision.GetComponentInParent<Player>().SetInteractable(this);
				UpdateTexts();
			}
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            collision.GetComponentInParent<Player>().SetInteractable(null);

            if (players.Contains(collision.GetComponentInParent<Player>()))
                players.Remove(collision.GetComponentInParent<Player>());

            if (players.Count == 0)
                popUp.alpha = 0;
            else
                SetIcon();
        }
    }

    public virtual void OnInteract(Player player)
    {
    }

    protected void SetIcon()
    {
		if (!buttonIcon) return;

        _currentPlayer = players[players.Count - 1];

		bool isUsingController = FindObjectOfType<PlayerInputController>();

		string buttonName;
		bool isGamepad;

		if (isUsingController)
		{
			buttonName = InputControlPath.ToHumanReadableString(_currentPlayer.GetInputController().GetInput().actions.FindAction("Interact").bindings[_currentPlayer.GetInputController().IsGamepad() ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
			InputControlPath.ToHumanReadableString(_currentPlayer.GetInputController().GetInput().actions.FindAction("Interact").bindings[_currentPlayer.GetInputController().IsGamepad() ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
			isGamepad = _currentPlayer.GetInputController().IsGamepad();
			print("<color=yellow>" + isGamepad + "</color>");
		}
		else
		{
			buttonName = InputControlPath.ToHumanReadableString(tutorial.control.FindAction("Interact").bindings[tutorial.isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
			InputControlPath.ToHumanReadableString(tutorial.control.FindAction("Interact").bindings[tutorial.isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
			isGamepad = tutorial.isGamepad;
		}

		if (isGamepad)
		{
			if (isUsingController)
			{
				for (int i = 0; i < tutorial.gamepadButtons.Count; i++)
				{
					if (tutorial.gamepadButtons[i].name == buttonName)
					{
						switch (_currentPlayer.GetInputController().GetControllerType())
						{
							case ControllerType.DualShock:
								buttonIcon.sprite = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
							case ControllerType.Xbox:
								buttonIcon.sprite = tutorial.gamepadButtons[i].XboxIcon;
								break;
							case ControllerType.Switch:
								buttonIcon.sprite = tutorial.gamepadButtons[i].SwitchIcon;
								break;
							case ControllerType.Keyboard:
								buttonIcon.sprite = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
							default:
								buttonIcon.sprite = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
						}
					}
					else
						continue;
				}
			}
			else
			{
				for (int i = 0; i < tutorial.gamepadButtons.Count; i++)
				{
					if (tutorial.gamepadButtons[i].name == buttonName)
					{
						switch (tutorial.type)
						{
							case ControllerType.DualShock:
								buttonIcon.sprite = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
							case ControllerType.Xbox:
								buttonIcon.sprite = tutorial.gamepadButtons[i].XboxIcon;
								break;
							case ControllerType.Switch:
								buttonIcon.sprite = tutorial.gamepadButtons[i].SwitchIcon;
								break;
							case ControllerType.Keyboard:
								buttonIcon.sprite = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
							default:
								buttonIcon.sprite = tutorial.gamepadButtons[i].PlayStationIcon;
								break;
						}
					}
					else
						continue;
				}
			}
		}
		else
		{
			for (int i = 0; i < tutorial.keyboard.Count; i++)
			{
				if (tutorial.keyboard[i].name == buttonName)
				{
					if (buttonIcon) buttonIcon.sprite = tutorial.keyboard[i].icon;
					break;
				}
				else
					continue;
			}
		}
	}
}
