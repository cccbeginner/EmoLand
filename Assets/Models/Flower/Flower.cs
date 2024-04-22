using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField]
    Animator FlowerAnimator;
    [SerializeField]
    Material FlowerMaterial;

    public bool HasGrow {  get; private set; }
    public bool IsOpen { get; private set; }

    private void Start()
    {
        IsOpen = false;
        HasGrow = false;
        Close(0);
        NotGrow();
    }

    public void NotGrow()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void GrowAndOpen()
    {
        Grow();
        Open(2f);
    }

    public void Grow()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        FlowerAnimator.SetTrigger("Grow");
    }

    public void Open(float timeDelay)
    {
        IsOpen = true;
        FlowerMaterial.SetInt("_Open", 1);
        FlowerMaterial.SetFloat("_TimeOffset", Time.fixedUnscaledTime + timeDelay);
    }

    public void Close(float timeDelay)
    {
        IsOpen = false;
        FlowerMaterial.SetInt("_Open", 0);
        FlowerMaterial.SetFloat("_TimeOffset", Time.fixedUnscaledTime + timeDelay);
    }

    public void Toggle(float timeDelay)
    {
        if (IsOpen) Close(timeDelay);
        else Open(timeDelay);
    }
}
