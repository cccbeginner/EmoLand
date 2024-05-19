using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeArea : MonoBehaviour
{
    [SerializeField] GameObject Trunk;
    [SerializeField] ParticleSystem LeafVolume;
    [SerializeField] Light SpotLight;
    [SerializeField] ParticleSystem DustParticle;
    [SerializeField] Material TrunkMaterialBright, TrunkMaterialDark;
    [SerializeField] Material LeafMaterialBright, LeafMaterialDark;
    public bool isOn {  get; private set; }
    public void TurnOn()
    {
        isOn = true;
        SpotLight.enabled = true;
        Trunk.GetComponent<MeshRenderer>().material = TrunkMaterialBright;
        LeafVolume.GetComponent<ParticleSystemRenderer>().sharedMaterial = LeafMaterialBright;
        LeafVolume.Pause();
        DustParticle.GetComponent<ParticleSystemRenderer>().enabled = true;
        DustParticle.Clear();
        DustParticle.Play();
    }

    public void TurnOff()
    {
        isOn = false;
        SpotLight.enabled = false;
        Trunk.GetComponent<MeshRenderer>().material = TrunkMaterialDark;
        LeafVolume.GetComponent<ParticleSystemRenderer>().sharedMaterial = LeafMaterialDark;
        DustParticle.GetComponent<ParticleSystemRenderer>().enabled = false;
    }
}
