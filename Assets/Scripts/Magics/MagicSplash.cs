using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : NetworkBehaviour
{
    private ParticleSystem m_ParticalSystem;
    private SceneObject m_SceneObject;
    private GameObject m_MainPlayer;
    private CharacterController m_PlayerController;
    private Vector3 m_PlayerPos;

    [Networked]
    bool nt_EnableSplash {  get; set; }

    public override void Spawned()
    {
        m_ParticalSystem = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        m_ParticalSystem.Stop();
        TryGetData();
    }

    private void TryGetData()
    {
        m_SceneObject = GetComponent<SceneObject>();
        m_MainPlayer = m_SceneObject.SceneManager.MainPlayer?.gameObject;
        if (m_MainPlayer == null) return;
        m_PlayerController = m_MainPlayer.GetComponent<CharacterController>();
        m_PlayerPos = m_MainPlayer.transform.position;
    }


    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if (m_MainPlayer == null)
        {
            TryGetData();
            if (m_MainPlayer == null)
            {
                return;
            }
        }
        Vector3 currentPos = m_MainPlayer.transform.position;

        // Follow main player.
        transform.position = currentPos;
        transform.forward = m_MainPlayer.transform.forward;

        // Enable splash if player is moving on ground, disable vice versa.
        if (m_PlayerController.isGrounded && (currentPos - m_PlayerPos).magnitude >= Time.deltaTime)
        {
            nt_EnableSplash = true;
        }
        else
        {
            nt_EnableSplash = false;
        }
        m_PlayerPos = currentPos;
    }
    public override void Render()
    {
        if (m_MainPlayer == null)
        {
            return;
        }
        if (nt_EnableSplash == true && m_ParticalSystem.isPlaying == false)
        {
            m_ParticalSystem.Play();
        }
        else if (nt_EnableSplash == false && m_ParticalSystem.isPlaying == true)
        {
            m_ParticalSystem.Stop();
        }
    }
}
