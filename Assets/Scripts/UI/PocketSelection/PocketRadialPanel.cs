using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PocketRadialPanel : MonoBehaviour
{
    public TextMeshProUGUI pocketNameText;
    public TextMeshProUGUI hpText;
    public Image hpFillBar;
    public TextMeshProUGUI specialTitleText;
    public TextMeshProUGUI specialDescriptionText;
    public PocketModGui modGuiPrefab;
    public Transform modsParent;
    private List<PocketModGui> modGuiList = new List<PocketModGui>();

    public void UpdateUi(Pocket pocket)
	{
        pocketNameText.text = pocket.pocketName;
        hpText.text = pocket.GetHealth().GetCurrentHealth() + "/" + pocket.GetHealth().GetMaxHealth();
        hpFillBar.fillAmount = pocket.GetHealth().GetCurrentPercentage();
        specialTitleText.text = pocket.specialTitle;
        specialDescriptionText.text = pocket.specialDescription;

        foreach(PocketModGui modGui in modGuiList)
		{
            Destroy(modGui.gameObject);
		}

        modGuiList.Clear();

        foreach(PocketMod mod in pocket.mods)
		{
            PocketModGui modGui = Instantiate(modGuiPrefab, modsParent);
            modGui.Setup(pocket, mod.modType.ToString(), mod.modValue);

            modGuiList.Add(modGui);
        }
	}
}
