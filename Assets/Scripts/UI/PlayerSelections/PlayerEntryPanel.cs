using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D.Animation;
using System.Linq;

public class PlayerEntryPanel : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] CurrentSelection currentSelection;
    private bool isConfirmed;

    [Header("Options")]
    [SerializeField] int currentCharacterIndex;
    [SerializeField] int currentPocketIndex;
    [SerializeField] int currentPocketLevel = 1;
    [SerializeField] List<PlayerSelection> characterOptions = new List<PlayerSelection>();
    private List<PlayerSelection> unlockedCharacters = new List<PlayerSelection>();
    List<GameObject> characterPhotos = new List<GameObject>();
    [SerializeField] public List<PocketSelection> pocketOptions = new List<PocketSelection>();
    public List<PocketSelection> unlockedPockets = new List<PocketSelection>();
    List<GameObject> pocketPhotos = new List<GameObject>();

    [Header("GUI")]
    [SerializeField] TextMeshProUGUI confirmText;
    [SerializeField] GameObject selectionIndicator;
    [SerializeField] Image characterImage;
    [SerializeField] Image pocketImage;
    public Image background;

    [Header("Tents")]
    public TentSelection selectedTent;
    Player selectedPlayer;
    List<TentSelection> tents = new List<TentSelection>();
    Ray2D ray;

    [Header("UI")]
    public GameObject playerInfo;
    public GameObject pocketInfo;
    public GameObject petClone;

    [Header("Player")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI dashText;

    [Header("Pocket")]
    public List<PocketSpot> pocketSpots;
	public PocketSpot selectedSpot;
    public int spotIndex = 0;
    public int pocketIndex = 0;
    public Pocket selectedPocket;
    public bool pocketSelected = false;

    [Header("Pocket | UI")]
    public GameObject pocketPanel;
    public RenderTexture lobbyPocketTexture;
    //public TextMeshProUGUI pocketNameText;
    //public TextMeshProUGUI pocketHealthText;
    //public TextMeshProUGUI pocketSpeedText;
    //[HideInInspector]

    [Header("Special (Chartacter)")]
    public TextMeshProUGUI specialText;
    string description;
    string specialDescription;

    GameObject currentPlayerImage;
    GameObject currentPocketImage;

    public PlayerEntryPanelSlot slot;
    PlayerEntryManager entryManager;
    bool confirmEnabled;
    RectTransform thisTransform;

    UnlockedCharacters unlock;

    public bool cheated = false;

    [Header("SFX")]
    [SerializeField] AudioClip caretakerNavigateSfx;
    [SerializeField] AudioClip pocketNavigateSfx;
    [SerializeField] AudioClip verticalNavigateSfx;
    [SerializeField] AudioClip confirmSfx;
    [SerializeField] AudioClip cancelSfx;
    AudioSource _audioSource;

    TentSelectionManager tentManager;

    bool submitHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        thisTransform = GetComponent<RectTransform>();
        unlock = GetComponent<UnlockedCharacters>();
        entryManager = FindObjectOfType<PlayerEntryManager>();
        tentManager = FindObjectOfType<TentSelectionManager>();

        _audioSource = GetComponent<AudioSource>();

        tents = tentManager.tents;
        selectedTent = tents[0];
        selectedPlayer = selectedTent.character;

        cheated = entryManager.enterWithCheat;

        UnlockCharacters();

        if (cheated)
            CheatUnlockPockets();

        UnlockPockets();

        //SpawnImages();

        //UpdateSprites();
        
        entryManager.OnPlayerJoined(this);

        index = entryManager.entries.IndexOf(this) + 1;

        selectedTent.SelectArrow(index);
        //pocketSpots = new List<PocketSpot>(FindObjectOfType<LobbyPocketSelection>().pocketSpots);

        Invoke("StartSelection", .2f);
    }

    public TentSelectionManager GetTentManager()
    {
        return tentManager;
    }

    void UnlockCharacters()
    {
        if (cheated)
        {
            for (int i = 0; i < characterOptions.Count; i++)
            {
                unlockedCharacters.Add(characterOptions[i]);
            }
        }
        else
        {
            for (int o = 0; o < unlock.unlockedCaretakers.Count; o++)
            {
                for (int i = 0; i < characterOptions.Count; i++)
                {
                    if (o < unlock.unlockedCaretakers.Count)
                    {
                        if (characterOptions[i].player.characterName.Contains(unlock.unlockedCaretakers[o].characterName))
                        {
                            unlockedCharacters.Add(characterOptions[i]);
                        }
                    }
                }
            }
        }
    }

    void CheatUnlockPockets()
    {
        if (cheated)
        {
            for (int i = 0; i < pocketOptions.Count; i++)
            {
                unlock.AddPocket(pocketOptions[i].pocket);
                //unlockedPockets.Add(pocketOptions[i]);
            }
        }
    }

    public void UnlockPockets()
    {
        foreach (var item in unlock.unlockedPockets)
        {
            int ID = int.Parse(item.Key.Substring(0, 2));
            int level = int.Parse(item.Key[2].ToString());

            PocketSelection newPocket = new PocketSelection(pocketOptions[ID]);
            //newPocket = pocketOptions[ID];

            newPocket.pocketLevel = level;
            unlockedPockets.Add(newPocket);
        }
    }

    private void Update()
    {
        //if(thisTransform.localScale != Vector3.one)
        //    GetComponent<RectTransform>().localScale = Vector3.one;

        //transform.localPosition = new Vector3(0, 0, 0);

        if (pocketPanel && selectedSpot)
        {
            selectedSpot = pocketSpots[spotIndex];
            selectedPocket = selectedSpot.pocketsAvailable[pocketIndex];

            if (pocketIndex < 0) pocketIndex = 0;

            pocketPanel.GetComponent<PocketAttributesPanel>().SetPlayer(GetComponent<PlayerInputController>().GetPlayer());
            pocketPanel.GetComponent<PocketAttributesPanel>().Setup(selectedPocket, lobbyPocketTexture, selectedSpot.categoryName);
        }else
        {
            spotIndex = 0;
            pocketIndex = 0;
        }
    }

    void SpawnImages()
    {
        for (int i = 0; i < characterOptions.Count; i++)
        {
            GameObject currentPhoto = Instantiate(characterOptions[i].playerImage, characterImage.transform.position, Quaternion.identity, transform);
            if (!characterPhotos.Contains(currentPhoto))
            {
                currentPhoto.SetActive(false);
                characterPhotos.Add(currentPhoto);
            }
        }

        for (int i = 0; i < pocketOptions.Count; i++)
        {
            GameObject currentPhoto = Instantiate(pocketOptions[i].pocketImage, pocketImage.transform.position, Quaternion.identity, transform);
            if (!pocketPhotos.Contains(currentPhoto))
            {
                currentPhoto.SetActive(false);
                pocketPhotos.Add(currentPhoto);
            }
        }
    }

    void UpdateSprites()
    {
        for (int i = 0; i < characterOptions.Count; i++)
        {
            if (i == currentCharacterIndex)
            {
                characterPhotos[i].SetActive(true);
                currentPlayerImage = characterPhotos[i];
            }
            else
                characterPhotos[i].SetActive(false);
        }

        nameText.text = characterOptions[currentCharacterIndex].player.characterName + " & " + pocketOptions[currentPocketIndex].pocket.pocketName;

        //characterImage.sprite = characterOptions[currentCharacterIndex].GetSpriteRenderer().sprite;
        //pocketImage.sprite = pocketOptions[currentPocketIndex].GetSpriteRenderer().sprite;
    }
    void StartSelection()
	{
        confirmEnabled = true;
        SetPocketAnimation(0);
    }

    public void HorizontalNavigate(InputAction.CallbackContext ctx)
    {
        print("|=� PRESSED �=|");
        if (!selectedSpot) return;

        


        //if (currentSelection == CurrentSelection.Character) NavigateCharacter(ctx.ReadValue<Vector2>().x);
        //else NavigatePocket(ctx.ReadValue<Vector2>().x);
    }

    public void Navigate(InputAction.CallbackContext ctx)
    {
        if (pocketSelected) return;

        if (selectedSpot)
        {
            if (ctx.ReadValue<Vector2>().x > 0) // Right
            {
                if (pocketIndex >= selectedSpot.pocketsAvailable.Count - 1)
                    pocketIndex = 0;
                else
                    pocketIndex++;
            }
            else if (ctx.ReadValue<Vector2>().x < 0) // Left
            {
                if (pocketIndex <= 0)
                    pocketIndex = selectedSpot.pocketsAvailable.Count - 1;
                else
                    pocketIndex--;
            }

            if (ctx.ReadValue<Vector2>().y > 0) // Up
            {
                pocketIndex = 0;

                if (spotIndex >= pocketSpots.Count - 1)
                    spotIndex = 0;
                else
                    spotIndex++;
            } 
            else if (ctx.ReadValue<Vector2>().y < 0) // Down
            {
                pocketIndex = 0;

                if (spotIndex <= 0)
                    spotIndex = pocketSpots.Count - 1;
                else
                    spotIndex--;
            }
        }

        if (IsConfirmed()) return;

        if (!selectedSpot)
        {
            ray = new Ray2D(selectedTent.transform.position, ctx.ReadValue<Vector2>() * 100);
            RaycastHit2D[] hit = Physics2D.RaycastAll(ray.origin, ray.direction, 100, LayerMask.GetMask("Tents"));

            List<RaycastHit2D> newHit = new List<RaycastHit2D>(hit);
            newHit.RemoveAll(h => h.collider.gameObject == selectedTent.gameObject);

            if (newHit[0].collider.gameObject.GetComponent<TentSelection>())
            {
                selectedTent.SelectArrow(index, false);
                selectedTent = newHit[0].collider.gameObject.GetComponent<TentSelection>();
                selectedTent.SelectArrow(index, true);
                selectedPlayer = selectedTent.character;
                print("|SELECTION TENT|" + selectedTent.transform.name);
            }
        }
    }

    void NavigateCharacter(float value)
    {
        if (IsConfirmed()) return;

        _audioSource.PlayOneShot(caretakerNavigateSfx);

        if (value > 0)
        {
            currentCharacterIndex++;
            if (currentCharacterIndex >= characterOptions.Count)
            {
                currentCharacterIndex = 0;
            }
        }
        else
        {
            currentCharacterIndex--;
            if (currentCharacterIndex < 0)
            {
                currentCharacterIndex = characterOptions.Count - 1;
            }
        }

        UpdateSprites();
    }

    void NavigatePocket(float value)
    {
        if (IsConfirmed()) return;

        _audioSource.PlayOneShot(caretakerNavigateSfx);

        if (value > 0)
        {
            currentPocketIndex++;
            if (currentPocketIndex >= pocketOptions.Count)
            {
                currentPocketIndex = 0;
            }
            SetPocketAnimation(currentPocketIndex);
        }
        else
        {
            currentPocketIndex--;
            if (currentPocketIndex < 0)
            {
                currentPocketIndex = pocketOptions.Count - 1;
            }
            SetPocketAnimation(currentPocketIndex);
        }

        UpdateSprites();
    }
    public void SetPocketAnimation(int index)
    {
        petClone.GetComponent<SpriteRenderer>().sprite = pocketOptions[index].pocket.gameObject.GetComponent<SpriteRenderer>().sprite;
        petClone.GetComponent<SpriteLibrary>().spriteLibraryAsset = pocketOptions[index].pocket.gameObject.GetComponent<SpriteLibrary>().spriteLibraryAsset;
        petClone.GetComponent<SpriteRenderer>().flipY = pocketOptions[index].pocket.gameObject.GetComponent<Pocket>().flipY;
        petClone.GetComponent<SpriteRenderer>().material = pocketOptions[index].pocket.gameObject.GetComponent<SpriteRenderer>().sharedMaterial;
        petClone.GetComponent<SpriteRenderer>().material.renderQueue = 4000;
        petClone.GetComponent<Animator>().SetInteger("state", 1);
    }

    public void Confirm(InputAction.CallbackContext ctx)
    {
        if (selectedPocket && !pocketSelected)
        {
            currentPocketIndex = GetSelectedPocketIndex(selectedPocket);

            Player player = GetComponent<PlayerInputController>().GetPlayer();
            if (player.currentPocket)
            {
                currentPocketLevel = selectedPocket.level;
                player.currentPocket.UnSetup();
                player.currentPocket.transform.SetParent(selectedSpot.transform);
                player.currentPocket = null;
            }
            
            if(selectedPocket.pocketType != PetType.Egg)
                selectedPocket.EnableSpecial();

            player.AddPocket(selectedPocket);
            currentPocketLevel = selectedPocket.level;

            player.GetInputController().GetPlayerEntryPanel().

            pocketPanel.transform.GetChild(0).GetComponent<Image>().color = Color.green;
            pocketPanel.transform.GetChild(1).GetComponent<Image>().color = Color.green;

            print("<color=cyan>Pocket: " + selectedPocket.pocketName + "</color> |<color=yellow> Level: " + currentPocketLevel + "</color>");
            pocketSelected = true;
        }

        if (!confirmEnabled || IsConfirmed()) return;

        _audioSource.PlayOneShot(confirmSfx);

        //nameText.color = Color.green;
        //selectionIndicator.SetActive(false);
        isConfirmed = true;

        //currentPlayerImage.GetComponentInChildren<Animator>().SetTrigger("Selected");

        if(confirmText)
            confirmText.text = "Cancel";
    }

    public void Cancel(InputAction.CallbackContext ctx)
    {
        if (!confirmEnabled) return;
        
        _audioSource.PlayOneShot(cancelSfx);
        
        if (isConfirmed)
        {
            //selectedTent.SelectArrow(index, false);
            //if (confirmText)
            //    confirmText.text = "Confirm";

            ////nameText.color = Color.white;
            ////selectionIndicator.SetActive(true);
            //isConfirmed = false;
            //entryManager.CancelStarting();
        }
        else
        {
            selectedTent.SelectArrow(index, false);

            PlayerEntryManager manager = FindObjectOfType<PlayerEntryManager>();

            if (manager.entries.Contains(this))
                manager.entries.Remove(this);

            if (index == 1)
                manager.LeaveGame(true);

            //slot.background.SetActive(true);
            Destroy(gameObject);
        }
    }

    public void SetSubmitHeld(bool held)
    {
        submitHeld = held;
    }

    public bool SubmitHeld()
    {
        return submitHeld;
    }

    void Uncofirm()
    {

    }

    public void VerticalNavigate(InputAction.CallbackContext ctx)
    {
        return;

        //_audioSource.PlayOneShot(verticalNavigateSfx);

        //if (currentSelection == CurrentSelection.Character)
        //{
        //    currentSelection = CurrentSelection.Pocket;
        //    selectionIndicator.transform.position = pocketImage.transform.position;
        //}
        //else
        //{
        //    currentSelection = CurrentSelection.Character;
        //    selectionIndicator.transform.position = characterImage.transform.position;
        //}
    }

    public bool IsConfirmed()
    {
        return isConfirmed;
    }

    public void SetPlayer(Player player)
	{
        GetComponent<PlayerInputController>().SetPlayer(player);
	}

    public Player GetCurrentPlayer()
	{
        return selectedPlayer;
    }

    public PocketSelection GetCurrentPocket()
	{
        return pocketOptions[currentPocketIndex];
    }

    public void SetCurrentPocketIndex(int index) { currentPocketIndex = index; }
    public int GetCurrentPocketLevel() { return currentPocketLevel; }
    public void SetCurrentPocketLevel(int level) { currentPocketLevel = level; }

    public int GetSelectedPocketIndex(Pocket pocket)
    {
        int index = 0;

        for (int i = 0; i < pocketOptions.Count; i++)
        {
            if (pocketOptions[i].pocket.pocketName == pocket.pocketName)
                index = i;
        }

        return index;
    }

    public UnlockedCharacters GetUnlock()
    {
        return unlock;
    }
}


public enum CurrentSelection
{
    Character, Pocket
}

[System.Serializable]
public class PlayerSelection
{
    public Player player;
    public GameObject playerImage;
}

[System.Serializable]
public class PocketSelection
{
    public Pocket pocket;
    public GameObject pocketImage;
    [TextArea]
    public string powerDescription;
    public int pocketLevel = 1;

    public PocketSelection(PocketSelection selection)
    {
        pocket = selection.pocket;
        pocketImage = selection.pocketImage;
        powerDescription = selection.powerDescription;
        pocketLevel = selection.pocketLevel;
    }
}