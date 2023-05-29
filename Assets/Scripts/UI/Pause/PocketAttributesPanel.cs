using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.SimpleLocalization;

public class PocketAttributesPanel : MonoBehaviour
{
    Pocket currentPocket;
    Player player;

    [Header("Egg")]
    [SerializeField] GameObject eggView;
    [SerializeField] TextMeshProUGUI eggTitle;
    [SerializeField] TextMeshProUGUI eggDescription;

    [Header("Default")]
    [SerializeField] GameObject defaultView;
    [SerializeField] TextMeshProUGUI pocketName;
    [SerializeField] TextMeshProUGUI pocketSpecial;
    [SerializeField] TextMeshProUGUI pocketSpecialDescription;

    [SerializeField] Image pocketIcon;
    [SerializeField] AttributeBar pocketHpBar;
    [SerializeField] AttributeBar pocketSpecialStatsBar;
    [SerializeField] AttributeBar pocketDropRateBar;
    [SerializeField] AttributeBar pocketWeaponBar;
    [SerializeField] AttributeBar pocketHealingBar;
    [SerializeField] AttributeBar pocketMovementBar;
    [SerializeField] AttributeBar stars;
    [SerializeField] TextMeshProUGUI amountText;

    [SerializeField] Button resumeButton;
    [SerializeField] TextMeshProUGUI resumeTextButton;

    [SerializeField] GameObject arrow;

    [Header("Lobby")]
    public Camera pocketCamera;
    [SerializeField] RawImage pocketImage;
    [SerializeField] TextMeshProUGUI categoryText;

    public void Setup(Player player, Pocket pocket)
	{
        this.player = player;
        Setup(pocket);
	}

    Material mat;

    public void Setup(Pocket pocket, RenderTexture texture = null, string category = null, bool evolved = false)
	{
        currentPocket = pocket;

        PocketDisplayInformations pocketInfo = pocket.GetComponent<PocketDisplayInformations>();
        pocketInfo.Setup();

        if (pocket.pocketType == PetType.Default)
        {
            eggView.SetActive(false);
            defaultView.SetActive(true);

            pocketName.text = pocketInfo.pocketName;
            pocketSpecial.text = pocketInfo.pocketSpecialName;
            pocketSpecialDescription.text = pocketInfo.pocketDescription;

			pocketIcon.rectTransform.localScale = new Vector3(1, pocketInfo.flipY ? -1 : 1, 1);

            pocketHpBar.SetValue(pocketInfo.GetAttributes().displayHp);
            pocketSpecialStatsBar.SetValue(pocketInfo.GetAttributes().specialStats);
            pocketDropRateBar.SetValue(pocketInfo.GetAttributes().dropRate);
            pocketWeaponBar.SetValue(pocketInfo.GetAttributes().weapon);
            pocketHealingBar.SetValue(pocketInfo.GetAttributes().healing);
            pocketMovementBar.SetValue(pocketInfo.GetAttributes().movement);

            //print("<color=yellow>" + player.GetInputController().GetPlayerEntryPanel().GetSelectedPocketIndex(pocket).ToString() + "| " + pocket.pocketName + "</color>");
            
            //amountText.text = "x" + pocket.level.ToString();

            if (evolved)
            {
                PaintExtraBarTiles(pocketHpBar, pocketInfo.GetAttributes().displayHp, pocketInfo.GetEvolvedAttributes().displayHp);
                PaintExtraBarTiles(pocketSpecialStatsBar, pocketInfo.GetAttributes().specialStats, pocketInfo.GetEvolvedAttributes().specialStats);
                PaintExtraBarTiles(pocketDropRateBar, pocketInfo.GetAttributes().dropRate, pocketInfo.GetEvolvedAttributes().dropRate);
                PaintExtraBarTiles(pocketWeaponBar, pocketInfo.GetAttributes().weapon, pocketInfo.GetEvolvedAttributes().weapon);
                PaintExtraBarTiles(pocketHealingBar, pocketInfo.GetAttributes().healing, pocketInfo.GetEvolvedAttributes().healing);
                PaintExtraBarTiles(pocketMovementBar, pocketInfo.GetAttributes().movement, pocketInfo.GetEvolvedAttributes().movement);
                stars.SetValue(pocket.level + 1);
            }
            else
            {
                stars.SetValue(pocket.level);
            }
        }
        else if (pocket.pocketType == PetType.Egg)
		{
            eggView.SetActive(true);
            defaultView.SetActive(false);

            stars.SetValue(0);

            if (pocket.GetComponentInParent<PocketSpot>())
                eggTitle.text = LocalizationManager.Localize("Pocket.EggName") + " #" + (pocket.GetComponentInParent<PocketSpot>().pocketsAvailable.IndexOf(pocket) + 1).ToString();
            else
                eggTitle.text = LocalizationManager.Localize("Pocket.EggName");
            
            eggDescription.text = LocalizationManager.Localize("Pocket.EggDescription");
			pocketIcon.rectTransform.localScale = Vector3.one;
        }

        if(ScreenManager.currentScreen == Screens.Lobby)
            amountText.text = "x" + FindObjectOfType<UnlockedCharacters>().unlockedPockets[player.GetInputController().GetPlayerEntryPanel().GetSelectedPocketIndex(pocket).ToString("00") + pocket.level.ToString()].ToString();

        

        pocketIcon.sprite = pocketInfo.icon;
        mat = pocketInfo.iconMaterial;

        if (ScreenManager.currentScreen != Screens.Lobby)
        {
            arrow.SetActive(player.pockets.Count > 1);
        }
        else
        {
            if(pocketCamera)
                pocketCamera.transform.position = new Vector3(pocket.transform.position.x, pocket.transform.position.y, -10);

            if (texture)
            {
                pocketCamera.targetTexture = texture;
                pocketImage.texture = texture;
            }
            else
            {
                pocketImage.texture = pocketCamera.targetTexture;
            }

            if (pocket.GetComponentInParent<PocketSpot>())
            {
                if (pocket.GetComponentInParent<PocketSpot>().pocketsAvailable.Count > 1)
                    arrow.SetActive(true);
                else
                    arrow.SetActive(false);
            }

            categoryText.text = category;
        }
    }

    public void PaintExtraBarTiles(AttributeBar bar, int currentValue, int targetValue)
    {
        int first = currentValue + 1;
        int last = targetValue + 1;
        bar.SetColorByInterval(new Vector2Int(first, last), true);
    }

    private void Update()
	{
        pocketIcon.material = mat;
    }

	public void Next()
	{
        Setup(player.GetNextPocket(currentPocket));
	}

    public void Previous()
	{
        Setup(player.GetPreviousPocket(currentPocket));
    }

    public void SetPlayer(Player player) { this.player = player; }

    public Player GetPlayer() { return player; }
}
