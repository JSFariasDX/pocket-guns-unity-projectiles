using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))] [RequireComponent(typeof(AudioSource))]
public class TremorsController : MonoBehaviour
{
    CinemachineImpulseSource impulse;
    PlayerControls controls;

    [Header("Tremor Duration")]
    float duration;

    float durationTimer;

    [Header("Tremor Intensity")]
    [Range(0, 1)]
    public float minIntensity;
    [Range(1, 3)]
    public float maxIntensity = 1;

    [Header("Tremor Interval")]
    public float minInterval;
    public float maxInterval;
    float actualTremorInterval;
    float tremorTimer;

    //[HideInInspector]
    public bool trembling = false;

    [HideInInspector] public bool isCave = false;

    [Header("Particles")]
    public GameObject stones;

    [Header("SFX")]
    public AudioClip[] tremorSounds;
    public AudioClip[] stonesSounds;
    AudioClip currentSound;
    AudioSource source;
    
    private void Awake()
    {
        controls = new PlayerControls();

        //controls.Gameplay.Dash.performed += _ => CauseManmadeTremor();
    }

    // Start is called before the first frame update
    void Start()
    {
        impulse = GetComponent<CinemachineImpulseSource>();
        source = GetComponent<AudioSource>();

        //duration = Random.Range(minDuration, maxDuration);
        duration = SelectCurrentSound().length - (impulse.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime);

        tremorTimer = 0;
        durationTimer = 0;
        actualTremorInterval = Random.Range(minInterval, maxInterval);

        impulse.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = duration;
    }

    // Update is called once per frame
    void Update()
    {
        isCave = GameplayManager.Instance.currentDungeonType == DungeonType.Cave;

        float actualDuration = duration + impulse.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime + impulse.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime;

        if (!isCave) return;

        #region Clamps
        if (minInterval < actualDuration + 1)
            minInterval = actualDuration + 1;

        if (maxInterval < minInterval)
            maxInterval = minInterval;

        if (minIntensity < 0)
            minIntensity = 0;

        if (maxIntensity < minIntensity)
            maxIntensity = minIntensity;
        #endregion

        if (trembling)
            durationTimer += Time.deltaTime;
        else
        {
            if (tremorTimer < actualTremorInterval)
                tremorTimer += Time.deltaTime;
            else
            {
                Invoke("CauseNaturalTremor", 0);
                source.clip = currentSound;
                source.Play();
            }
        }

        if (durationTimer >= actualDuration)
        {
            if (trembling)
                duration = SelectCurrentSound().length - (impulse.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime);

            //RumbleControllers(0, 0);

            trembling = false;
        }

        ControlVolume(actualDuration - (impulse.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime * 2));
    }

    void CauseNaturalTremor()
    {
        if (trembling) return;

        float actualIntensity = Random.Range(minIntensity, maxIntensity);

        source.volume = 1;

        impulse.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = duration;
        impulse.GenerateImpulse(actualIntensity);

        SpawnParticle(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height)));

        actualTremorInterval = Random.Range(minInterval, maxInterval);

        print("NATURAL");

        //RumbleControllers(1, 1);

        tremorTimer = 0;
        durationTimer = 0;
        trembling = true;
    }

    public void CauseManmadeTremor()
    {
        if (trembling) return;

        if (!isCave) return;

        source.volume = 1;

        impulse.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = duration / 2;
        impulse.GenerateImpulse(maxIntensity * 1.25f);

        SpawnParticle(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height)));

        actualTremorInterval = Random.Range(minInterval, maxInterval);

        print("CAUSED");

        //RumbleControllers(1, 1);

        tremorTimer = 0;
        durationTimer = 0;
        trembling = true;
    }

    void RumbleControllers(float lowFrequency, float highFrequency)
    {
        List<Player> players = GameplayManager.Instance.GetPlayers(false);

        foreach (var item in players)
        {
            item.input.Rumble(lowFrequency, highFrequency);
        }
    }

    AudioClip SelectCurrentSound()
    {
        AudioClip current = tremorSounds[Random.Range(0, tremorSounds.Length)];
        currentSound = current;

        return current;
    }

    void ControlVolume(float actualDuration)
    {
        if (durationTimer >= actualDuration)
            source.volume -= Time.deltaTime;
    }

    void SpawnParticle(Vector2 position)
    {
        GameObject particle = Instantiate(stones.gameObject, Camera.main.transform);
        particle.transform.position = position;

        ParticleSystem actualParticle = particle.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule main = actualParticle.main;

        main.startDelay = impulse.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime / 2;
        main.duration = duration + impulse.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime;

        AudioSource audio = particle.GetComponent<AudioSource>();
        audio.clip = stonesSounds[Random.Range(0, stonesSounds.Length)];

        //print("SPAWN PARTICLE");

        actualParticle.Play();
        audio.Play();
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
