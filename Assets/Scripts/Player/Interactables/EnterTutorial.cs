using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTutorial : Interactable
{
    [Header("Tutorial UI")]
    public CanvasGroup blackFade;
    public CanvasGroup HUDCanvas;
    public GameObject minimapUI;
    public List<TutorialsController> tutorials = new List<TutorialsController>();
    Gate gate;
    Gate gate1;
    Gate gate2;

    [Header("Tutorial")]
    public GameObject lobbyElements;
    GameObject currentElements;
    public Transform entryPoint;
    public Transform exitPoint;
    Enemy canhao;
    [SerializeField] Transform canhaoSpot;
    Enemy adulto;
    [SerializeField] Transform adultoSpot;

    [HideInInspector] public Player thisPlayer;
    bool alreadyEntered = false;

    [SerializeField] public GameObject leaveTutorialButton;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SetName("Tutorial");
        SetDescription("Do you want to learn the basics first?");

        RestartLobby();
    }

    private void Update()
    {
        if(GameplayManager.Instance.GetPlayers(false).Count > 1)
        {
            SetName("Nope");
            SetDescription("This is a one-person training session!");
        }
        else
        {
            if (alreadyEntered)
            {
                SetName("Sorry");
                SetDescription("I need to rearrange everything again");
            }
            else
            {
                SetName("Training");
                SetDescription("Be prepared and learn how to survive on your journey.");
            }
        }

    }

    void RestartLobby()
    {
        LobbyController lobby = FindObjectOfType<LobbyController>();

        if (currentElements)
        {
            if (lobby.disabledStuff.Contains(currentElements))
                lobby.disabledStuff.Remove(currentElements);

            Destroy(currentElements);
        }

        //room4Trigger.enemies.Clear();

        GameObject newElements = Instantiate(lobbyElements, entryPoint.position, Quaternion.identity, entryPoint);
        currentElements = newElements;
        lobby.disabledStuff.Add(currentElements);

        canhao = currentElements.transform.Find("Canhao").GetComponent<Enemy>();
        adulto = currentElements.transform.Find("Adulto (1)").GetComponent<Enemy>();

        canhao.GetComponent<EnemyBehaviourController>().enabled = true;
        canhao.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        //currentEnemies.Add(canhao.gameObject);
        
        adulto.GetComponent<EnemyBehaviourController>().enabled = true;
        adulto.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;

        gate = currentElements.transform.Find("TutorialGate Variant").GetComponent<Gate>();
        gate1 = currentElements.transform.Find("TutorialGate Variant (1)").GetComponent<Gate>();
        gate2 = currentElements.transform.Find("TutorialGate Variant (2)").GetComponent<Gate>();
        //currentEnemies.Add(adulto.gameObject);

        //for (int i = 0; i < currentEnemies.Count; i++)
        //{
        //    room4Trigger.enemies.Add(currentEnemies[i]);
        //}

        foreach (var item in tutorials)
        {
            item.tutorialIndex = 0;
            item.gameObject.SetActive(false);
        }

        List<Player> players = GameplayManager.Instance.GetPlayers(false);
        foreach (var item in players)
        {
            item.GetHealth().SetHealth(item.GetHealth().GetMaxHealth());
        }

        tutorials[0].roomGate = gate;

        tutorials[1].roomGate = gate1;
        tutorials[1].closeGate = gate;

        tutorials[2].roomGate = gate2;
        tutorials[2].closeGate = gate1;

        tutorials[3].roomGate = gate2;
        tutorials[3].closeGate = gate2;

        Invoke("ResetLastTutorial", .1f);
    }

    public override void OnInteract(Player player)
    {
        if (alreadyEntered) return;
        GoToTutorial(player);
    }

    void GoToTutorial(Player player)
    {
        if (player.currentPocket == null)
        {
            SendMessageUpwards("OnBuyFail");
            return;
        }else if(player.currentPocket.pocketType == PetType.Egg)
        {
            SendMessageUpwards("EggTraining");
            return;
        }

        HUDCanvas.alpha = 1;
        minimapUI.SetActive(true);

        canhao.target = null;
        canhao.transform.position = canhaoSpot.position;
        canhao.GetComponent<EnemyBehaviourController>().isAggressive = false;
        adulto.target = null;
        adulto.transform.position = adultoSpot.position;
        adulto.GetComponent<EnemyBehaviourController>().isAggressive = false;

        thisPlayer = player;
        thisPlayer.isOnTutorial = true;
        thisPlayer.transform.position = entryPoint.position;

        leaveTutorialButton.SetActive(true);

        //alreadyEntered = true;
    }

    public void ExitTutorial()
    {
        HUDCanvas.alpha = 0;
        minimapUI.SetActive(false);

        TutorialsController[] tutorials = FindObjectsOfType<TutorialsController>();

        foreach (var item in tutorials)
        {
            item.tutorialIndex = 0;
            item.gameObject.SetActive(false);
        }

        thisPlayer.isOnTutorial = false;
        thisPlayer.transform.position = exitPoint.position;
        thisPlayer = null;

        leaveTutorialButton.SetActive(false);

        RestartLobby();
    }

    void ResetLastTutorial()
    {
        tutorials[4].SetComplete(false);
    }
}
