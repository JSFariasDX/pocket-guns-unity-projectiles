using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class LoadingScreen : MonoBehaviour
{
    [Header("Variables")]
    public float loadingProgress;

    [Header("Scenes")]
    bool canLoad;

    [Header("UI")]
    public Image loadingBar;
    public Image loadingButton;
    public CanvasGroup loadingScreen;

    // Start is called before the first frame update
    void Start()
    {
        loadingScreen = GetComponent<CanvasGroup>();

        canLoad = false;

        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        loadingBar.transform.parent.gameObject.SetActive(true);
        loadingButton.gameObject.SetActive(false);

        InputSystem.onAnyButtonPress.Call(ctx => SetCanLoad());
    }

    // Update is called once per frame
    void Update()
    {
        if (!loadingBar) return;

        #region UI
        loadingBar.fillAmount = loadingProgress;

        if (loadingProgress >= 1)
        {
            loadingBar.transform.parent.gameObject.SetActive(false);
            loadingButton.gameObject.SetActive(true);
        }
        #endregion
    }

    public void LoadWithLoadingScreen(Screens screen)
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        StartCoroutine(LoadAsync(screen));
    }

    IEnumerator LoadAsync(Screens screen)
    {
        //SetLoadScene(false);

        AsyncOperation operation = SceneManager.LoadSceneAsync(screen.ToString());
        if (screen != Screens.MainMenu)
        {
            operation.allowSceneActivation = false;
        }

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingProgress = progress;
            //print(loadingProgress);

            if (progress >= 1 && canLoad)
            {
                //print("Please load scene");
                yield return new WaitForSeconds(.5f);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void SetCanLoad()
    {
        //if (loadingProgress >= 1)
        //{
        //    if (!canLoad)
        //    {
        //        GetComponent<CanvasGroup>().alpha = 0;
        //        GetComponent<CanvasGroup>().blocksRaycasts = false;
        //        canLoad = true;
        //    }
        //}
    }
}
