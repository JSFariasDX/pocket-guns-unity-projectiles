using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;


public class LobbyPocketSelection : MonoBehaviour
{
    bool opened = false;

    [Header("Components")]
    public Light2D forestLight;

    [Header("Spots")]
    public List<PocketSpot> pocketSpots = new List<PocketSpot>();

    List<Pocket> allPockets = new List<Pocket>();
    List<Transform> allRandomTransforms = new List<Transform>();

    [Header("UI")]
    public CanvasGroup pocketSelectionUI;
    public List<GameObject> pocketPanels = new List<GameObject>();
    public List<RenderTexture> cameraTextures = new List<RenderTexture>();
    List<Player> allPlayers = new List<Player>();

    public bool hasClosed = false;
    public bool selected = false;

    // Start is called before the first frame update
    void Start()
    {
        pocketSelectionUI.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        selected = AllSelected();

        if (opened)
        {
            if (AllSelected())
                StartCoroutine(WaitToCloseSelection(2));
        }
    }

    bool AllSelected()
    {
        foreach (var item in GameplayManager.Instance.GetPlayers(false))
        {
            if (!item.GetInputController().GetPlayerEntryPanel().pocketSelected)
            {
                return false;
            }
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            OpenSelection();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            CloseSelection();
        }
    }

    public void OpenSelection()
    {
        //if (AllSelected()) return;

        foreach (var item in GameplayManager.Instance.GetPlayers(false))
        {
            item.GetInputController().GetPlayerEntryPanel().pocketSelected = false;
        }

        CinemachineFramingTransposer body = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;
        body.m_MaximumOrthoSize = 14;

        FocusAllPoints();

        GatherAllPockets();

        pocketSelectionUI.alpha = 1;
        pocketSelectionUI.interactable = true;
        pocketSelectionUI.blocksRaycasts = true;

        hasClosed = false;
        opened = true;

        List<Player> players = GameplayManager.Instance.GetPlayers(false);
        allPlayers = new List<Player>(players);
        foreach (var item in allPlayers)
        {
            List<PocketSpot> spots = new List<PocketSpot>();

            for (int i = 0; i < pocketSpots.Count; i++)
            {
                if (pocketSpots[i].pocketsAvailable.Count > 0)
                    spots.Add(pocketSpots[i]);
            }

            item.GetInputController().GetPlayerEntryPanel().pocketSpots.AddRange(spots);

            bool hadPocket = item.currentPocket;
            item.GetInputController().GetPlayerEntryPanel().selectedSpot = item.GetInputController().GetPlayerEntryPanel().pocketSpots[0];
            item.GetInputController().GetPlayerEntryPanel().spotIndex = hadPocket ? GetSpotIndex(item.GetInputController().GetPlayerEntryPanel(), item.currentPocket) : 0;
            item.GetInputController().GetPlayerEntryPanel().pocketIndex = hadPocket ? GetPocketIndexFromSpot(item.GetInputController().GetPlayerEntryPanel().pocketSpots, item.currentPocket) : 0;
            if(hadPocket)
                print("<color=yellow>" + GetPocketIndexFromSpot(item.GetInputController().GetPlayerEntryPanel().pocketSpots, item.currentPocket) + "</color>");
            item.GetInputController().SetMapInput("UI");

            item.gameObject.layer = 3;
            foreach (var gun in item.currentGuns)
            {
                gun.GetComponentInChildren<SpriteRenderer>().gameObject.layer = 3;
            }
        }

        for (int i = 0; i < pocketPanels.Count; i++)
        {
            if (i <= allPlayers.Count - 1)
            {
                pocketPanels[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                pocketPanels[i].transform.GetChild(1).GetComponent<Image>().color = Color.white;

                allPlayers[i].GetInputController().GetPlayerEntryPanel().lobbyPocketTexture = cameraTextures[i];
                pocketPanels[i].gameObject.SetActive(true);
                allPlayers[i].GetInputController().GetPlayerEntryPanel().pocketPanel = pocketPanels[i];
            }
            else pocketPanels[i].gameObject.SetActive(false);
        }

    }

    public void CloseSelection()
    {
        CinemachineFramingTransposer body = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;
        body.m_MaximumOrthoSize = 7;

        DefocusAllPoints();

        foreach (var item in allRandomTransforms)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in allPlayers)
        {
            item.GetInputController().GetPlayerEntryPanel().selectedSpot = null;
            item.GetInputController().GetPlayerEntryPanel().pocketSpots.Clear();
            item.GetInputController().SetMapInput("Gameplay");

            if(item.currentPocket)
                item.currentPocket.SetTargetPosition(item.PetPositionLeft);
        }

        foreach (var item in pocketSpots)
        {
            item.pocketsAvailable.Clear();
        }

        allRandomTransforms.Clear();
        pocketSelectionUI.alpha = 0;
        pocketSelectionUI.interactable = false;
        pocketSelectionUI.blocksRaycasts = false;

        Pocket[] pockets = FindObjectsOfType<Pocket>();
        allPockets = new List<Pocket>(pockets);

        foreach (var item in allPockets)
        {
            item.gameObject.layer = 0;
        }

        hasClosed = true;
        opened = false;
    }

    public void GatherAllPockets()
    {
        Pocket[] pockets = FindObjectsOfType<Pocket>();
        allPockets = new List<Pocket>(pockets);

        for (int i = 0; i < allPockets.Count; i++)
        {
            allPockets[i].gameObject.layer = 24;
            if (allPockets[i].pocketType == PetType.Egg || allPockets[i].level <= 0)
            {
                pocketSpots[7].AddPocket(allPockets[i]);
                allPockets[i].transform.parent = pocketSpots[7].transform;
                continue;
            }

            switch (allPockets[i].essence)
            {
                case PetEssence.Accuracy:
                    pocketSpots[2].AddPocket(allPockets[i]);
                    allPockets[i].transform.parent = pocketSpots[2].transform;
                    break;
                case PetEssence.Movement:
                    pocketSpots[6].AddPocket(allPockets[i]);
                    allPockets[i].transform.parent = pocketSpots[6].transform;
                    break;
                case PetEssence.Shot:
                    pocketSpots[0].AddPocket(allPockets[i]);
                    allPockets[i].transform.parent = pocketSpots[0].transform;
                    break;
                case PetEssence.GlobalDamage:
                    pocketSpots[5].AddPocket(allPockets[i]);
                    allPockets[i].transform.parent = pocketSpots[5].transform;
                    break;
                case PetEssence.Reward:
                    pocketSpots[4].AddPocket(allPockets[i]);
                    allPockets[i].transform.parent = pocketSpots[4].transform;
                    break;
                case PetEssence.Health:
                    pocketSpots[3].AddPocket(allPockets[i]);
                    allPockets[i].transform.parent = pocketSpots[3].transform;
                    break;
                case PetEssence.Protect:
                    pocketSpots[1].AddPocket(allPockets[i]);
                    allPockets[i].transform.parent = pocketSpots[1].transform;
                    break;
                default:
                    break;
            }

        }

        foreach (var item in pocketSpots)
        {
            item.SortList();
        }
    }

    int GetSpotIndex(PlayerEntryPanel panel, Pocket pocket)
    {
        for (int i = 0; i < panel.pocketSpots.Count; i++)
        {
            if (panel.pocketSpots[i].categoryName.Contains(pocket.essence.ToString()))
            {
                if (pocket.pocketType != PetType.Egg)
                    return i;
                else
                    return panel.pocketSpots.IndexOf(pocketSpots[7]);
            }
        }

        return 0;
    }

    int GetPocketIndexFromSpot(List<PocketSpot> spots, Pocket pocket)
    {
        if (pocket.pocketType == PetType.Egg)
        {
            return pocketSpots[7].pocketsAvailable.IndexOf(pocket);
        }
        else
        {
            for (int i = 0; i < spots.Count; i++)
            {
                if (spots[i].pocketsAvailable.Contains(pocket))
                    return spots[i].pocketsAvailable.IndexOf(pocket);
            }
        }

        return 0;
    }

    IEnumerator WaitToCloseSelection(float time)
    {
        opened = false;

        yield return new WaitForSeconds(time);

        CloseSelection();
    }

    void FocusAllPoints()
    {
        forestLight.intensity = .5f;

        CameraManager camManager = FindObjectOfType<CameraManager>();

        for (int i = 0; i < pocketSpots.Count; i++)
        {
            CinemachineTargetGroup.Target thisTarget = camManager.AddTarget(pocketSpots[i].transform, 2.5f, 1);
            pocketSpots[i].thisTarget = thisTarget;
        }
    }

    void DefocusAllPoints()
    {
        forestLight.intensity = 1f;

        CameraManager camManager = FindObjectOfType<CameraManager>();

        for (int i = 0; i < pocketSpots.Count; i++)
        {
            camManager.DisableTarget(pocketSpots[i].thisTarget);
        }
    }
}
