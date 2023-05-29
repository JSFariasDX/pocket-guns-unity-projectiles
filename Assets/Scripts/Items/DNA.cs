using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : Collectible
{
    List<Transform> players = new List<Transform>();
    [Header("Setup")]
    public CircleCollider2D wallCollider;
    public float speed = 15;
    Rigidbody2D rb;

    [Header("Delay time")]
    public float delayTime = 1;
    float desiredDistance = 3;

    protected override void Start()
    {
        base.Start();
        canBeCollected = false;
        rb = GetComponent<Rigidbody2D>();

        for (int i = 0; i < GameplayManager.Instance.GetPlayers(true).Count; i++)
        {
            players.Add(GameplayManager.Instance.GetPlayers(true)[i].transform);
        }

        StartCoroutine(Delay(delayTime));

    }

    private void Update()
    {
        if (!canBeCollected) return;

        float distance = Vector2.Distance(GetClosestEnemy(players.ToArray()).position, transform.position);

        if (distance < desiredDistance)
        {
            float velocity = speed * Time.fixedDeltaTime;

            rb.MovePosition(Vector2.Lerp(transform.position, GetClosestEnemy(players.ToArray()).position, velocity));

            if (distance < 1)
                wallCollider.enabled = true;
            else
                wallCollider.enabled = false;
        }

        desiredDistance += Time.deltaTime;
    }

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);

        canBeCollected = true;
    }

    public override void onPlayerCollect(Player player)
    {
        if (!canBeCollected) return;

        int currentDNA = PlayerPrefs.GetInt("DNA", 0);
        currentDNA += amount;
        PlayerPrefs.SetInt("DNA", currentDNA);

        FindObjectOfType<DNAGui>().OpenCounter(1, amount);

        base.onPlayerCollect(player);
    }

    Transform GetClosestEnemy(Transform[] enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in enemies)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }
}
