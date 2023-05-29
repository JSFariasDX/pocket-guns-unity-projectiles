using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
	public bool isTutorial;
    int currentGuis;
	public Transform leftPlayerGuiContainer;
	public Transform rightPlayerGuiContainer;
    public List<Player> characterPrefabs;
    public List<Pocket> pocketsPrefabs;

    Player spawnedPlayer;
    Pocket spawnedPocket;
    
	Screens screen;

    public CameraManager camManager;
    public Player player;

    private void Start()
    {
		//if (ScreenManager.currentScreen == Screens.Tutorial)
		if (isTutorial)
		{
			//TutorialSpawnPlayer(Vector2.zero);

   //         spawnedPlayer = FindObjectOfType<Player>(true);

   //         spawnedPlayer.startingGunsPrefabs.RemoveAt(1);
		}
		else
		{
            if (controls != null)
                controls.Disable();
		}
    }

    private void Update()
    {
        if (isTutorial)
        {
            if (controls != null && spawnedPlayer.GetCurrentGun())
            {
                spawnedPlayer.GetCurrentGun().SetShootHold(controls.Gameplay.Fire.IsPressed());
            }
        }
    }

    public Transform GetPlayerGuiParent()
	{
        currentGuis++;

        if (currentGuis <= 2)
		{
            return leftPlayerGuiContainer;
		}
		else
		{
            return rightPlayerGuiContainer;
		}
	}

    public void TutorialSpawnPlayer(Vector2 spawnPosition)
	{
        GetComponent<GameplayManager>().SpawnPlayers(Vector2Int.zero);

        //camManager = FindObjectOfType<CameraManager>(true);

        //spawnedPlayer = Instantiate(characterPrefabs[0]);

        //player = spawnedPlayer.GetComponent<Player>();

        //spawnedPlayer.SetPosition(spawnPosition);
        //spawnedPocket = spawnedPlayer.StartPocket(pocketsPrefabs[0]);
        //SetupSinglePlayerInput(spawnedPlayer, spawnedPocket);

        //GameplayManager.Instance.players.Add(spawnedPlayer.GetComponent<Player>());

        ////FindObjectOfType<CameraManager>().SetSingleTargetCameraFocus(spawnedPlayer.transform);

        //player.StartPocket(spawnedPocket);
        //player.SetPlayerIndex(0);
        //player.SetCamTarget(camManager.AddTarget(transform));
	}

    PlayerControls controls;
	void SetupSinglePlayerInput(Player player, Pocket pocket)
    {
        // Setup controls

        controls = new PlayerControls();
        controls.Gameplay.Enable();
        controls.Gameplay.Walk.performed += player.HandleWalkInput;
        controls.Gameplay.Walk.canceled += player.HandleResetWalk;
        controls.Gameplay.Dash.started += player.HandleDashInput;
        controls.Gameplay.ChangeWeapon.performed += player.HandleChangeWeaponInput;
        controls.Gameplay.Interact.performed += player.HandleInteract;

        // Pocket
        controls.Gameplay.Special.performed += pocket.GetSpecial().TryActivate;

        // Aim
        controls.Gameplay.Aim.performed += player.GetAim().OnAimInput;
        controls.Gameplay.Aim.canceled += player.GetAim().ResetAim;
        controls.Gameplay.MousePosition.performed += ctx => player.GetAim().mouseInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.MousePosition.canceled += ctx => player.GetAim().mouseInput = Vector2.zero;

        // Pause
        controls.Gameplay.Pause.performed += TutorialPauseInput;
        controls.UI.Cancel.performed += TutorialPauseInput;
        controls.UI.Pause.performed += TutorialPauseInput;
    }

    public void TutorialPauseInput(InputAction.CallbackContext ctx)
    {
        if (GameplayManager.Instance.IsCutsceneInProgress || !isTutorial)
            return;

        PauseManager.Instance.OnPauseInput(ctx, FindObjectOfType<Player>());

        if (PauseManager.Instance.IsGamePaused())
        {
            controls.UI.Enable();
            controls.Gameplay.Disable();
        }
        else
        {
            controls.UI.Disable();
            controls.Gameplay.Enable();
        }
    }
}
