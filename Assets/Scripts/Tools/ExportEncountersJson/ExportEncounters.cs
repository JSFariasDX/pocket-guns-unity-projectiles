using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "EncountersExporter", menuName = "Tools/EncountersExporter")]
public class ExportEncounters : ScriptableObject
{
	[InspectorButton("OutputJson")]
	public bool export;

	public string folder;
	[TextArea]public string output;

	public void OutputJson()
	{
		FormattedEncounterData[] encounters = GetFormattedEncounters();

		//string strOutput = JsonUtility.ToJson(JsonArrayHelper.ToJson(encounters));
		string strOutput = JsonArrayHelper.ToJson(encounters);
		Debug.Log(strOutput);
		output = strOutput;

		string path = Application.persistentDataPath + "/Encounters" + folder + ".txt";

		File.WriteAllText(path, strOutput);
		ShowExplorer(path);
	}

	public void ShowExplorer(string itemPath)
	{
		itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
		System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
	}

	EnemiesEncounter[] GetEncounters()
	{
		return Resources.LoadAll<EnemiesEncounter>("Data/EnemiesEncounter/" + folder);
	}

	FormattedEncounterData[] GetFormattedEncounters()
	{
		List<FormattedEncounterData> encountersData = new List<FormattedEncounterData>();
		foreach(EnemiesEncounter encounter in GetEncounters())
		{
			encountersData.Add(new FormattedEncounterData(encounter));
		}

		return encountersData.ToArray();
	}
}

[System.Serializable]
public class FormattedEncounterData
{
	[SerializeField] public int challengeLevel;
	[SerializeField] public string enemies;

	public FormattedEncounterData(EnemiesEncounter encounter)
	{
		challengeLevel = encounter.challengeLevel;

		List<string> list = new List<string>();
		foreach(EnemyEncounter enc in encounter.encounters)
		{
			list.Add(enc.enemyCount + "x " + enc.enemyPrefab.gameObject.name);
		}

		enemies = JsonArrayHelper.ToJson(list.ToArray());
		enemies.Replace("Encontros", "");
	}
}



[System.Serializable]
public class EnemyEncounterJsonFormat
{
	public string enemyName;
	public int enemyCount;

	public EnemyEncounterJsonFormat(EnemyEncounter encounter)
	{
		enemyName = encounter.enemyPrefab.gameObject.name;
		enemyCount = encounter.enemyCount;
	}
}




public class JsonArrayHelper
{
	public static T[] FromJson<T>(string json)
	{
		Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
		return wrapper.Items;
	}

	public static string ToJson<T>(T[] array)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return UnityEngine.JsonUtility.ToJson(wrapper);
	}

	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] Items;
	}
}