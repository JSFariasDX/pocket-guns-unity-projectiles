using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialsController : MonoBehaviour
{
    [Header("Components")]
    PlayerControls controls;

    [Header("Tutorials")]
    public bool removeFirstInMultiplayer;
    public List<GameObject> tutorials = new List<GameObject>();
    public int tutorialIndex = 0;
    public Gate roomGate;
    public Gate closeGate;
    public bool shouldUnlockGate = true;

    [Header("Last Tutorial")]
    public bool lastTutorial = false;
    public CanvasGroup blackFade;
    bool completed = false;

    public Pocket pocket;
    private Health _pocketHealth;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    public void Setup(Pocket pocket)
    {
        if(roomGate)
            roomGate.isLocked = true;

        this.pocket = pocket;
        _pocketHealth = pocket.GetHealth();

        if (removeFirstInMultiplayer)
		{
            if (FindObjectsOfType<PlayerInputController>().Length > 1)
            {
                tutorials.RemoveAt(0);
            }
        }
    }

    void Update()
    {
        if (ScreenManager.currentScreen == Screens.Tutorial)
        {
            
        }

        if (pocket == null)
            pocket = FindObjectOfType<Player>().currentPocket;
        else
        {
            _pocketHealth = pocket.GetHealth();

            if (_pocketHealth.GetCurrentPercentage() <= .15f)
                _pocketHealth.SetHealth(_pocketHealth.GetMaxHealth());
        }

        if (tutorialIndex <= tutorials.Count - 1)
        {
            for (int i = 0; i < tutorials.Count; i++)
            {
                if (i == tutorialIndex)
                    tutorials[i].SetActive(true);
                else
                    tutorials[i].SetActive(false);
            }

            if (closeGate)
            {
                closeGate.Close(true);
                closeGate.isLocked = true;
            }

            
        }
        else
        {
            foreach (var item in tutorials)
            {
                item.SetActive(false);
            }

            if(!lastTutorial)
                gameObject.SetActive(false);
            else
            {
                if (blackFade)
                {
                    if (blackFade.alpha < 1)
                        blackFade.alpha += Time.deltaTime;
                    else
                    {
                        if (!completed)
                        {
                            //ScreenManager.Instance.ChangeScreen(Screens.MainMenu, true);
                            FindObjectOfType<EnterTutorial>().ExitTutorial();
                            completed = true;
                        }
                    }
                }
                else
                {
                    if (!completed)
                    {
                        //ScreenManager.Instance.ChangeScreen(Screens.MainMenu, true);
                        FindObjectOfType<EnterTutorial>().ExitTutorial();
                        completed = true;
                    }
                }
            }

            if (roomGate)
            {
                if(shouldUnlockGate)
                    roomGate.isLocked = false;
            }
        }
    }

    public void NextScreen()
    {
        if(tutorialIndex < tutorials.Count)
            tutorialIndex++;
    }

    public void HatchEgg()
    {
        if (pocket != null)
        {
            pocket.Hatch();
        }
    }

    public void SetComplete(bool set)
    {
        completed = set;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
