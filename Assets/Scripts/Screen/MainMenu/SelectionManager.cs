using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    [Header("Pocket")]
    public List<Player> currentCharacters = new List<Player>();
    public List<Pocket> currentPockets = new List<Pocket>();

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

	public void Reset()
	{
        currentCharacters.Clear();
        currentPockets.Clear();
	}

    public List<Player> GetCurrentCharacters()
	{
        return currentCharacters;
	}

	public void AddCharacter(Player player)
	{
        currentCharacters.Add(player);
	}

    public void AddPocket(Pocket pocket)
	{
        currentPockets.Add(pocket);
	}
}
