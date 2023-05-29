using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPoint : MonoBehaviour
{
    public string exitToSceneKey;
    // Start is called before the first frame update

    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("ExitPoint");
        //Debug.Log(other.tag);
        if (other.CompareTag("PlayerCollider"))
        {
            SceneManager.LoadScene(exitToSceneKey);
        }
    }
}
