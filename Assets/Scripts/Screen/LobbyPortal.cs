using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPortal : MonoBehaviour
{
    [Header("Components")]
    public LoadingScreenManager loadingScreen;
    Animator animator;
    [SerializeField] float minimumDistance = 5;
    [SerializeField] Collider2D portalTrigger;

    float playersDistance;
    bool isOpen = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        loadingScreen = FindObjectOfType<LoadingScreenManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!FindObjectOfType<LobbyPocketSelection>().hasClosed)
        {
            animator.SetFloat("Closing Speed", 2);
            isOpen = false;
            return;
        }

        animator.SetFloat("Closing Speed", 1);
        playersDistance = Vector2.Distance(transform.position, Camera.main.transform.Find("TargetGroup").position);

        if (AllPlayersHavePockets())
        {
            if (playersDistance < minimumDistance)
                isOpen = true;
            else
                isOpen = false;

            animator.SetBool("Opened", isOpen);
        }
    }

    bool AllPlayersHavePockets()
    {
        List<Player> players = GameplayManager.Instance.GetPlayers(true);
        foreach (var item in players)
        {
            if (item.currentPocket == null) return false;
        }

        return true;
    }

    public void ActivatePortal()
    {
        if (portalTrigger.enabled)
            portalTrigger.enabled = false;
        else
            portalTrigger.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            if (!isOpen) return;

            loadingScreen.OpenLoading();
            MainMenu.StartGame(Difficulty.Easy);

            foreach (var item in GameplayManager.Instance.players)
            {
                Destroy(item.gameObject);
            }
        }
    }
}
