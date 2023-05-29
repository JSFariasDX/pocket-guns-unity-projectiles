using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EvolvePocketMenu : MonoBehaviour
{
    [SerializeField] OpenBioengineerPanel openBioengineerPanel;
    [SerializeField] Pocket currentPocket;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI dnaText;
    [SerializeField] TextMeshProUGUI dnaCostText;
    [SerializeField] TextMeshProUGUI pocketCostText;
    [SerializeField] Image pocketCostIcon;

    [SerializeField] GameObject pocketMenuCanvas;
    [SerializeField] PocketAttributesPanel evolvingPocketPanel;
    [SerializeField] PocketAttributesPanel evolvedPocketPanel;

    [SerializeField] Button evolveButton;
    [SerializeField] Image confirmBar;

    int dnaCost;
    int pocketCost;
    bool isEnabled;

    public bool cheated = false;

    UnlockedCharacters saveSystem;

    [Header("Conrfirm bar")]
    [SerializeField] Color barColor;
    [SerializeField] float barSpeed = 2;
    float value = 0;
    bool blinking = false;
    
    UnlockedCharacters GetSaveSystem()
	{
        if (!saveSystem)
		{
            saveSystem = FindObjectOfType<UnlockedCharacters>();
        }
        return saveSystem;
    }

    private void Awake()
	{
        if(cheated)
            PlayerPrefs.SetInt("DNA", 0);
    }

	private void Update()
	{
		if (!EventSystem.current.currentSelectedGameObject)
		{
            evolveButton.Select();
		}

        if (PlayerPrefs.GetInt("DNA") > 0)
        {
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
                Evolve();
                value = 0;
            }

            confirmBar.fillAmount = value;
        } 
        else if (currentPocket.GetPlayer().entryPanel.SubmitHeld() && !blinking)
        {
            StartCoroutine(BlinkBar());
        }
	}

	public void Open()
	{
		EnablePocketMenu(true, openBioengineerPanel.GetPlayer().GetCurrentPocket());
        openBioengineerPanel.Hide();
        EventSystem.current.SetSelectedGameObject(evolveButton.gameObject);
    }

    public void Close()
    {
        EnablePocketMenu(false, null);
        openBioengineerPanel.EnableOptions(true);
        EventSystem.current.SetSelectedGameObject(openBioengineerPanel.evolveButton.gameObject);
    }

    void SetupValues(Pocket pocket)
    {
        if (!pocket) return;

        currentPocket = pocket;

        titleText.text = "Are you sure you wanna evolve " + pocket.pocketName + "?";

        dnaCost = pocket.level * 7;
        pocketCost = pocket.level;

        dnaText.text = "You have " + GetCurrentDna().ToString() + " DNA";
        dnaCostText.text = "x" + dnaCost;
        pocketCostText.text = "x" + pocketCost;

        evolveButton.interactable = GetCurrentDna() >= dnaCost;

        pocketCostIcon.sprite = pocket.GetComponent<PocketDisplayInformations>().icon;

        print(openBioengineerPanel.GetPlayer() ? true : false);

        evolvingPocketPanel.SetPlayer(openBioengineerPanel.GetPlayer());
        evolvingPocketPanel.Setup(pocket, null, null, false);

        evolvedPocketPanel.SetPlayer(openBioengineerPanel.GetPlayer());
        evolvedPocketPanel.Setup(pocket, null, null, true);

        EventSystem.current.SetSelectedGameObject(evolveButton.gameObject);
    }


    int GetCurrentDna() { return PlayerPrefs.GetInt("DNA", 100); }

    public void EnablePocketMenu(bool enable, Pocket pocket)
    {
        isEnabled = enable;

        SetupValues(pocket);

        pocketMenuCanvas.SetActive(enable);
        //CanvasGroup group = GetComponent<CanvasGroup>();
        //group.alpha = enable ? 1 : 0;
        //group.interactable = enable ? true : false;

        evolveButton.Select();
    }

    public bool IsEnabled() { return isEnabled; }

    public void EvolvePressed(bool pressed)
    {
        Player player = currentPocket.GetPlayer();
        player.entryPanel.SetSubmitHeld(pressed);
    }

    public void EvolveButtonClick()
    {
        if (blinking) return;
        if (PlayerPrefs.GetInt("DNA") <= 0)
        {
            // Play some sound
            StartCoroutine(BlinkBar());
        }
    }

    IEnumerator BlinkBar()
    {
        blinking = true;
        confirmBar.fillAmount = 1;
        confirmBar.color = Color.red;

        yield return new WaitForSecondsRealtime(.25f);

        confirmBar.color = barColor;

        yield return new WaitForSecondsRealtime(.25f);

        confirmBar.color = Color.red;

        yield return new WaitForSecondsRealtime(.25f);

        confirmBar.color = barColor;

        yield return new WaitForSecondsRealtime(.25f);
        confirmBar.fillAmount = 0;
        blinking = false;
    }

    public void Evolve()
	{
        GetSaveSystem().EvolveAndSave(currentPocket);

        PlayerPrefs.SetInt("DNA", GetCurrentDna() - dnaCost);

        DestroyPocketWithSameLevel(currentPocket);

        Close();
        openBioengineerPanel.UpdateTexts();
        openBioengineerPanel.EnableOptions(false);

        currentPocket.GetPlayer().entryPanel.SetSubmitHeld(false);
    }

    void DestroyPocketWithSameLevel(Pocket pocket)
    {
        List<Pocket> allPockets = new List<Pocket>(FindObjectsOfType<Pocket>());
        if (allPockets.Contains(pocket))
            allPockets.Remove(pocket);

        foreach (var item in allPockets)
        {
            if (item.level == pocket.level)
            {
                Destroy(item.gameObject);
            }
        }

        pocket.GetPlayer().GetInputController().GetPlayerEntryPanel().unlockedPockets.Clear();
        pocket.GetPlayer().GetInputController().GetPlayerEntryPanel().UnlockPockets();
    }
}
