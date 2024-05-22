using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropletPlayer : MonoBehaviour
{
    public Vector3 UnitSizeScale;//(1,1,1)
    public float UnitSizeRadius = 0.25f;
    public int SizeMin = 1;
    public int SizeMax { get
        {
            return PlayerDataSystem.currentStage + 1;
        }
    }


    private bool m_IsGroundedPrevious;
    public Rigidbody rigidBody { get { return GetComponent<Rigidbody>(); } }
    public SphereCollider sphereCollider { get { return GetComponent<SphereCollider>(); } }
    public Animator slimeAnimator { get { return GetComponentInChildren<Animator>(); } }
    public SlimeAudioPlayer slimeAudioPlayer { get { return GetComponentInChildren<SlimeAudioPlayer>(); } }

    public UnityEvent OnLeaveGround;
    public UnityEvent OnTouchGround;
    public UnityEvent<int> OnResize;
    public UnityEvent OnBeingDestroy;
    public bool isGrounded { get; private set; }
    public RaycastHit downRaycastHit { get; private set; }

    // The list update every physics frame, clear every late update.
    private List<Collision> CollisionList = new List<Collision>();
    public bool isEatable { get; set; }
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
            m_Size = value;
            if (m_Size < SizeMin) m_Size = SizeMin;
            if (m_Size > SizeMax) m_Size = SizeMax;
            ReloadSize();
            OnResize.Invoke(m_Size);
        }
    }

    private void Awake()
    {
        CollisionList = new List<Collision>();
    }

    void Start()
    {
        size = SizeMax;
        m_IsGroundedPrevious = true;

        OnLeaveGround.AddListener(LeaveGroundAnime);
        OnTouchGround.AddListener(TouchGroundAnime);
        OnTouchGround.AddListener(TouchGroundAudio);
    }

    private void ReloadSize()
    {
        float sqrt3 = Mathf.Pow(m_Size, 1f / 3f);
        transform.localScale = sqrt3 * UnitSizeScale;
    }

    public void EatAnime()
    {
        slimeAudioPlayer.Eat();
        slimeAnimator.SetTrigger("Eat");
    }
    private void LeaveGroundAnime()
    {
        slimeAnimator.ResetTrigger("Grounded");
        slimeAnimator.SetTrigger("Jump");
    }
    private void TouchGroundAudio()
    {
        slimeAudioPlayer.Grounded();
    }
    private void TouchGroundAnime()
    {
        slimeAnimator.ResetTrigger("Jump");
        slimeAnimator.SetTrigger("Grounded");
    }

    private void FixedUpdate()
    {
        IsGroundedUpdate();
        GroundingEvents();
        CollisionList.Clear();
    }
    public void LateUpdate()
    {
        // Raycast for ground test
        float r = sphereCollider.radius;
        int collideMask = LayerMask.GetMask("Static", "Rigid", "Water", "StaticNoCamCollide");

        // Make a down raycast to environment.
        bool isRayHit = Physics.Raycast(transform.position + r * Vector3.up, -Vector3.up, out RaycastHit hit, r + 0.1f, collideMask);
        if (isRayHit)
        {
            // Set position if ray hit.
            // This may fix problem of going through collider unexpectedly.
            downRaycastHit = hit;
            transform.position = hit.point;
        }

    }

    // Update the variable isGrounded
    private void IsGroundedUpdate()
    {
        isGrounded = false;

        foreach (Collision collision in CollisionList)
        {
            if (collision.contactCount == 0) continue;
            Vector3 normal = collision.GetContact(0).normal;
            if (normal.y < -0.4f)
            {
                isGrounded = true;
                break;
            }
        }
        if (slimeAnimator.GetCurrentAnimatorStateInfo(1).IsName("SlimeJump"))
        {
            TouchGroundAnime();
        }
    }

    public void EatDroplet(DropletLocal another)
    {
        size += another.Size;
        EatAnime();
    }

    private void OnDestroy()
    {
        OnBeingDestroy.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionList.Add(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CollisionList.Add(collision);
    }

    private void GroundingEvents()
    {
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
}
