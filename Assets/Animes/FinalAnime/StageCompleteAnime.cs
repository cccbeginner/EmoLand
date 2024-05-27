using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StageCompleteAnime : MonoBehaviour
{
    public float GoodMudRadius = 0;
    public float RibbonAlphaMask = -5;
    [SerializeField] Material GroundMaterial;
    [SerializeField] AudioSource GroundAudio;
    [SerializeField] Material RibbonMaterial;
    [SerializeField] AudioSource RibbonAudio1;
    [SerializeField] AudioSource RibbonAudio2;
    [SerializeField] AudioSource RibbonAudio3;
    [SerializeField] GameObject AnimeCamera;
    [SerializeField] ParticleSystem EggHonkParticle;
    [SerializeField] AudioSource EggHonkAudio;
    [SerializeField] GameObject UITransition;

    private void Start()
    {
        DisableAnime();
    }

    public void InitDone()
    {
        SetMudRadius(150);
        SetRibbonAlphaMask(5);
    }

    public void InitNotDone()
    {
        SetMudRadius(0);
        SetRibbonAlphaMask(-5);
    }

    void SetMudRadius(float r)
    {
        GroundMaterial.SetFloat("_InRadius", r);
        GroundMaterial.SetFloat("_OutRadius", r + 5);
    }

    void SetRibbonAlphaMask(float val)
    {
        RibbonMaterial.SetFloat("_AlphaMask", val);
    }

    public void EggHonk()
    {
        EggHonkParticle.Play();
        EggHonkAudio.Play();
    }

    public void PlayRibbonAudio1()
    {
        RibbonAudio1.Play();
    }
    public void PlayRibbonAudio2()
    {
        RibbonAudio2.Play();
    }
    public void PlayRibbonAudio3()
    {
        RibbonAudio3.Play();
    }
    public void PlayerGroundAudio()
    {
        GroundAudio.Play();
    }

    void EnableAnime()
    {
        GetComponent<PlayableDirector>().Play();
        ThirdPersonCamera.main.gameObject.SetActive(false);
        AnimeCamera.SetActive(true);
    }
    void DisableAnime()
    {
        ThirdPersonCamera.main.gameObject.SetActive(true);
        AnimeCamera.SetActive(false);
        EggHonkParticle.Stop();
    }

    public void StartFinalAnime()
    {
        StartCoroutine(SetValueRoutine());
    }

    IEnumerator SetValueRoutine()
    {
        EnableAnime();

        float time = 0;
        while (time <= GetComponent<PlayableDirector>().duration)
        {
            time += Time.deltaTime;
            SetMudRadius(GoodMudRadius);
            SetRibbonAlphaMask(RibbonAlphaMask);
            yield return null;
        }
        DisableAnime();
    }
}
