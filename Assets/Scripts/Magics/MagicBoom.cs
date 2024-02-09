using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBoom : NetworkBehaviour
{
    private ParticleSystem m_ParticalSystem;
    private SceneObject m_SceneObject;
    private GameObject m_MainPlayer;

    public override void Spawned()
    {
        m_SceneObject = GetComponent<SceneObject>();
        m_ParticalSystem = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        m_ParticalSystem.transform.position = m_SceneObject.GetMainPlayer().transform.position;
        Debug.Log(transform.position);
    }
}
