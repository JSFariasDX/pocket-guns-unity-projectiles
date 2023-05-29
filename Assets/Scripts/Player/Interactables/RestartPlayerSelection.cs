using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartPlayerSelection : Interactable
{
    [Header("Showcase")]
    public GameObject tentPlayer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SetName("Maybe some other time?");
        SetDescription("Another one can go");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnInteract(Player player)
    {
        GameObject.Find("LobbyCenter").AddComponent<AudioListener>();

        Camera.main.GetComponent<CameraManager>().DisableTarget(player.camTarget);
        Camera.main.GetComponent<CameraManager>().AddTarget(GameObject.Find("LobbyCenter").transform);

        LobbyController lobby = FindObjectOfType<LobbyController>();

        Destroy(GameObject.FindGameObjectWithTag("HUDCanvas"));
        Destroy(GameObject.FindGameObjectWithTag("PauseManager"));
        Destroy(GameObject.FindGameObjectWithTag("Respawn"));
        Destroy(GameObject.Find("LoadingScreen 2"));

        for (int i = 0; i < lobby.disabledStuff.Count; i++)
        {
            if (lobby.disabledStuff[i] != this.gameObject)
                lobby.disabledStuff[i].SetActive(false);
        }

        Destroy(player.gameObject);

        tentPlayer.SetActive(true);
        popUp.alpha = 0;

        StartCoroutine(Restart(1));
    }

    IEnumerator Restart(float time)
    {
        FindObjectOfType<LobbyPocketSelection>().GatherAllPockets();

        yield return new WaitForSecondsRealtime(time);

        ScreenManager.Instance.ChangeScreen(Screens.Lobby);
    }
}
