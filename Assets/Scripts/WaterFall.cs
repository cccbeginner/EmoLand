using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFall : MonoBehaviour
{
    [SerializeField] ParticleSystem TrailParticle;
    [SerializeField] ParticleSystem FogParticle;
    [SerializeField] ParticleSystem SplashParticle;

    void Start()
    {
        TrailParticle.Play();
        FogParticle.Play();
        SplashParticle.Play();
    }
}
