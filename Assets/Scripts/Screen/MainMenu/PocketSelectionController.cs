using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PocketSelectionController : MonoBehaviour
{
    [Header("Pockets")]
    public List<GameObject> pets = new List<GameObject>();
    public List<GameObject> chosenPets = new List<GameObject>();
    public List<Toggle> buttons = new List<Toggle>();
    public int selectedButton;
    int selectIndex;

    GameObject currentPocket;
    public AudioSource selectPocketSound;

    // Start is called before the first frame update
    void Start()
    {
        return;
        if (SelectionManager.Instance.currentPockets.Count <= 0)
        {
            //SelectionManager.Instance.AddPocket(currentPocket.GetComponent<Pocket>());
            SelectPocket(0);

            foreach (GameObject item in chosenPets)
            {
                item.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        return;

        for (int i = 0; i < chosenPets.Count; i++)
        {
            if (i == selectIndex)
            {
                chosenPets[i].SetActive(true);
            }
            else
            {
                chosenPets[i].SetActive(false);
            }
        }
    }

    public void TogglePocket(bool toggle)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].isOn) selectedButton = i;
        }

        if (toggle)
            SelectPocket(selectedButton);
    }

    public Toggle GetSelectedButton()
    {
        return buttons[selectedButton];
    }

    public void SelectPocket(int index)
    {
        selectPocketSound.Play();
        selectIndex = index;
        currentPocket = pets[index];
        //SelectionManager.Instance.currentPockets[0] = currentPocket.GetComponent<Pocket>();
    }
}
