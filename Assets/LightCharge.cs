using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightCharge : MonoBehaviour
{
    [SerializeField]
    int maxCharge = 40;
    int charge = 0;
    [SerializeField]
    public float minIntensity;
    [SerializeField]
    public float maxIntensity = 2;

    public Vector2 minManInnerRadius;
    public Vector2 minManOuterRadius;

    [SerializeField]
    Light2D controlledLight;
    [SerializeField]
    float timeToUncharge = 5f;
    float dischargeTimer;
    bool isUncharging = false;

    private Coroutine unchargeRoutine;

    [Header("Sounds")]
    public List<AudioClip> chargingUp;
    public List<AudioClip> chargingDown;
    AudioSource source;

    private void Start()
    {
        HandleIntensity();

        dischargeTimer = timeToUncharge;
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleIntensity();

        if(charge > 0)
        {
            if (dischargeTimer >= 0)
                dischargeTimer -= Time.deltaTime;
        }
        else
        {
            dischargeTimer = timeToUncharge;
        }

        if (dischargeTimer <= 0)
        {
            if (charge > 0)
            {
                if(chargingDown.Count > 0)
                    source.PlayOneShot(chargingDown[Random.Range(0, chargingDown.Count)], Random.Range(.4f, .8f));
            }

            charge = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            IncrementCharge();
            //HandleIntensity();
        }
    }

    void IncrementCharge()
    {
        if (unchargeRoutine != null)
        {
            AbortUncharge();
        }
        if (charge < maxCharge)
        {
            charge++;
        }
        
        dischargeTimer = timeToUncharge;

        source.PlayOneShot(chargingUp[Random.Range(0, chargingUp.Count)], Random.Range(.4f, .8f));
    }

    void HandleIntensity()
    {
        float currentIntensityRate = ((float)charge / (float)maxCharge);
        if (currentIntensityRate >= 1)
        {
            GetComponent<LightPulse>().minimum = maxIntensity * 0.8f;
            GetComponent<LightPulse>().maximum = maxIntensity;
        } else if (currentIntensityRate >= 0.75)
        {
            GetComponent<LightPulse>().minimum = maxIntensity * 0.5f;
            GetComponent<LightPulse>().maximum = maxIntensity * 0.75f;
        } else if (currentIntensityRate >= 0.5)
        {
            GetComponent<LightPulse>().minimum = maxIntensity * 0.25f;
            GetComponent<LightPulse>().maximum = maxIntensity * 0.5f;
        } else if (currentIntensityRate >= 0.25)
        {
            GetComponent<LightPulse>().minimum = minIntensity;
            GetComponent<LightPulse>().maximum = maxIntensity * 0.25f;
        } else
        {
            GetComponent<LightPulse>().minimum = minIntensity * 0.5f;
            GetComponent<LightPulse>().maximum = minIntensity * 2;
        }

        GetComponent<Light2D>().pointLightInnerRadius = Mathf.Lerp(minManInnerRadius.x, minManInnerRadius.y, currentIntensityRate);
        GetComponent<Light2D>().pointLightOuterRadius = Mathf.Lerp(minManOuterRadius.x, minManOuterRadius.y, currentIntensityRate);
    }

    IEnumerator Uncharge(float unchargeRate)
    {
        isUncharging = true;
        int unchargeValue = Mathf.FloorToInt(maxCharge * 0.25f);
        var instruction = new WaitForEndOfFrame();
        while (unchargeRate > 0)
        {
            unchargeRate -= Time.deltaTime;
            yield return instruction;
        }
        if (charge > 0)
        {
            charge -= unchargeValue;
        }
        HandleIntensity();
        isUncharging = false;
    }

    private void AbortUncharge()
    {
        StopCoroutine(unchargeRoutine);
        unchargeRoutine = null;
        isUncharging = false;
    }
}
