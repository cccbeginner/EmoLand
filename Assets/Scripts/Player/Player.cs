using Fusion;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{ 
    public static Player main { get; private set; }
    public PlayerMove playerMove { get { return GetComponent<PlayerMove>(); } }
    public PlayerJump playerJump { get { return GetComponent<PlayerJump>(); } }
    public PlayerSize playerSize { get { return GetComponent<PlayerSize>(); } }
    public int Size { get { return playerSize.Size; } }

    // Add events
    public UnityEvent OnLeaveGround;
    public UnityEvent OnTouchGround;
    public UnityEvent<ControllerColliderHit> OnHitCollider;

    public Camera PlayerCamera;
    public CharacterController PlayerController { get; private set; }
    public Animator SlimeAnimator { get; private set; }

    public float GravityValue = -9.81f;
    public float AirResistence = 0.1f;
    public float GroundResistence = 0.4f;
    public float GroundedMinSpeed = 0.5f;

    [Networked]
    bool nt_isGroundedCurrent { get; set; }
    bool m_IsGroundedPrevious = true;
    public bool IsGrounded { get {  return m_IsGroundedPrevious; } }

    private void Awake()
    {
        PlayerController = GetComponent<CharacterController>();
        SlimeAnimator = GetComponentInChildren<Animator>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            PlayerCamera = Camera.main;
            PlayerCamera.GetComponent<ThirdPersonCamera>().Target = transform;
            m_IsGroundedPrevious = true;
            m_GravityMultiplier = 1f;
            main = this;

            // Re-enable character controller to make it reset transform.
            PlayerController.enabled = false;
            PlayerController.enabled = true;

            m_PrevPos = PlayerController.transform.position;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (HasStateAuthority)
        {
            main = null;
        }
    }

    [Networked]
    private Vector3 m_Impact { get; set; }
    [Networked]
    private Vector3 m_ConstantImpact { get; set; }
    [Networked]
    private float m_GravityMultiplier { get; set; }
    public float GravityMultiplier
    {
        get { return m_GravityMultiplier; }
        set { m_GravityMultiplier = value; }
    }
    private Vector3 NT_GetImpacts()
    {
        // Apply gravity.
        m_Impact += m_GravityMultiplier * GravityValue * Runner.DeltaTime * Vector3.up;

        // Apply resistence.
        float remainPortion = 1 - AirResistence * Runner.DeltaTime;
        if (PlayerController.isGrounded)
        {
            remainPortion -= GroundResistence * Runner.DeltaTime;
        }

        Vector3 ret = (m_Impact + m_ConstantImpact) * remainPortion;

        // Modify impact if they are counteract.
        Vector3 newImpact = m_Impact;
        if (newImpact.x * m_ConstantImpact.x < 0f)
        {
            if (Mathf.Abs(newImpact.x) > Mathf.Abs(m_ConstantImpact.x)) newImpact.x -= m_ConstantImpact.x;
            else newImpact.x = 0f;
        }
        if (newImpact.y * m_ConstantImpact.y < 0f)
        {
            if (Mathf.Abs(newImpact.y) > Mathf.Abs(m_ConstantImpact.y)) newImpact.y -= m_ConstantImpact.y;
            else newImpact.y = 0f;
        }
        if (newImpact.z * m_ConstantImpact.z < 0f)
        {
            if (Mathf.Abs(newImpact.z) > Mathf.Abs(m_ConstantImpact.z)) newImpact.z -= m_ConstantImpact.z;
            else newImpact.z = 0f;
        }

        m_Impact = newImpact * remainPortion;

        return ret;
    }

    private void NT_ClampCeilGround()
    {
        // Test if on the ground or touch ceiling.
        if ((PlayerController.collisionFlags & CollisionFlags.Below) != 0 && m_Impact.y < 0)
        {
            m_Impact = new Vector3(m_Impact.x, 0, m_Impact.z);
        }
        if ((PlayerController.collisionFlags & CollisionFlags.Above) != 0 && m_Impact.y > 0)
        {
            m_Impact = new Vector3(m_Impact.x, 0, m_Impact.z);
        }
    }

    public Vector3 Velocity { get { return m_Velocity; } }
    [Networked]
    private Vector3 m_Velocity { get; set; }
    [Networked]
    private Vector3 m_PrevPos { get; set; }
    private void NT_ExposeVelocity()
    {
        m_Velocity = (transform.position - m_PrevPos) / Runner.DeltaTime;
        m_PrevPos = transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false)
        {
            return;
        }

        NT_ClampCeilGround();
        Vector3 otherVec = NT_GetImpacts();
        if (PlayerController.isGrounded && otherVec.sqrMagnitude < GroundedMinSpeed){
            m_Impact = Vector3.zero;
        }
        PlayerController.Move(otherVec * Runner.DeltaTime);
        

        // Expose velocity
        NT_ExposeVelocity();

        // Reset variables at end.
        nt_isGroundedCurrent = PlayerController.isGrounded;
    }

    public override void Render()
    {
        // Detect Grounded
        if (nt_isGroundedCurrent == false && m_IsGroundedPrevious == true)
        {
            OnLeaveGround.Invoke();
        }
        if (nt_isGroundedCurrent == true && m_IsGroundedPrevious == false)
        {
            OnTouchGround.Invoke();
        }
        m_IsGroundedPrevious = nt_isGroundedCurrent;
    }

    public void EatTrigger()
    {
        SlimeAnimator.SetTrigger("Eat");
    }

    public void AddImpact(Vector3 force)
    {
        m_Impact += force;
    }

    public void AddConstantImpact(Vector3 force)
    {
        m_ConstantImpact += force;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        OnHitCollider.Invoke(hit);
    }
}