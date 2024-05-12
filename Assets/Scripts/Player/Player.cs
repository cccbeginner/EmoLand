using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{ 
    public static Player main { get; private set; }
    public PlayerMove playerMove { get { return GetComponent<PlayerMove>(); } }
    public PlayerJump playerJump { get { return GetComponent<PlayerJump>(); } }
    public PlayerSprint playerSprint { get { return GetComponent<PlayerSprint>(); } }

    // Add events
    public UnityEvent OnLeaveGround;
    public UnityEvent OnTouchGround;

    public DropletNetwork droplet { get; private set; }
    public Rigidbody rigidBody { get { return droplet.rigidBody; } }
    public SphereCollider sphereCollider { get { return droplet.sphereCollider; } }
    public Animator slimeAnimator { get { return droplet.slimeAnimator; } }

    public override void Spawned()
    {
        droplet = GetComponent<DropletNetwork>();
        droplet.OnLeaveGround.AddListener(OnLeaveGround.Invoke);
        droplet.OnTouchGround.AddListener(OnTouchGround.Invoke);

        if (HasStateAuthority)
        {
            droplet.isEatable = false;
            main = this;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (HasStateAuthority)
        {
            main = null;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            return;
        }
    }
}