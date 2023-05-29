using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParticle : MonoBehaviour
{
    public void Setup(float duration)
	{
        SetParticleSystemDuration(duration);
        //Destroy(GetComponent<AudioSource>(), duration);
        //Destroy(gameObject, duration + 10);
	}

    void SetParticleSystemDuration(float duration)
	{
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop();

        ParticleSystem.MainModule main = particleSystem.main;
        main.duration = duration + 3;
        
        particleSystem.Play();
    }
}
