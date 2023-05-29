using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    ControllerType controllerType;
	[SerializeField] Player player;
    [SerializeField] PlayerEntryPanel playerEntryPanel;
	[SerializeField] PlayerInput input;
	PlayerEntryManager manager;
	InputControl control;
	bool isGamepad;
	public string currentInputMapping;

	public bool isRumbling = false;

	Vector2 motorSpeeds;
	private void Start()
	{
		playerEntryPanel = GetComponent<PlayerEntryPanel>();
		input = GetComponent<PlayerInput>();
		manager = FindObjectOfType<PlayerEntryManager>();

		
	}

	public bool IsGamepad()
	{
		return isGamepad;
	}

	public Player GetPlayer() { return player; }

	public void SetPlayer(Player player)
	{
		this.player = player;
		player.playerIndex = input.playerIndex;
		player.SetInputController(this);
	}

	public void Rumble(float lowFrequency, float highFrequency)
    {
		if (isGamepad)
		{
			if (motorSpeeds.magnitude > 0) isRumbling = true;
			else isRumbling = false;

			input.GetDevice<Gamepad>().SetMotorSpeeds(lowFrequency, highFrequency);

			//if (lowFrequency > 0 && highFrequency > 0)
			//{
			//	if(lowFrequency > motorSpeeds.x || highFrequency > motorSpeeds.y)
			//		input.GetDevice<Gamepad>().SetMotorSpeeds(lowFrequency, highFrequency);
			//}
			//else
			//{
			//	input.GetDevice<Gamepad>().SetMotorSpeeds(lowFrequency, highFrequency);
			//}

			motorSpeeds = new Vector2(lowFrequency, highFrequency);
		}
	}

	IEnumerator RumbleController(float lowFrequency, float highFrequency, float duration)
    {
		if (isGamepad)
			input.GetDevice<Gamepad>().SetMotorSpeeds(lowFrequency, highFrequency);

		yield return new WaitForSecondsRealtime(duration);

		if (isGamepad)
			input.GetDevice<Gamepad>().SetMotorSpeeds(0, 0);
	}

	public PlayerInput GetInput()
	{
		return input;
	}

	public PlayerEntryPanel GetPlayerEntryPanel()
	{
		return playerEntryPanel;
	}

	public bool IsGamePaused()
	{
		return PauseManager.Instance.IsGamePaused();
	}

	#region Player Controller
	public void OnWalk(InputAction.CallbackContext ctx)
	{
		DefineController(ctx);

		if (!player) return;
		if (player.GetIsDead()) return;

		if (ctx.performed) player.HandleWalkInput(ctx);
		else if (ctx.canceled) player.HandleResetWalk(ctx);
	}

	public void OnAim(InputAction.CallbackContext ctx)
	{
		DefineController(ctx);

		if (!player) return;
		if (player.GetIsDead()) return;

		if (ctx.performed) player.GetAim().OnAimInput(ctx);
		else if (ctx.canceled) player.GetAim().ResetAim(ctx);
	}

	public void OnSpecial(InputAction.CallbackContext ctx)
	{
		if (!player) return;
		if (player.GetIsDead()) return;

		if (ctx.performed) player.GetCurrentPocket().GetSpecial().TryActivate(ctx);
	}

	public void OnFire(InputAction.CallbackContext ctx)
	{
		if (!player) return;
		if (player.GetIsDead()) return;

		player.GetCurrentGun().SetShootHold(ctx.action.IsPressed());
	}

	public void OnReload(InputAction.CallbackContext ctx)
    {
		if (!player) return;
		if (player.GetIsDead()) return;

		player.GetCurrentGun().Reload();
    }

	public void OnChangeWeapon(InputAction.CallbackContext ctx)
	{
		if (!player) return;
		if (player.GetIsDead()) return;

		if (ctx.performed) player.HandleChangeWeaponInput(ctx);
	}

	public void OnDash(InputAction.CallbackContext ctx)
	{
		if (!player) return;
		if (player.GetIsDead()) return;

		if (ctx.started) player.HandleDashInput(ctx);
	}

	public void OnChangePocket(InputAction.CallbackContext ctx)
	{
		if (!player) return;
		if (player.GetIsDead()) return;

		if (ctx.started)
		{
			player.SwitchToNextPocket();
		}
	}

	public void OnMousePosition(InputAction.CallbackContext ctx)
	{
		if (!player) return;
		if (player.GetIsDead()) return;

		player.aim.OnMouseMoved(ctx);
	}

	public void OnMap(InputAction.CallbackContext ctx)
	{
		if (!player) return;
		if (player.GetIsDead()) return;
	}

	public void OnPocketMenu(InputAction.CallbackContext ctx)
	{
		return; // Desativado sob ordens do lincoln

		if (!player) return;
		if (player.GetIsDead()) return;

		if (manager.entries.Count == 1)
		{
			if (ctx.started && !IsGamePaused()) player.GetPocketMenu().OpenPocketMenu(ctx);
			else if (ctx.canceled) player.GetPocketMenu().ClosePocketMenu(ctx);
		}
	}

	public void OnInteract(InputAction.CallbackContext ctx)
	{
		if (!player) return;
		if (player.GetIsDead()) return;

		if (ctx.performed) player.HandleInteract(ctx);
	}

	public void OnPause(InputAction.CallbackContext ctx)
	{
		if (!player) return;

		if (ctx.started)
		{
			if (PauseManager.Instance.ReturnPauseDelay() > 0) return;

			PauseManager.Instance.OnPauseInput(ctx, player);
			player.OnPauseInput(ctx);
		}
	}

	#endregion

	#region PlayerEntry
	public void OnVerticalNavigate(InputAction.CallbackContext ctx)
	{
		if (ctx.performed) playerEntryPanel.VerticalNavigate(ctx);
	}

	public void OnHorizontalNavigate(InputAction.CallbackContext ctx)
	{
		if (ctx.performed) playerEntryPanel.HorizontalNavigate(ctx);
	}

	public void OnNavigate(InputAction.CallbackContext ctx)
    {
		if (ctx.performed)
        {
			DefineController(ctx);
			playerEntryPanel.Navigate(ctx);
        }
    }

	public void OnSubmit(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			DefineController(ctx);
			playerEntryPanel.Confirm(ctx);
			playerEntryPanel.SetSubmitHeld(true);
		}
		else if(ctx.canceled)
			playerEntryPanel.SetSubmitHeld(false);
	}

	public void OnCancel(InputAction.CallbackContext ctx)
	{
		if (ctx.performed) playerEntryPanel.Cancel(ctx);
	}

	#endregion

	#region UI

	public void OnUIPause(InputAction.CallbackContext ctx)
	{
		if (ctx.started)
		{
			if (PauseManager.Instance.ReturnPauseDelay() > 0) return; 

			PauseManager.Instance.OnPauseInput(ctx, player);
			player.OnPauseInput(ctx);
		}
	}

	public void OnUiSubmit(InputAction.CallbackContext ctx)
	{
	}

	public void OnNextAttributeTab(InputAction.CallbackContext ctx)
	{
		if (!player) return;

		if (ctx.started) PauseManager.Instance.NextAttributeTab();
	}

	public void OnPreviousAttributeTab(InputAction.CallbackContext ctx)
	{
		if (!player) return;

		if (ctx.started) PauseManager.Instance.PreviousAttributeTab();
	}

	public void OnBack(InputAction.CallbackContext ctx)
	{
		if (!player) return;

		if (ctx.performed)
		{
			PauseManager.Instance.OnBack();
		}
	}

	//public void OnUiHorizontalNavigate(InputAction.CallbackContext ctx)
	//{
	//	if (!player) return;

	//	if (ctx.performed)
	//		PauseManager.Instance.OnAttributeHorizontalNavigate(ctx, player);
	//}

	//public void OnUiVerticalNavigate(InputAction.CallbackContext ctx)
	//{
	//	if (!player) return;

	//	if (ctx.performed) 
	//		PauseManager.Instance.OnAttributeVerticalNavigate(ctx, player);
	//}

	public void OnUINavigate(InputAction.CallbackContext ctx)
    {
		if (!player) return;

		if (ctx.performed) PauseManager.Instance.OnAttributeNavigate(ctx, player);

	}

	#endregion

	// Tools
	public void SetMapInput(string mapName)
	{
		input.SwitchCurrentActionMap(mapName);
		currentInputMapping = input.currentActionMap.name;
	}

	public static void UpdateAllPlayersMapInputs(string map)
	{
		foreach (Player player in GameplayManager.Instance.GetPlayers(false))
		{
			player.GetInputController().SetMapInput(map);
		}
	}

	void DefineController(InputAction.CallbackContext ctx)
	{
		//if (isControllerDefined) return;

		isGamepad = ctx.control.device is Gamepad;
		DetectControllerBrand(ctx.control.device.name);
	}

	public ControllerType GetControllerType() { return controllerType; }

	bool isControllerDefined;
	void DetectControllerBrand(string name)
	{
		if (name.Contains("DualShock"))
		{
			controllerType = ControllerType.DualShock;
		}
		else if (name.Contains("XInput"))
		{
			controllerType = ControllerType.Xbox;
		}
		else if (name.Contains("Switch"))
		{
			controllerType = ControllerType.Switch;
		}
		else
		{
			controllerType = ControllerType.Keyboard;
		}
	}
}

public enum ControllerType
{
	DualShock, Xbox, Switch, Keyboard
}