using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool hasStarted = false;

    [Header("Components")]
    public GameObject roomTutorial;

    [Header("last Tutorial")]
    public bool lastPart = false;
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject singleAdult;

    [Header("Dialogue")]
    public GameObject dialogueBalloon;
    public Dialogue monsterDialogue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lastPart)
        {
            if (roomTutorial)
            {
                if (enemies.Count <= 0 && !roomTutorial.activeSelf)
                    roomTutorial.SetActive(true);
            }
        }

        if (singleAdult)
        {
            if (hasStarted)
            {
                singleAdult.GetComponent<Health>().SetInvulnerability(false);
            }
            else
            {
                singleAdult.GetComponent<Health>().SetInvulnerability(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartTutorial();

        if (dialogueBalloon != null)
        {
            dialogueBalloon.SetActive(true);
            dialogueBalloon.GetComponent<DialogueController>().StartDialogue(monsterDialogue);
        }
    }

    public void RemoveEnemy(GameObject me)
    {
        if (enemies.Contains(me))
            enemies.Remove(me);
    }

    public void StartTutorial()
    {
        //if (!IsOnTutorial()) return;

        if (!roomTutorial.activeSelf)
        {
            roomTutorial.SetActive(true);
            hasStarted = true;
        }
    }

    public static bool IsOnTutorial()
	{
        return SceneManager.GetActiveScene().name.Contains("Tutorial");
	}
}
