using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectController : MonoBehaviour
{
    [Header("Behaviour")]
    public Vector2 minMaxRadius;
    [Range(.2f, 1)]
    float radius;
    public Vector2 minMaxSpeed;
    float speed;
    public float minThreshold;
    float t;

    [Header("Death")]
    public GameObject deathEffect;

    // Start is called before the first frame update
    void Start()
    {
        radius = Random.Range(minMaxRadius.x, minMaxRadius.y);
        speed = Random.Range(minMaxSpeed.x, minMaxSpeed.y);

        if (speed < 0 && speed > -minThreshold)
            speed -= minThreshold;
        else if (speed > 0 && speed < minThreshold)
            speed += minThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        MoveLikeFly();
    }

    void MoveLikeFly()
    {
        t += Time.deltaTime;
        float sinVariable = (0.5f + 0.5f * (Mathf.Sin(t * 0.3f) + 0.3f * Mathf.Sin(2 * t + 0.8f) + 0.26f * Mathf.Sin(3 * t + 0.8f)));
        float radiusY = radius * Mathf.Clamp(sinVariable, Random.Range(0.3f, 0.5f), sinVariable);

        Vector3 desiredPosition = new Vector3(radius * Mathf.Cos(t * speed), radiusY * Mathf.Sin(t * (speed)), 0);

        transform.localPosition = desiredPosition;
    }

    public void Die()
    {
        if (deathEffect) 
        { 
            GameObject death = Instantiate(deathEffect, transform.position, Quaternion.identity);
            death.transform.localScale = new Vector3(.15f, .15f, .15f);
        }
        Destroy(gameObject);
    }
}
