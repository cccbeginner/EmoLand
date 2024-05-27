using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Flower : MonoBehaviour
{
    [SerializeField]
    Animator FlowerAnimator;
    [SerializeField]
    Material FlowerMaterial;
    Material m_FlowerMaterialInstance;

    List<GameObject> m_Petals;

    public bool HasGrow {  get; private set; }
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        IsOpen = false;
        HasGrow = false;
        m_FlowerMaterialInstance = new Material(FlowerMaterial);
        FindPetals();
        AssignPetalMaterial();
        Close(0);
        //NotGrow();
        //Grow();
    }

    private void FindPetals()
    {
        m_Petals = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Petals")
            {
                foreach (Transform petal in child)
                {
                    m_Petals.Add(petal.gameObject);
                }
            }
        }
    }

    private void AssignPetalMaterial()
    {
        foreach (GameObject petal in m_Petals)
        {
            petal.GetComponent<MeshRenderer>().material = m_FlowerMaterialInstance;
        }
    }

    public void NotGrow()
    {
        HasGrow = false;
        FlowerAnimator.SetTrigger("DontGrow");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void GrowAndOpen()
    {
        Grow();
        Open(2.5f);
    }

    public void Grow()
    {
        HasGrow = true;
        FlowerAnimator.SetTrigger("Grow");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void Open(float timeDelay)
    {
        IsOpen = true;
        m_FlowerMaterialInstance.SetInt("_Open", 1);
        m_FlowerMaterialInstance.SetFloat("_TimeOffset", Time.time + timeDelay);

        foreach (GameObject petal in m_Petals)
        {
            petal.GetComponent<Collider>().enabled = true;
        }
    }

    public void Close(float timeDelay)
    {
        IsOpen = false;
        m_FlowerMaterialInstance.SetInt("_Open", 0);
        m_FlowerMaterialInstance.SetFloat("_TimeOffset", Time.time + timeDelay);

        foreach (GameObject petal in m_Petals)
        {
            petal.GetComponent<Collider>().enabled = false;
        }
    }

    public void Toggle(float timeDelay)
    {
        if (IsOpen) Close(timeDelay);
        else Open(timeDelay);
    }
}
