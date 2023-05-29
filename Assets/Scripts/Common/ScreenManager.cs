using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Screens {
    SplashScreen = 1,
    MainMenu = 2,
    FirstRoom = 3,
    QueenRoom = 4,
    Tutorial = 5,
    EndRun = 6,
    PlayerEntryMenu = 7, 
    Lobby = 12,
    PocketMor = 14
}

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;
    public static Screens currentScreen = Screens.SplashScreen;

    public float loadingProgress { get; set; }

    public bool canLoad;

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

        int ID = 32;
        print(ID.ToString("00") + "1");
    }

    public void ChangeScreen(Screens screen, bool needLoadingScreen)
    {
        currentScreen = screen;

        if (screen == Screens.MainMenu || screen == Screens.FirstRoom)
        {
            DestroyIfExists("Player");
            DestroyIfExists("HUDCanvas");
            DestroyIfExists("PauseManager");
            //DestroyIfExists("Shockwave");
        }

        if (needLoadingScreen)
        {
            if (GameObject.FindGameObjectWithTag("Loading"))
            {
                LoadingScreen loading = GameObject.FindGameObjectWithTag("Loading").GetComponent<LoadingScreen>();

                loading.LoadWithLoadingScreen(screen);
            }
            else
            {
                //Debug.LogError("In order to display the Loading Screen you FIRST need to have it on your Canvas");

                StartCoroutine(LoadAsync(screen));
            }
        }
        else
        {
            //SceneManager.LoadScene((int)screen);
            StartCoroutine(LoadAsync(screen));
        }
    }

    public void ChangeScreen(Screens screen)
    {
        ChangeScreen(screen, false);

        //currentScreen = screen;

        //if (screen == Screens.MainMenu || screen == Screens.FirstRoom)
        //{
        //    DestroyIfExists("HUDCanvas");
        //    DestroyIfExists("Player");
        //    DestroyIfExists("PauseManager");
        //}
        //    //SceneManager.LoadScene((int)screen);
        //    StartCoroutine(LoadAsync(screen));
    }

    void DestroyIfExists(string tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        if (obj != null)
        {
            print("<color=green>FOUND</color>");
            Destroy(obj);
        }
        else
        {
            print("<color=yellow>NOT FOUND</color>");
        }
    }

    public bool IsGameplay()
    {
        return (int)currentScreen > 2;
    }

    IEnumerator LoadAsync(Screens screen)
    {
        //SetLoadScene(false);

        AsyncOperation operation = SceneManager.LoadSceneAsync((int)screen);
        if (screen != Screens.MainMenu)
        {
            operation.allowSceneActivation = false;
        }

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingProgress = progress;
            //print(loadingProgress);

            if (progress >= 1)
            {
                //print("Please load scene");
                yield return new WaitForSeconds(.5f);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
