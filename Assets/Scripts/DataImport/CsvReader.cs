using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsvReader : MonoBehaviour
{
    public TextAsset textAssetData;

    [System.Serializable]
    public class PocketData
	{
        public string name;
        public int hp;
        public int dropRate;
        public int weapon;
        public int healing;
        public int movement;
        public int specialStats;
	}

    [System.Serializable]
    public class PocketDataList
	{
        public PocketData[] pocketDatas;
	}
    public PocketDataList pocketList = new PocketDataList();

	private void Start()
	{
        ReadCsv();
    }

    void ReadCsv()
	{
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);

        int tableSize = data.Length / 4 - 1;
        pocketList.pocketDatas = new PocketData[tableSize];

        for (int i = 0; i < tableSize; i++)
		{
            pocketList.pocketDatas[i] = new PocketData();
            pocketList.pocketDatas[i].name = data[4 * (i + 1)];
        }
    }
}
