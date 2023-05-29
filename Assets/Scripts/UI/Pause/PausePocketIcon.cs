using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PausePocketIcon : MonoBehaviour
{
    [SerializeField] SpriteRenderer pocketSpriteRenderer;

    public void Setup(Pocket pocket)
	{
		pocketSpriteRenderer.GetComponent<SpriteLibrary>().spriteLibraryAsset = pocket.GetComponent<SpriteLibrary>().spriteLibraryAsset;
		pocketSpriteRenderer.flipY = pocket.flipY;
		pocketSpriteRenderer.material = pocket.GetSpriteRenderer().material;
	}
}
