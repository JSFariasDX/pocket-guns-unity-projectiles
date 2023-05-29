using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    [SerializeField] private GameObject forestGlobalLight;
	[SerializeField] private List<DungeonType> globalLightDungeons = new List<DungeonType>();

    public void Setup(DungeonConfig config)
	{
		Debug.Log("Configuring Global Light...");
		bool useGlobalLight = globalLightDungeons.Contains(config.type);
		forestGlobalLight.SetActive(useGlobalLight);
		EnablePlayersLight(!useGlobalLight);
	}

	private void EnablePlayersLight(bool enable)
	{
		Debug.Log("Configuring Players Light...");
		foreach (PlayerLight light in FindObjectsOfType<PlayerLight>(true))
		{
			light.gameObject.SetActive(enable);
		}
	}
}
