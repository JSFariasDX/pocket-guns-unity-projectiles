using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dungeonText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Transform defeatedEnemiesParent;
    [SerializeField] DefeatedEnemiesGui defeatedEnemiesPrefab;
    [SerializeField] TextMeshProUGUI collectedCoinsText;
    [SerializeField] TextMeshProUGUI scoreText;

	private void Awake()
	{
        
    }

    public static void Clear()
    {
        SelectionManager.Instance.Reset();

        foreach (PlayerEntryPanel entryPanel in FindObjectsOfType<PlayerEntryPanel>())
        {
            Destroy(entryPanel.gameObject);
        }

        // Players
        foreach (Player player in FindObjectsOfType<Player>())
        {
            Destroy(player.gameObject);
        }

        // Pocket menu
        foreach (PocketRadialMenu pocketMenu in FindObjectsOfType<PocketRadialMenu>())
        {
            Destroy(pocketMenu.transform.parent.gameObject);
        }

        // HUD
        Destroy(GameObject.FindGameObjectWithTag("HUDCanvas"));
    }
}
