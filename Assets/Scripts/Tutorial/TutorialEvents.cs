using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvents : MonoBehaviour
{
    [Header("Tutorial")]
    public Tutorial tutorial;
    public TutorialTrigger trigger;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!TutorialTrigger.IsOnTutorial()) return;

        if (!tutorial && GetComponent<TutorialEgg>())
        {
            FindTutorial();
        }
    }

    

    void FindTutorial()
    {
        GameObject tut = GameObject.Find("TutBackground").transform.GetChild(0).gameObject;

        if(tut.activeSelf == true)
        {
            tutorial = GameObject.Find("TutorialText (EGG)").GetComponent<Tutorial>();
        }
    }

    private void OnDestroy()
    {
        if (tutorial)
            CompleteTutorial();

        if (trigger)
        {
            trigger.RemoveEnemy(gameObject);
        }
    }

    public void CompleteTutorial()
    {
        //if (!TutorialTrigger.IsOnTutorial()) return;
        tutorial.CompleteTutorial();
    }
}
