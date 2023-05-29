using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class OpenBioengineerPanel : Interactable
{
    [Header("BioEngineer")]
    public GameObject optionsMenu;

    [SerializeField] GameObject optionsPanel;
    [SerializeField] public Button evolveButton;
    [SerializeField] TextMeshProUGUI evolveButtonText;
    [SerializeField] public Button dnaTradeButton;
    [SerializeField] TextMeshProUGUI dnaTradeButtonText;

    [SerializeField] string titleText = "Pocket Evolve";
    [TextArea, SerializeField] string subtitleText = "Do you wanna evolve your pocket?";

    [SerializeField] string pocketLessText;
    [TextArea, SerializeField] string pocketLessDescriptionText;

    [TextArea, SerializeField] string eggSubtitleText;
    [TextArea, SerializeField] string maxLevelSubtitleText;
    [TextArea, SerializeField] string minPocketsText;

    bool isEnabled;
    public GameObject currentSelected;
    Player player;
    [SerializeField] Pocket currentPocket;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SetName(titleText);
        SetDescription(subtitleText);
    }

	private void Update()
	{
        currentSelected = EventSystem.current.currentSelectedGameObject;
	}

	public override void UpdateTexts()
	{
		base.UpdateTexts();

        SetName(titleText);
        SetDescription(subtitleText);
        buttonIcon.gameObject.SetActive(true);

        if (player.currentPocket)
        {
            if (player.GetCurrentPocket().level <= 0)
            {
                evolveButtonText.text = eggSubtitleText;
            }
            else if (player.GetCurrentPocket().level >= 3)
            {
                evolveButtonText.text = maxLevelSubtitleText;
            }
            else
            {
                evolveButtonText.text = "Evolve Pocket";
            }
        }
        
        if (FindObjectOfType<UnlockedCharacters>().GetPocketsCount() <= 4)
		{
            dnaTradeButtonText.text = minPocketsText;
        }
        else
        {
            dnaTradeButtonText.text = "Trade pocket for DNA";
        }

    }

	public override void OnInteract(Player player)
    {
        this.player = player;
        if (!player.currentPocket)
        {
            SendMessageUpwards("OnBuyFail");
            return;
        }

        UpdateTexts();

        int pocketLevel = player.GetCurrentPocket().level;
        
        evolveButton.interactable = pocketLevel < 3 && pocketLevel > 0;
        dnaTradeButton.interactable = FindObjectOfType<UnlockedCharacters>().GetPocketsCount() > 4;

        currentPocket = player.currentPocket;

        optionsMenu.SetActive(true);

        EnableOptions(true);
    }

    public void EnableOptions(bool enable)
	{
        isEnabled = enable;

        if (enable)
            PauseManager.Instance.SimplePause();
        else
            PauseManager.Instance.SimpleResume();

        List<Player> player = GameplayManager.Instance.GetPlayers(false);
        foreach (var item in player)
        {
            item.GetInputController().SetMapInput(enable ? "UI" : "Gameplay");
        }

        Cursor.visible = enable;

        if (enable)
        {
            currentPocket.gameObject.layer = 24;
        }
        else
        {
            currentPocket.gameObject.layer = 0;
            currentPocket = null;
        }

        //CanvasGroup group = optionsPanel.GetComponent<CanvasGroup>();
        //group.alpha = enable ? 1 : 0;
        //group.interactable = enable ? true : false;
        optionsPanel.SetActive(enable);
        EventSystem.current.SetSelectedGameObject(evolveButton.gameObject);
    }
    
    public bool IsEnabled() { return isEnabled; }

    public void Hide()
	{
        optionsPanel.SetActive(false);
    }

    public Player GetPlayer() { return _currentPlayer; }
}