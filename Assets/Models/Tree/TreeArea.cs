using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeArea : MonoBehaviour
{
    [SerializeField] GameObject Trunk;
    [SerializeField] ParticleSystem LeafVolume;
    [SerializeField] Light SpotLight;
    [SerializeField] Material TrunkMaterialBright, TrunkMaterialDark;
    [SerializeField] Material LeafMaterialBright, LeafMaterialDark;

    private bool _switch = false;
    [SerializeField]
    bool Switch = false;

    private void Start()
    {
        TurnOff();
    }
    private void Update()
    {
        if (_switch != Switch)
        {
            _switch = Switch;
            if (_switch ) TurnOn();
            else TurnOff();
        }
    }

    public void TurnOn()
    {
        SpotLight.enabled = true;
        Trunk.GetComponent<MeshRenderer>().material = TrunkMaterialBright;
        LeafVolume.GetComponent<ParticleSystemRenderer>().sharedMaterial = LeafMaterialBright;
    }

    public void TurnOff()
    {
        SpotLight.enabled = false;
        Trunk.GetComponent<MeshRenderer>().material = TrunkMaterialDark;
        LeafVolume.GetComponent<ParticleSystemRenderer>().sharedMaterial = LeafMaterialDark;
    }
}
