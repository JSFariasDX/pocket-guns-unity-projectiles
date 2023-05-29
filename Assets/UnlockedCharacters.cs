using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class UnlockedCharacters : MonoBehaviour
{
    PlayerEntryPanel panel;

    public List<Caretaker> unlockedCaretakers = new List<Caretaker>();
    //public List<UnlockedPocket> unlockedPockets = new List<UnlockedPocket>();

    public Dictionary<string, int> unlockedPockets = new Dictionary<string, int>();

    //PlayerData data;

    // Start is called before the first frame update
    void Awake()
    {
        unlockedCaretakers.Clear();
        panel = GetComponent<PlayerEntryPanel>();
        //unlockedPockets.Clear();

        //data = new PlayerData(unlockedCaretakers, unlockedPockets);

        LoadData();
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPocket(Pocket pocket, bool hatched = false)
    {
        for (int i = 0; i < panel.pocketOptions.Count; i++)
        {
            if(panel.pocketOptions[i].pocket.pocketName == pocket.pocketName)
            {
                pocket.pocketIndex = panel.pocketOptions.IndexOf(panel.pocketOptions[i]);
            }
        }

        string pocketKey = pocket.pocketIndex.ToString("00") + (pocket.pocketType == PetType.Egg ? "0" : "1");

        if (unlockedPockets.ContainsKey(pocketKey))
        {
            unlockedPockets[pocketKey]++;
        }
        else
        {
            unlockedPockets.Add(pocketKey, 1);
        }

        if (hatched)
        {
            unlockedPockets[pocket.pocketIndex.ToString("00") + "0"]--;

            if (unlockedPockets[pocket.pocketIndex.ToString("00") + "0"] <= 0)
                unlockedPockets.Remove(pocket.pocketIndex.ToString("00") + "0");
        }
    }

    public Pocket GetPocketByKey(string key)
    {
        int index = int.Parse(key.Substring(0, 2));

        return GetComponent<PlayerEntryPanel>().pocketOptions[index].pocket;
    }

    public Pocket GetFirstPocket()
	{
        return GetPocketByKey(unlockedPockets.First().Key);

    }

    public void SaveData()
    {
        PlayerData newData = new PlayerData(unlockedCaretakers, unlockedPockets);

        SaveDataHandler.SavePlayerData(newData);
    }

    void LoadData()
    {
        PlayerData loadedData = SaveDataHandler.LoadPlayerData();

        unlockedPockets = loadedData.loadedPocket;
        unlockedCaretakers = loadedData.unlockedCaretakers;
    }

    public void RemovePocket(Pocket pocket)
	{
        int index = GetPocketIndex(pocket);

        // Remove current level pocket
        string oldKey = index.ToString("00") + pocket.level.ToString();
        if (unlockedPockets[oldKey] > 1)
        {
            unlockedPockets[oldKey]--;
        }
        else
        {
            unlockedPockets.Remove(oldKey);
            pocket.GetPlayer().GetInputController().GetPlayerEntryPanel().unlockedPockets.Clear();
            pocket.GetPlayer().GetInputController().GetPlayerEntryPanel().UnlockPockets();

            if (pocket.pocketType == PetType.Egg)
            {
                Destroy(pocket.gameObject);
            }
            else if (GetPocketCount(pocket) <= 0)
            {
                //FindObjectOfType<PocketDnaTradePanel>().DeletePocketInstances(pocket);
                pocket.GetPlayer().pockets.Remove(pocket);
                Destroy(pocket.gameObject);
            }
            else if (!HaveMoreOfSameLevel(pocket))
            {
                int newLevel = GetOtherLevel(pocket);
                pocket.level = newLevel;
            }
            //Destroy(pocket.gameObject);
        }

        SaveData();
        LoadData();
    }

    bool HaveMoreOfSameLevel(Pocket pocket)
	{
        foreach (PocketSelection p in pocket.GetPlayer().GetInputController().GetPlayerEntryPanel().unlockedPockets)
        {
            if (p.pocket.pocketName.Equals(pocket.pocketName) && p.pocketLevel == pocket.level && p.pocketLevel > 0)
            {
                return true;
            }
        }

        return false;
    }

    int GetOtherLevel(Pocket pocket)
	{
        int other = pocket.level;

        foreach (PocketSelection p in pocket.GetPlayer().GetInputController().GetPlayerEntryPanel().unlockedPockets)
        {
            if (p.pocket.pocketName.Contains(pocket.pocketName) && p.pocketLevel != pocket.level && p.pocketLevel > 0)
			{
                return p.pocketLevel;
			}
        }

        return other;
	}

    public void EvolveAndSave(Pocket pocket)
    {
        int index = GetPocketIndex(pocket);

        // Remove current level pocket
        string oldKey = index.ToString("00") + pocket.level.ToString();
        if (unlockedPockets[oldKey] > 1)
		{
            unlockedPockets[oldKey]--;
		}
		else
		{
            unlockedPockets.Remove(oldKey);
		}

        // Register new pocket level
        pocket.level++;
        string newKey = index.ToString("00") + pocket.level;

        if (unlockedPockets.ContainsKey(newKey)) 
        {
            unlockedPockets[newKey]++;
		}
		else
		{
            unlockedPockets[newKey] = 1;
        }

        SaveData();
    }

    int GetPocketIndex(Pocket pocket)
	{
        for (int i = 0; i < panel.pocketOptions.Count; i++)
        {
            if (panel.pocketOptions[i].pocket.pocketName == pocket.pocketName)
            {
                pocket.pocketIndex = panel.pocketOptions.IndexOf(panel.pocketOptions[i]);
                return pocket.pocketIndex;
            }
        }

        return 0;
    }

    public int GetPocketCount(Pocket pocket)
	{
        int index = GetPocketIndex(pocket);

        int pocketCount = 0;

        for(int i = 1; i < 4; i++)
		{
            string key = index.ToString("00") + i;
            if (unlockedPockets.ContainsKey(key))
			{
                pocketCount += unlockedPockets[key];

            }
		}

        return pocketCount;
	}

    public int GetPocketsCount()
	{
        int count = 0;
        foreach(var p in unlockedPockets.Values)
		{
            count += p;
		}

        return count;
	}
}

[System.Serializable]
public class PlayerData
{
    public List<Caretaker> unlockedCaretakers = new List<Caretaker>();
    public Dictionary<string, int> loadedPocket = new Dictionary<string, int>();

    public PlayerData(List<Caretaker> caretakers, Dictionary<string, int> pockets)
    {
        unlockedCaretakers = caretakers;
        loadedPocket = pockets;
    }
}

[System.Serializable]
public class Caretaker
{
    public string characterName;

    public Caretaker(string name)
    {
        characterName = name;
    }
}

[System.Serializable]
public class UnlockedPocket
{
    public string pocketName;
    public int howMany;

    public UnlockedPocket(string name, int howMany)
    {
        pocketName = name;
        this.howMany = howMany;
    }
}
