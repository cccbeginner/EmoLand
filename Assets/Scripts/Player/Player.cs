using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{ 
    public static Player main { get; private set; }
    public PlayerMove playerMove { get { return GetComponent<PlayerMove>(); } }
    public PlayerJump playerJump { get { return GetComponent<PlayerJump>(); } }
    public PlayerSprint playerSprint { get { return GetComponent<PlayerSprint>(); } }

    // Add events
    public UnityEvent OnLeaveGround;
    public UnityEvent OnTouchGround;

    public DropletPlayer droplet { get; private set; }
    public Rigidbody rigidBody { get { return droplet.rigidBody; } }
    public SphereCollider sphereCollider { get { return droplet.sphereCollider; } }
    public Animator slimeAnimator { get { return droplet.slimeAnimator; } }
    public SlimeAudioPlayer slimeAudioPlayer { get { return droplet.slimeAudioPlayer; } }

    void Awake()
    {
        droplet = GetComponent<DropletPlayer>();
        droplet.OnLeaveGround.AddListener(OnLeaveGround.Invoke);
        droplet.OnTouchGround.AddListener(OnTouchGround.Invoke);

        droplet.isEatable = false;
        main = this;
    }
}