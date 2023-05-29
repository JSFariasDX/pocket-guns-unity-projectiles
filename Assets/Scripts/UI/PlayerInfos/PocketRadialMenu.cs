using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PocketRadialMenu : MonoBehaviour
{
    public Player player;
    public Transform selector;
    public PocketRadialPanel panel;
    public Image characterIcon;

    [Header("Slots")]
    public PocketRadialSlot currentSlot;
    public PocketRadialSlot previousSlot;
    public PocketRadialSlot selectedSlot;
    public List<PocketRadialSlot> slots = new List<PocketRadialSlot>();

    private Vector2 normalizedCursorPosition;

    private int currentSelection;
    private int previousSelection;

    private TutorialManager tutorialManager; // Used for gamepad check

    public bool isOpen = false;

    private TutorialHelper _eggHelper;

	private void Start()
	{
        tutorialManager = FindObjectOfType<TutorialManager>();

        selectedSlot = slots[0];
        selectedSlot.playerIsUsing = true;

        name = gameObject.name + " " + player.characterName;
        player.pocketMenu = this;
        transform.parent.parent = null;

        GetComponentInParent<Canvas>().worldCamera = Camera.main;
        GetComponentInParent<Canvas>().sortingLayerName ="UI";
        _eggHelper = FindObjectOfType<TutorialHelper>(true);

        GetComponent<CanvasGroup>().alpha = 0;
        Close();
    }

    private void OnDestroy()
    {
        //controls.Gameplay.PocketMenu.started -= OpenPocketMenu;
        //controls.Gameplay.PocketMenu.canceled -= ClosePocketMenu;
        //controls.Gameplay.Disable();
    }

    private void Update()
	{
        if(player)
            SelectionController();

        //UpdateUi();

        GetComponent<CanvasGroup>().alpha = isOpen ? 1 : 0;
    }

    private void SelectionController()
	{
        currentSelection = GetSlotIndex();
        if (currentSelection != previousSelection)
		{
            previousSlot = slots[previousSelection];
            previousSlot.Deselect();
            previousSelection = currentSelection;

            currentSlot = slots[currentSelection];
            currentSlot.Select();

            if (currentSlot.pocket)
			{
                selector.gameObject.SetActive(true);
                selector.transform.position = currentSlot.pocketSpriteRenderer.transform.position;
                if (currentSlot.pocket.pocketType == PetType.Egg)
				{
                    panel.gameObject.SetActive(false);
				}
				else
				{
                    panel.gameObject.SetActive(true);
                    panel.UpdateUi(currentSlot.pocket);
				}
			}
			else
			{
                panel.gameObject.SetActive(false);
                selector.gameObject.SetActive(false);
            }
        }
    }

	private int GetSlotIndex()
	{
        Vector2 aimPos = Camera.main.WorldToScreenPoint(player.aim.transform.position);
        normalizedCursorPosition = new Vector2(aimPos.x - Screen.width / 2, aimPos.y - Screen.height / 2);

        float angle = Mathf.Atan2(normalizedCursorPosition.y, normalizedCursorPosition.x) * Mathf.Rad2Deg;

        angle = (angle + 360) % 360;
        int selection = (int)angle / 45;

        return selection;
	}

    public void UpdateUi()
	{
        foreach (PocketRadialSlot slot in slots)
        {
            slot.Deselect();
            slot.SetEmpty();
        }

        for (int i = 0; i < player.pockets.Count; i++)
		{
            slots[i].Setup(player.pockets[i]);
        }

        if (currentSlot)
        {
            currentSlot.Select();
        }
    }

    public void OpenPocketMenu(InputAction.CallbackContext ctx)
	{
        if (GameplayManager.Instance.IsCutsceneInProgress)
            return;

        if (_eggHelper.IsHatchingEgg)
            return;
        
        if (PauseManager.Instance.IsGamePaused() || PauseManager.Instance.IsPauseMenuActive())
            return;

        if (ScreenManager.currentScreen != Screens.Tutorial && !player.isDashing && !player.currentPocket.GetComponent<Special>().IsActivated())
        {
            characterIcon.sprite = player.characterIcon;
            //player.aim.transform.position = Camera.main.ScreenToWorldPoint(transform.position + Vector3.right * 5);
            player.aim.GetComponent<SpriteRenderer>().sprite = null;
            PauseManager.Instance.SimplePause();
            UpdateUi();
            isOpen = true;

            slots[0].transform.parent.gameObject.SetActive(true);

            //gameObject.SetActive(true);
            //GetComponent<CanvasGroup>().alpha = 1;
        }
	}

    public void ClosePocketMenu(InputAction.CallbackContext ctx)
	{
        if (GameplayManager.Instance.IsCutsceneInProgress)
            return;
            
        if (_eggHelper.IsHatchingEgg)
            return;
        
        if (PauseManager.Instance.IsPauseMenuActive())
            return;

        player.aim.SetAimState(AimStates.Idle);

        if (slots[currentSelection].pocket)
		{
            if (selectedSlot != null)
                selectedSlot.playerIsUsing = false;

            selectedSlot = currentSlot;
            player.SetCurrentPocket(currentSelection);
            currentSlot.playerIsUsing = true;
        }
        Close();
    }

    public void Close()
    {
        PauseManager.Instance.SimpleResume();

        slots[0].transform.parent.gameObject.SetActive(false);

        //gameObject.SetActive(false);
        //GetComponent<CanvasGroup>().alpha = 0;
        isOpen = false;
    }
}
