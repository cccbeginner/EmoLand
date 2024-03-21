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
    public UnityEvent<int> OnResize;
    public bool isGrounded { get; private set; }

    [Networked]
    public bool isEatable { get; set; }
    [Networked]
    private int m_Size { get; set; }
    public int size
    {
        get
        {
            return m_Size;
        }
        set
        {
            if (value < 0) return;
            if (!HasStateAuthority) return;
            m_Size = value;
            if (m_Size < SizeMin) m_Size = SizeMin;
            if (m_Size > SizeMax) m_Size = SizeMax;
            ReloadSize();
            OnResize.Invoke(m_Size);
        }
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            slimeAnimator = GetComponentInChildren<Animator>();
            rigidBody = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
            m_Size = InitSize;
            size = InitSize;
            m_IsGroundedPrevious = true;

            OnLeaveGround.AddListener(LeaveGroundAnime);
            OnTouchGround.AddListener(TouchGroundAnime);
        }
    }

    private void ReloadSize()
    {
        float sqrt3 = Mathf.Pow(m_Size, 1f / 3f);
        transform.localScale = sqrt3 * UnitSizeScale;
    }

    public void EatAnime()
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

    public void FixedUpdate()
    {
        // Raycast for ground test
        float r = sphereCollider.radius;
        int collideMask = LayerMask.GetMask("Static Object", "Rigid Object", "Water");

        // Make a down raycast to environment.
        bool isRayHit = Physics.Raycast(transform.position + r * Vector3.up, -Vector3.up, out RaycastHit hit, r + 0.1f, collideMask);
        if (isRayHit)
        {
            // Set position if ray hit.
            // This may fix problem of going through collider unexpectedly.
            transform.position = hit.point;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // Grounded Related Events
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

    private void OnCollisionEnter(Collision collision)
    {
        if (HasStateAuthority)
        {
            Droplet another = collision.gameObject.GetComponent<Droplet>();

            if (another != null)
            {
                int eatResult = DecideWhoEat(another);
                if (eatResult == 1)
                {
                    EatDroplet(another);
                }
                else if (eatResult == 0)
                {
                    BeEatenByDroplet(another);
                }
            }
        }
    }

    private int DecideWhoEat(Droplet another)
    {
        // return -1 => no action, 0 => be eaten, 1=> is eater
        if (!another.isEatable)
        {
            if (isEatable)
            {
                // You are player, I am droplet
                return 0;
            }
            else
            {
                // Both player, do nothing
                return -1;
            }
        }
        else
        {
            if (!isEatable)
            {
                // I am player, You are droplet
                return 1;
            }
            else
            {
                // Both droplet
                bool isEater = false;
                if (another.transform.localScale.sqrMagnitude != gameObject.transform.localScale.sqrMagnitude)
                {
                    isEater = (another.transform.localScale.sqrMagnitude < gameObject.transform.localScale.sqrMagnitude);
                }
                else if (another.transform.position.y != gameObject.transform.position.y)
                {
                    isEater = (another.transform.position.y < gameObject.transform.position.y);
                }
                else if (another.transform.position.x != gameObject.transform.position.x)
                {
                    isEater = (another.transform.position.x < gameObject.transform.position.x);
                }
                else if (another.transform.position.z != gameObject.transform.position.z)
                {
                    isEater = (another.transform.position.z < gameObject.transform.position.z);
                }
                if (isEater) return 1;
                else return 0;
            }
        }
    }

    private void EatDroplet(Droplet another)
    {
        size += another.size;
        EatAnime();
    }

    private void BeEatenByDroplet(Droplet another)
    {
        sphereCollider.enabled = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        StartCoroutine(DelayEaten(another));
    }

    IEnumerator DelayEaten(Droplet another)
    {
        Vector3 targetPos = Vector3.Lerp(another.transform.position, transform.position, size / (another.size + size));
        while (true)
        {
            Vector3 distVec = targetPos - transform.position;
            Vector3 moveVec = distVec.normalized * Time.deltaTime * 5 / distVec.magnitude;
            if (moveVec.sqrMagnitude <= distVec.sqrMagnitude)
            {
                transform.position += moveVec;
            }
            else
            {
                transform.position += distVec;
            }
            if (distVec.sqrMagnitude < 0.1)
            {
                break;
            }
            yield return null;
        }
        Runner.Despawn(GetComponent<NetworkObject>());
    }
}
