using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.AI;

public class SinisterBossRoom : MonoBehaviour
{
    [Header("Components")]
    public Collider2D cameraCollider;

    [Header("Jumpscare")]
    public Image jumpscareImage;

    [Header("Maps")]
    public GameObject map1;
    public GameObject map2;
    public GameObject minimapObject;

    [Header("Boss")]
    public GameObject sinisterShadow;
    public Transform clonePoints;
    public float targetRadius = 15;
    Transform target;
    Vector3 targetDirection;

    [Header("Boundaries")]
    bool isInLevel2 = false;

    // Start is called before the first frame update
    void Start()
    {
        target = Camera.main.transform.Find("TargetGroup");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistanceFromCenter();
        SetClonePointsPosition();
    }

    public void SetupScene()
    {
        StartCoroutine(JumpscareCountdown());
    }

    public void TouchedPlayer()
    {
        StartCoroutine(Jumpscare());
    }

    IEnumerator JumpscareCountdown()
    {
        yield return new WaitForSeconds(6);

        if(!sinisterShadow.GetComponent<SinisterShadow>().awakening)
            StartCoroutine(Jumpscare());
    }

    IEnumerator Jumpscare()
    {
        //yield return new WaitForSeconds(7f);

        jumpscareImage.color = Color.white;
        sinisterShadow.GetComponent<SinisterShadow>().DeactivateAwakening();

        yield return new WaitForSeconds(0.1f);

        jumpscareImage.color = Color.black;
        map1.SetActive(false);
        map2.SetActive(true);
        minimapObject.SetActive(false);
        FindObjectOfType<NavMeshSurface>().BuildNavMeshAsync();
        sinisterShadow.transform.localPosition = Vector2.zero;

        Player[] players = GameplayManager.Instance.GetPlayers(false).ToArray();
        foreach (var item in players)
        {
            item.isInWater = true;
        }

        yield return new WaitForSeconds(1f);

        jumpscareImage.color = Color.clear;
        sinisterShadow.GetComponent<SinisterShadow>().Setup();
        map2.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Transition");
        CinemachineConfiner2D confiner = Camera.main.GetComponentInChildren<CinemachineConfiner2D>();

        //confiner.m_BoundingShape2D = null;
        confiner.InvalidateCache();
        confiner.m_BoundingShape2D = cameraCollider;

        isInLevel2 = true;
    }

    void SetClonePointsPosition()
    {
        targetDirection = target.position - map2.transform.position;
        targetDirection = Vector2.ClampMagnitude(targetDirection, targetRadius);
        clonePoints.position = Vector2.Lerp(clonePoints.position, map2.transform.position + targetDirection, 5 * Time.deltaTime);
    }

    void CalculateDistanceFromCenter()
    {
        if (!isInLevel2) return;
     
        List<Player> player = GameplayManager.Instance.GetPlayers(true);

        for (int i = 0; i < player.Count; i++)
        {
            float distance = Vector2.Distance(player[i].transform.position, map2.transform.position);

            if (distance >= 25 && !player[i].GetHealth().isInvulnerable) player[i].GetHealth().Decrease(1, null, null, .25f);
        }
    }
}
