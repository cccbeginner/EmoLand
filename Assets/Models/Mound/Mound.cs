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

    void Start()
    {
        m_MoundMaterialInstance = new Material(MoundMaterial);
        GetComponent<MeshRenderer>().material = m_MoundMaterialInstance;
        MoundSlot.OnDropletInRange.AddListener(BecomeWet);
        BecomeDry();
        //BecomeWet();
    }


    void BecomeWet()
    {
        m_MoundMaterialInstance.SetFloat("_TimeOffset", Time.time);
    }
    void BecomeDry()
    {
        m_MoundMaterialInstance.SetFloat("_TimeOffset", 99999);
    }
}
