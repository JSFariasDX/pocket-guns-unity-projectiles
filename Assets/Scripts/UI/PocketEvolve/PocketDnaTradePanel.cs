using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PocketDnaTradePanel : MonoBehaviour
{
    [SerializeField] OpenBioengineerPanel openBioengineerPanel;
    [SerializeField] PocketAttributesPanel pocketPanel;
    [SerializeField] GameObject tradeForDnaCanvas;
    [SerializeField] Button tradeButton;
    [SerializeField] TextMeshProUGUI dnaValueText;
    [SerializeField] TextMeshProUGUI currentDnaText;
    bool isEnabled;

    int dnaValue;
    Pocket currentPocket;
    UnlockedCharacters saveSystem;

    [SerializeField] Image confirmBar;

    [Header("Conrfirm bar")]
    [SerializeField] float barSpeed = 2;
    float value = 0;

    UnlockedCharacters GetSaveSystem()
	{
        if (!saveSystem) saveSystem = FindObjectOfType<UnlockedCharacters>();
        return saveSystem;
	}

    private void Update()
    {
        if (!EventSystem.current.currentSelectedGameObject)
        {
            tradeButton.Select();
        }

        if (currentPocket.GetPlayer().entryPanel.SubmitHeld())
        {
            if (value < 1) value += Time.unscaledDeltaTime * barSpeed;
        }
        else
        {
            if (value > 0) value -= Time.unscaledDeltaTime * (barSpeed / 2);
        }

        if (value > 1)
        {
            Trade();
            value = 0;
        }

        confirmBar.fillAmount = value;
    }

    void SetupValues(Pocket pocket)
	{
        if (!pocket) return;
        
        currentPocket = pocket;
        pocketPanel.SetPlayer(openBioengineerPanel.GetPlayer());
        pocketPanel.Setup(pocket, null, null, false);

        dnaValue = 1;

        dnaValueText.text = "x" + dnaValue;
        currentDnaText.text = "You have " + PlayerPrefs.GetInt("DNA") + " DNA";
    }

    public void Open()
	{
        openBioengineerPanel.Hide();
        EnableTradeMenu(true, openBioengineerPanel.GetPlayer().GetCurrentPocket());
    }

    public void Close()
	{
        EnableTradeMenu(false, null);
        openBioengineerPanel.EnableOptions(true);
        EventSystem.current.SetSelectedGameObject(openBioengineerPanel.dnaTradeButton.gameObject);
    }

    public void Trade()
	{
        int currentDna = PlayerPrefs.GetInt("DNA");
        PlayerPrefs.SetInt("DNA", currentDna + dnaValue);

        GetSaveSystem().RemovePocket(currentPocket);

        Close();
        openBioengineerPanel.UpdateTexts();
        openBioengineerPanel.EnableOptions(false);

        currentPocket.GetPlayer().entryPanel.SetSubmitHeld(false);
    }

    public void DeletePocketInstances(Pocket pocket)
	{
        foreach(Pocket p in FindObjectsOfType<Pocket>())
		{
            if (p.pocketName.Equals(pocket.pocketName) && p.level > 0)
			{
                Destroy(p.gameObject);
			}
		}

        Destroy(pocket.gameObject);
	}

    public void EnableTradeMenu(bool enable, Pocket pocket)
    {
        isEnabled = enable;

        SetupValues(pocket);
        //CanvasGroup group = GetComponent<CanvasGroup>();
        //group.alpha = enable ? 1 : 0;
        //group.interactable = enable ? true : false;
        tradeForDnaCanvas.SetActive(enable);
        tradeButton.Select();
        //EventSystem.current.SetSelectedGameObject(tradeButton.gameObject);
    }

    public bool IsEnabled() { return isEnabled; }

    public void TradePressed(bool pressed)
    {
        Player player = currentPocket.GetPlayer();
        player.entryPanel.SetSubmitHeld(pressed);
    }
}
