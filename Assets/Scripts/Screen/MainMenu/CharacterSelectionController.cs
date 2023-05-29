using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionController : MonoBehaviour
{
    [Header("Character")]
    public List<Player> characters = new List<Player>();
    public Transform charactersGroup;
    public Image choosenCharacter;
    public List<Toggle> buttons = new List<Toggle>();
    public int selectedButton;
    int selectIndex;

    public AudioSource selectCharacterSound;

    // Start is called before the first frame update
    void Start()
    {
        return;
        buttons = new List<Toggle>(charactersGroup.GetComponentsInChildren<Toggle>(true));
        SelectionManager.Instance.Reset();
        SelectionManager.Instance.AddCharacter(characters[0]);

        if (SelectionManager.Instance.currentCharacters.Count <= 0)
        {
            SelectCharacter(0);
        }
    }

    public void ToggleCharacter(bool toggle)
    {
        // Select character
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].isOn) selectedButton = i;
        }

        if (toggle) SelectCharacter(selectedButton);
    }

    public Toggle GetSelectedButton()
	{
        return buttons[selectedButton];
	}

    public void SelectCharacter(int index)
    {
        selectCharacterSound.Play();
        selectIndex = index;
        choosenCharacter.sprite = characters[index].GetComponent<SpriteRenderer>().sprite;
        SelectionManager.Instance.AddCharacter(characters[index]);
    }
}
