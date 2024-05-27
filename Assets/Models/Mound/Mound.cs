using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mound : MonoBehaviour
{
    [SerializeField]
    Material MoundMaterial;
    Material m_MoundMaterialInstance;
    [SerializeField]
    MoundSlot MoundSlot;

    void Awake()
    {
        m_MoundMaterialInstance = new Material(MoundMaterial);
        GetComponent<MeshRenderer>().material = m_MoundMaterialInstance;
        MoundSlot.OnDropletInRange.AddListener(BecomeWet);
        //BecomeDry();
        //BecomeWet();
    }


    public void BecomeWet()
    {
        m_MoundMaterialInstance.SetFloat("_TimeOffset", Time.time);
    }
    public void BecomeDry()
    {
        m_MoundMaterialInstance.SetFloat("_TimeOffset", 99999);
    }
}
