using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Spawn", menuName = "NPC Spawn")]
public class NpcSpawnSO : ScriptableObject
{
    public int MinNpcRooms;

    [Header("NPC Dependencies")]
    public NPC Merchant;
    public NPC Medic;
    public NPC Explorer;
    public NPC RetiredWarmonger;
    public NPC SinisterMerchant;
    public NPC Master;

    [Header("NPC Type Settings")]
    [Range(0f, 1f)] public float RetiredWarmongerChance;
    [Range(0f, 1f)] public float MedicChance;
    [Range(0f, 1f)] public float SinisterMerchantChance;
    [Range(0f, 1f)] public float ExplorerChance;
    [Range(0f, 1f)] public float MasterChance;

    [Header("Extra Rooms Settings")]
    [Range(0f, 1f)] public float PlusOneChance;
    [Range(0f, 1f)] public float PlusTwoChance;
    [Range(0f, 1f)] public float PlusThreeChance;

    private List<NPC> _npcList;
    private List<float> _chances;

    private float _merchantChance;
    private float _totalChance;
    private float _remainingChance;

    private void OnValidate()
    {
        _merchantChance = 1f - (MedicChance + ExplorerChance + RetiredWarmongerChance + SinisterMerchantChance + MasterChance);
        _totalChance = MedicChance + ExplorerChance + RetiredWarmongerChance + SinisterMerchantChance + MasterChance + _merchantChance;

        if (_totalChance > 1f)
            Debug.LogWarning($"Total NPC type chance for {name} is greater than 100%.");
    }

    public void Setup()
    {
        _npcList = new List<NPC>(){Medic, Explorer, RetiredWarmonger, SinisterMerchant, Master, Merchant };
        _chances = new List<float>(){MedicChance, ExplorerChance, RetiredWarmongerChance, SinisterMerchantChance, MasterChance, _merchantChance};
        _remainingChance = _totalChance;
    }

    public NPC GetNpcType(bool removeExplorer)
    {
        Debug.Log("------------------EXPLORER REMOVED? " + removeExplorer);

        var randomNpc = Random.Range(0f, _remainingChance);
        var cumulativeChance = 0f;

        for (int i = 0; i < _npcList.Count; i++)
        {
            if (removeExplorer)
            {
                if (_npcList[i] == Explorer)
                    SelectAndRemoveNpcFromList(i);
            }

            

            cumulativeChance += _chances[i];

            if (randomNpc < cumulativeChance)
            {
                return SelectAndRemoveNpcFromList(i);
            }
        }

        return SelectAndRemoveNpcFromList(_npcList.Count - 1);
    }

    private NPC SelectAndRemoveNpcFromList(int index)
    {
        var selectedNPC = _npcList[index];
        _npcList.RemoveAt(index);
        _chances.RemoveAt(index);

        _remainingChance = 0f;
        foreach (float chance in _chances)
            _remainingChance += chance;

        return selectedNPC;
    }

    public int GetNpcRoomCount()
    {
        var count = MinNpcRooms;
        var chances = new float[] {PlusOneChance, PlusTwoChance, PlusThreeChance};

        for (int i = 0; i < chances.Length; i++)
        {
            if (Random.value < chances[i])
                count++;
            else
                break;
        }

        return count;
    }
}