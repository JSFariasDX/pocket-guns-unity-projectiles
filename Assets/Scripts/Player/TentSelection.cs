using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentSelection : MonoBehaviour
{
    [Header("Components")]
    public Player character;
    public GameObject inGameCharacter;
    public GameObject returnInteraction;

    bool isSelected = false;

    bool player1Selecting;
    bool player2Selecting;
    bool player3Selecting;
    bool player4Selecting;

    public List<GameObject> playerArrows = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerArrows[0].SetActive(player1Selecting);
        playerArrows[1].SetActive(player2Selecting);
        playerArrows[2].SetActive(player3Selecting);
        playerArrows[3].SetActive(player4Selecting);
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }

    public void SetIsSelected(bool select)
    {
        isSelected = select;
    }

    public void SelectArrow(int index, bool selecting = true)
    {
        switch (index)
        {
            case 1:
                player1Selecting = selecting;
                break;
            case 2:
                player2Selecting = selecting;
                break;
            case 3:
                player3Selecting = selecting;
                break;
            case 4:
                player4Selecting = selecting;
                break;
        }
    }
}
