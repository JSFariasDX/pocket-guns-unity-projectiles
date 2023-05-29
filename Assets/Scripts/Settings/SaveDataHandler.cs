using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SaveDataHandler
{
    public static void SavePlayerData(PlayerData data, bool newSaveData = false)
    {
        string savePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "player.json";

        string json = JsonConvert.SerializeObject(data, Formatting.None);
        //Debug.Log(json);

        File.WriteAllText(savePath, json);

        if (newSaveData)
            Debug.Log("No save data found. Creating new save data... " + "(" + savePath + ")");
        else
            Debug.Log("Saving data... " + "(" + savePath + ")");
    }

    public static PlayerData LoadPlayerData()
    {
        string savePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "player.json";

        if (File.Exists(savePath))
        {
            string loaded = File.ReadAllText(savePath);

            PlayerData loadedData = JsonConvert.DeserializeObject<PlayerData>(loaded);

            Debug.Log("Loading data..." );

            return loadedData;
        }
        else
        {
            List<Caretaker> newCaretakers = new List<Caretaker>();
            Caretaker newCaretaker = new Caretaker("Zoy");
            newCaretakers.Add(newCaretaker);

            //List<UnlockedPocket> newPockets = new List<UnlockedPocket>();
            //UnlockedPocket newVeremillion = new UnlockedPocket("Vermillion", 1);
            //UnlockedPocket newIke = new UnlockedPocket("Ike", 1);
            //newPockets.Add(newVeremillion);
            //newPockets.Add(newIke);
            Dictionary<string, int> loadedPocket = new Dictionary<string, int>();

            loadedPocket["001"] = 1;
            loadedPocket["011"] = 1;

            PlayerData data = new PlayerData(newCaretakers, loadedPocket);

            SavePlayerData(data, true);

            return data;
        }
    }
}
