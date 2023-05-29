using Assets.SimpleLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PocketDisplayInformations : MonoBehaviour
{
    public string pocketName;
    public string pocketSpecialName;
    [TextArea] public string pocketDescription;

    PocketDisplayAttributes attributesLevel1;
    PocketDisplayAttributes attributesLevel2;
    PocketDisplayAttributes attributesLevel3;

    public bool flipY;

    public Sprite icon;
    public Material iconMaterial;

    Pocket pocket;

	private void Start()
	{
        Setup();
	}
	public void Setup()
	{
		pocket = GetComponent<Pocket>();
       
        pocketName = pocket.pocketName;
        pocketSpecialName = GetComponent<Special>().displayName;
        pocketDescription = LocalizationManager.Localize("Pocket." + pocketName + "_level " + pocket.level);

        if (pocket.pocketType == PetType.Default) icon = pocket.GetComponent<SpriteLibrary>().GetSprite("Idle", "Entry_0");
        else icon = pocket.GetComponent<SpriteLibrary>().GetSprite("Egg", "Entry");

        flipY = pocket.flipY;
        iconMaterial = pocket.GetSpriteRenderer().material;
    }

    public PocketDisplayAttributes GetAttributes()
	{
        attributesLevel1 = new PocketDisplayAttributes(1);
        attributesLevel2 = new PocketDisplayAttributes(2);
        attributesLevel3 = new PocketDisplayAttributes(3);

        if (pocket.level == 1) return attributesLevel1;
        else if (pocket.level == 2) return attributesLevel2;
        else return attributesLevel3;
	}
    public PocketDisplayAttributes GetEvolvedAttributes()
    {
        if (pocket.level == 1) return attributesLevel2;
        else return attributesLevel3;
    }
}
