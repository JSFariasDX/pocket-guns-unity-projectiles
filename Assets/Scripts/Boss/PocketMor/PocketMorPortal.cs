using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketMorPortal : MonoBehaviour
{
    public LoadingScreenManager loadingScreen;
    bool isActivated;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isActivated) return;

		if (collision.CompareTag("PlayerCollider"))
		{
			if (!collision.GetComponentInParent<Player>().currentPocket) return;

			isActivated = true;

			loadingScreen.OpenLoading();
			MainMenu.StartPocketMorBattle();

			foreach (var item in GameplayManager.Instance.players)
			{
				Destroy(item.gameObject);
			}
		}
	}
}
