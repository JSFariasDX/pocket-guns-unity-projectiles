using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMove : MonoBehaviour
{
    public bool isActivated = false;
    public Transform[] movePoints;
    int currentPoint = 0;
    Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            Transform destination = movePoints[currentPoint];
            float speed = enemy.speed * Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, destination.position, speed);

            if (transform.position.x == destination.position.x)
            {
                GetNextPoint();
            }
        }
    }

    public void SetActivation(bool value)
    {
        isActivated = value;
    }

    void GetNextPoint()
    {
        if (currentPoint + 1 < movePoints.Length)
        {
            currentPoint++;
        } else
        {
            currentPoint = 0;
        }
    }
}
