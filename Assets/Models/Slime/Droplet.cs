using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;

public class Droplet : NetworkBehaviour
{
    public Vector3 UnitSizeScale;//(1,1,1)
    public float UnitSizeRadius = 0.25f;
    public int InitSize = 1;
    public int SizeMin = 1;
    public int SizeMax = 10;

    private bool m_IsGroundedPrevious;
    public Rigidbody rigidBody { get; private set; }
    public SphereCollider sphereCollider { get; private set; }
    public Animator slimeAnimator { get; private set; }

    public UnityEvent OnLeaveGround;
    public UnityEvent OnTouchGround;
    public bool isGrounded
    {
        get
        {
            return Physics.Raycast(transform.position+0.1f*Vector3.up, -Vector3.up, 0.2f);
        }
    }

    public UnityEvent<int> OnResize;
    private int m_Size;
    public int size
    {
        get
        {
            return m_Size;
        }
        set
        {
            if (value < 0) return;
            m_Size = value;
            if (m_Size < SizeMin) m_Size = SizeMin;
            if (m_Size > SizeMax) m_Size = SizeMax;
            ReloadSize();
            OnResize.Invoke(m_Size);
        }
    }

    private void Awake()
    {
        slimeAnimator = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        size = InitSize;
        m_IsGroundedPrevious = true;
    }

    private void ReloadSize()
    {
        float sqrt3 = Mathf.Pow(m_Size, 1f / 3f);
        transform.localScale = sqrt3 * UnitSizeScale;
    }

    public void EatTrigger()
    {
        slimeAnimator.SetTrigger("Eat");
    }
    private void LeaveGroundAnime()
    {
        slimeAnimator.ResetTrigger("Grounded");
        slimeAnimator.SetTrigger("Jump");

    }
    private void TouchGroundAnime()
    {
        slimeAnimator.ResetTrigger("Jump");
        slimeAnimator.SetTrigger("Grounded");
    }

    public override void Spawned()
    {
        OnLeaveGround.AddListener(LeaveGroundAnime);
        OnTouchGround.AddListener(TouchGroundAnime);
    }

    public void FixedUpdate()
    {
        // Detect Grounded
        if (isGrounded == false && m_IsGroundedPrevious == true)
        {
            OnLeaveGround.Invoke();
        }
        if (isGrounded == true && m_IsGroundedPrevious == false)
        {
            OnTouchGround.Invoke();
        }
        m_IsGroundedPrevious = isGrounded;
    }
}
