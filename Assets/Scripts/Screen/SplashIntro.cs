using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class SplashIntro : MonoBehaviour
{
    private Coroutine _autoCoroutine;

    void Start()
    {
        _autoCoroutine = StartCoroutine(NextScreenCoroutine());
        InputSystem.onAnyButtonPress.CallOnce(ctrl => GoToNextScreenManual());
    }

    private IEnumerator NextScreenCoroutine()
    {
        yield return new WaitForSeconds(4.5f);
        GoToNextScreenAuto();
    }

    void GoToNextScreenAuto()
    {
        SceneManager.LoadScene("Splash");
    }

    private void GoToNextScreenManual()
    {
        if (_autoCoroutine != null && SceneManager.GetActiveScene().name == "SplashIntro")
        {
            StopCoroutine(_autoCoroutine);
            GoToNextScreenAuto();
        }
    }
}
