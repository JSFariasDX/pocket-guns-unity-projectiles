using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHelper : MonoBehaviour
{
    [HideInInspector] public bool IsHatchingEgg;

    [Header("Components")]
    TutorialsController tut;
    AudioSource source;
    public AudioClip eggSound;
    public AudioClip windingSound;
    public AudioClip hatchSound;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        tut = GetComponentInParent<TutorialsController>();
        source = GameObject.Find("PauseManager").GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (_animator != null)
            _animator.enabled = true;
    }

    private void StartHatch()
    {
        Time.timeScale = 0f;
    }

    public void HatchEgg()
    {
        if (tut.pocket)
        {
            tut.HatchEgg();
            source.pitch = .5f;
            source.PlayOneShot(hatchSound, 5f);
        }
    }

    public void EndHatch()
    {
        tut.pocket.EnableSpecial();
        tut.NextScreen();
        tut.tutorialIndex = 0;
        Time.timeScale = 1f;
        IsHatchingEgg = false;
        
        _animator.enabled = false;
        tut.gameObject.SetActive(false);
    }

    public void PlaySound(float pitch)
    {
        source.pitch = pitch;
        source.PlayOneShot(eggSound);
    }

    public void PlayWindingSound(float pitch)
    {
        source.pitch = pitch;
        source.PlayOneShot(windingSound, .5f);
    }
}
