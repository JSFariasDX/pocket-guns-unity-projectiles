using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
   private float count;
    [SerializeField] bool showFps;

    private IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnGUI()
    {
        if (showFps)
        {
            GUI.Label(new Rect(5, 100, 100, 25), "FPS: " + Mathf.Round(count));
        }
    }
}
