using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{ 
    public static Player main { get; private set; }

    // Add events
    public UnityEvent OnJumpBegin;
    public UnityEvent OnLeaveGround;
    public UnityEvent OnTouchGround;
    public UnityEvent<int> OnResize; 
    public UnityEvent<ControllerColliderHit> OnHitCollider;

    private Camera m_Camera;

    private CharacterController m_Controller;
    private Animator m_SlimeAnimator;

    [SerializeField]
    private InputAction m_Move, m_Jump;

    public float MoveForce = 2f;
    public float RorateSpeed = 18f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;
    public float Resistence = 0.1f;

    [SerializeField]
    private GameObject m_SlimeModel;


    // Variables for animation.
    [Networked]
    Vector2 nt_MoveVecInput { get; set; }
    [Networked]
    int nt_jumpCount { get; set; }
    [Networked]
    bool nt_isGroundedCurrent { get; set; }

    int m_LastVisibleJump = 0;
    bool m_IsGroundedPrevious = true;

    private void Awake()
    {
        m_Controller = GetComponent<CharacterController>();
        m_SlimeAnimator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        m_Move.Enable();
        m_Jump.Enable();
    }

    private void OnDisable()
    {
        m_Move.Disable();
        m_Jump.Disable();
    }

    float KeepJumpTrigger = 0f;
    void Update()
    {
        if (m_Jump.triggered)
        {
            KeepJumpTrigger = 0.3f;
        }
        if (KeepJumpTrigger > 0f)
        {
            m_JumpPressed = true;
            KeepJumpTrigger -= Time.deltaTime;
        }
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            m_Camera = Camera.main;
            m_Camera.GetComponent<ThirdPersonCamera>().Target = transform;
            m_IsGroundedPrevious = true;
            nt_SizeChanged = false;
            nt_jumpCount = 0;
            m_GravityMultiplier = 1f;
            nt_Size = 1;
            main = this;

            // Re-enable character controller to make it reset transform.
            m_Controller.enabled = false;
            m_Controller.enabled = true;

            m_PrevPos = m_Controller.transform.position;
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
    private bool m_JumpPressed;
    public float GravityMultiplier
    {
        get { return m_GravityMultiplier; }
        set { m_GravityMultiplier = value; }
    }
    private Vector3 NT_GetImpacts()
    {
        // Apply gravity.
        m_Impact += m_GravityMultiplier * GravityValue * Runner.DeltaTime * Vector3.up;

        // Apply jump force
        if (m_JumpPressed && m_Controller.isGrounded)
        {
            // Start Jump
            m_Impact += JumpForce * Vector3.up;
            nt_jumpCount++;
        }

        // Apply air resistence.
        float remainPortion = 1 - Resistence * Runner.DeltaTime;
        m_Impact *= remainPortion;

        return m_Impact + m_ConstantImpact * remainPortion;
    }

    private Vector3 NT_GetMoveForce()
    {
        // Get move vector in camera space.
        nt_MoveVecInput = m_Move.ReadValue<Vector2>();
        Vector3 moveVec = Vector3.zero;

        if (nt_MoveVecInput != Vector2.zero)
        {
            // Convert camera space to world.
            Quaternion cameraRotationY = Quaternion.Euler(0, m_Camera.transform.rotation.eulerAngles.y, 0);
            Vector3 vecMoveWorld = cameraRotationY * new Vector3(nt_MoveVecInput.x, 0, nt_MoveVecInput.y);

            // Rotate Character gradually.
            Quaternion q1 = Quaternion.LookRotation(gameObject.transform.forward);
            Quaternion q2 = Quaternion.LookRotation(vecMoveWorld);
            Vector3 vecForward = Quaternion.Lerp(q1, q2, RorateSpeed * Runner.DeltaTime) * Vector3.forward;
            moveVec = vecForward.normalized * MoveForce;
        }
        return moveVec;
    }

    private void NT_ClampCeilGround()
    {
        // Test if on the ground or touch ceiling.
        if ((m_Controller.collisionFlags & CollisionFlags.Below) != 0 && m_Impact.y < 0)
        {
            m_Impact = new Vector3(m_Impact.x, 0, m_Impact.z);
        }
        if ((m_Controller.collisionFlags & CollisionFlags.Above) != 0 && m_Impact.y > 0)
        {
            m_Impact = new Vector3(m_Impact.x, 0, m_Impact.z);
        }
    }

    // Variables for sizing slime.
    Vector3 UNIT_SIZE_SCALE = new Vector3(1, 1, 1);
    float UNIT_SIZE_RADIUS = 0.25f;
    [Networked]
    private bool nt_SizeChanged { get; set; }
    [Networked]
    private int nt_Size { get; set; }
    public int Size
    {
        get
        {
            return nt_Size;
        }
        set
        {
            if (value < 0) return;
            nt_Size = value;
            nt_SizeChanged = true;
            OnResize.Invoke(value);
        }
    }
    private void NT_ReloadSize()
    {
        float sqrt3 = Mathf.Pow(nt_Size, 1f / 3f);
        float r = sqrt3 * UNIT_SIZE_RADIUS;
        m_SlimeModel.transform.localScale = sqrt3 * UNIT_SIZE_SCALE;
        m_Controller.center = new Vector3(0, r, 0);
        m_Controller.radius = r;
        m_Controller.height = 2 * r;
        nt_SizeChanged = false;
    }


    public Vector3 Velocity { get { return m_Velocity; } }
    [Networked]
    private Vector3 m_Velocity { get; set; }
    [Networked]
    private Vector3 m_PrevPos { get; set; }
    private void NT_Expose()
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
        Vector3 moveVec = NT_GetMoveForce();
        if (moveVec != Vector3.zero)
        {
            gameObject.transform.forward = moveVec.normalized;
        }
        Vector3 otherVec = NT_GetImpacts();
        m_Controller.Move((otherVec + moveVec) * Runner.DeltaTime);

        // Change size if needed. Including visual and collider.
        if (nt_SizeChanged)
        {
            NT_ReloadSize();
        }

        // Expose velocity
        NT_Expose();

        // Reset variables at end.
        m_JumpPressed = false;
        nt_isGroundedCurrent = m_Controller.isGrounded;
    }

    public override void Render()
    {
        base.Render();

        // Update Animation
        // DetectMove
        if (nt_MoveVecInput != Vector2.zero)
        {
            m_SlimeAnimator.SetBool("Move", true);
        }
        else
        {
            m_SlimeAnimator.SetBool("Move", false);
        }

        // Detect Jump
        if (m_LastVisibleJump < nt_jumpCount)
        {
            m_SlimeAnimator.SetTrigger("Jump");
            m_SlimeAnimator.ResetTrigger("Grounded");
            OnJumpBegin.Invoke();
        }
        m_LastVisibleJump = nt_jumpCount;

        // Detect Grounded
        if (nt_isGroundedCurrent == false && m_IsGroundedPrevious == true)
        {
            OnLeaveGround.Invoke();
        }
        if (nt_isGroundedCurrent == true && m_IsGroundedPrevious == false)
        {
            m_SlimeAnimator.SetTrigger("Grounded");
            OnTouchGround.Invoke();
        } else if (m_SlimeAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("SlimeJump") && m_IsGroundedPrevious == true)
        {
            // Reach here if there is a bug that makes animator stranded at Jump state
            // even when the character is on the ground.
            m_SlimeAnimator.SetTrigger("Grounded");
        }
        m_IsGroundedPrevious = nt_isGroundedCurrent;

        // Expose Velocity
        
    }

    public void EatTrigger()
    {
        m_SlimeAnimator.SetTrigger("Eat");
    }

    public void AddForce(Vector3 force)
    {
        m_Impact += force / Size;
    }

    public void AddConstantForce(Vector3 force)
    {
        m_ConstantImpact += force / Size;
    }

    public void TriggerJump()
    {
        m_JumpPressed = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        OnHitCollider.Invoke(hit);
    }
}