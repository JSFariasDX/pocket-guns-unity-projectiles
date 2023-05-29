using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerArrow : MonoBehaviour
{
	// Troca a sprite somente se tiver mais de 1 jogador/entrada
	[SerializeField] Player player;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
	[SerializeField] TMPro.TextMeshProUGUI playerIndex;

	private void Start()
	{ 
		int entriesCount = FindObjectsOfType<PlayerInputController>().Length;
		if (entriesCount > 1)
		{
			//if (image) image.sprite = sprites[playerGui.GetPlayer().playerIndex];
			spriteRenderer.sprite = sprites[player.playerIndex];
			playerIndex.text = "P" + (player.GetPlayerIndex() + 1);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

    private void Update()
    {
		playerIndex.gameObject.SetActive(ScreenManager.currentScreen == Screens.Lobby);
    }
}
