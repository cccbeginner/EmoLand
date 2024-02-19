using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBoom : NetworkBehaviour
{
    private ParticleSystem m_ParticalSystem;
    private Player m_MainPlayer;
    private bool FirstUpdate = true;

    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        m_ParticalSystem = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        m_MainPlayer = Player.main;
        m_ParticalSystem.Stop();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if (FirstUpdate)
        {
            m_ParticalSystem.transform.position = m_MainPlayer.transform.position;
            m_MainPlayer.Size -= 1;
            m_MainPlayer.AddForce(Vector3.one);
            m_ParticalSystem.Play();
            FirstUpdate = false;
        }
    }
}
