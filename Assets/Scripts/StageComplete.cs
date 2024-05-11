using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StageComplete : MonoBehaviour
{
    public float GoodMudRadius = 0;
    public float RibbonAlphaMask = -5;
    public float EggContourAlpha = 1;
    [SerializeField] Material GroundMaterial;
    [SerializeField] Material RibbonMaterial;
    [SerializeField] Material EggContourMaterial;
    [SerializeField] GameObject AnimeCamera;
    [SerializeField] ParticleSystem EggHonkParticle;
    [SerializeField] GameObject UITransition;

    private void Start()
    {
        DisableAnime();
    }

    public void InitDone()
    {
        SetMudRadius(150);
        SetRibbonAlphaMask(5);
        SetEggContourAlpha(1);
    }

    public void InitNotDone()
    {
        SetMudRadius(0);
        SetRibbonAlphaMask(-5);
        SetEggContourAlpha(1);
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

    void SetEggContourAlpha(float alpha)
    {
        EggContourMaterial.SetFloat("_AlphaMultiplier", alpha);
    }

    public void EggHonk()
    {
        EggHonkParticle.Play();
    }

    void EnableAnime()
    {
        GetComponent<PlayableDirector>().Play();
        ThirdPersonCamera.main.GetComponent<Camera>().enabled = false;
        AnimeCamera.SetActive(true);
    }
    void DisableAnime()
    {
        ThirdPersonCamera.main.GetComponent<Camera>().enabled = true;
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
            SetEggContourAlpha(EggContourAlpha);
            yield return null;
        }
        DisableAnime();
    }
}
