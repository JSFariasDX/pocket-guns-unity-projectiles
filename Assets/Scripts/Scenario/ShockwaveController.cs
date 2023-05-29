using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveController : MonoBehaviour
{
    [Header("Components")]
    public ParticleSystem particle;
    CircleCollider2D shockwaveCollider;
    PlayerControls controls;

    [Header("Damage")]
    [Range(0, 1)] public float damageDistancePercentage;
    float damageDistance;
    public float damage = 10;
    float particleTime;
    float timePercentage;
    float timeAlive;
    float colliderRadius;
    float innerRadius;

    [HideInInspector]
    public bool canDamage = false;

    [Tooltip("Mesma curva do Size over Lifetime do Particle System")]
    public AnimationCurve sizeCurve;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.UI.Submit.performed += _ => ResetEvent();

        damageDistance = damageDistancePercentage;
    }

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        shockwaveCollider = GetComponent<CircleCollider2D>();

        colliderRadius = (particle.main.startSizeMultiplier / 2) - ((particle.main.startSizeMultiplier / 2) * .078f);

        shockwaveCollider.radius = 0;

        particleTime = particle.main.startLifetimeMultiplier;
        timePercentage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (particle.isPlaying)
        {
            if (timePercentage < particleTime)
                timePercentage += Time.deltaTime;
        }
        else
        {
            timePercentage = 0;
        }

        timeAlive = timePercentage / particleTime;

        shockwaveCollider.radius = Mathf.Lerp(0, colliderRadius, sizeCurve.Evaluate(timeAlive));
        innerRadius = Mathf.Lerp(0, colliderRadius - (colliderRadius * damageDistance), sizeCurve.Evaluate(timeAlive));

        if (timeAlive >= .7f)
            damageDistance = 0;
        else
            damageDistance = damageDistancePercentage;
    }

    public void Setup(float duration, float startSize)
    {
        ParticleSystem.MainModule main = particle.main;

        main.duration = duration;
        main.startSize = startSize;
    }

    public void ResetEvent()
    {
        timePercentage = 0;
        particle.Play();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerCollider"))
        {
            float distance = Vector2.Distance(collision.transform.position, transform.position);

            if(distance < shockwaveCollider.radius && distance > innerRadius)
            {
                canDamage = true;
            }
            else
            {
                canDamage = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        float desiredRadius = Mathf.Lerp(0, colliderRadius - (colliderRadius * damageDistance), sizeCurve.Evaluate(timeAlive));

        Gizmos.DrawWireSphere(transform.position, desiredRadius);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
