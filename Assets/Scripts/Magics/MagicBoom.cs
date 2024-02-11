using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBoom : NetworkBehaviour
{
    private ParticleSystem m_ParticalSystem;
    private SceneObject m_SceneObject;
    private GameObject m_MainPlayer;
    private bool FirstUpdate = true;

    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        m_SceneObject = GetComponent<SceneObject>();
        m_ParticalSystem = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        m_MainPlayer = m_SceneObject.GetMainPlayer();
        m_ParticalSystem.Stop();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if (FirstUpdate)
        {
            m_ParticalSystem.transform.position = m_MainPlayer.transform.position;
            m_MainPlayer.GetComponent<PlayerController>().Size /= 2;
            m_MainPlayer.GetComponent<PlayerController>().AddForce(Vector3.one);
            m_ParticalSystem.Play();
            FirstUpdate = false;
        }
    }
}
