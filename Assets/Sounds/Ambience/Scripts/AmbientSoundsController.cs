using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundsController : MonoBehaviour
{
    public AmbientSounds sounds;
    AudioSource source;

    float actualInterval;
    private Coroutine _playCoroutine;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void SetupAmbientSounds(AmbientSounds soundPack, bool startSounds = false)
    {
        if (_playCoroutine != null)
            StopCoroutine(_playCoroutine);

        sounds = soundPack;

        if (startSounds)
        {
            PlayBackground();
            StartSounds();
        }
    }

    IEnumerator PlaySounds()
    {
        while (true)
        {
            actualInterval = Random.Range(sounds.minInterval, sounds.maxInterval);

            source.PlayOneShot(sounds.ambientSounds[Random.Range(0, sounds.ambientSounds.Count)], sounds.volumeMultiplier);
            //print("PLAY: " + actualInterval);

            yield return new WaitForSeconds(actualInterval);
        }
    }

    public void StartSounds()
    {
        if(sounds && sounds.ambientSounds.Count > 0)
            _playCoroutine = StartCoroutine(PlaySounds());
    }

    public void PlayBackground()
    {
        if (sounds.backgroundSound)
        {
            source.clip = sounds.backgroundSound;
            source.Play();
        }
        else
            source.Stop();
    }
}
