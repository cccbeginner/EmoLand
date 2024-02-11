using Fusion;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Networked]
    private Vector3 m_Impact { get; set; }
    private bool m_JumpPressed;
    private Camera m_Camera;

    private CharacterController m_Controller;
    private Animator m_SlimeAnimator;
    private SceneObject m_SceneObject;

    [SerializeField]
    private InputAction m_Move, m_Jump;

    public float MoveForce = 2f;
    public float RorateSpeed = 18f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;
    public float AirResistence = 0.5f;

    [SerializeField]
    private GameObject m_SlimeModel;

    // Variables for sizing slime.
    Vector3 UNIT_SIZE_SCALE = new Vector3(1,1,1);
    float UNIT_SIZE_RADIUS = 0.25f;
    [Networked]
    private bool nt_SizeChanged { get; set; }
    [Networked]
    private float nt_Size { get; set; }
    public float Size
    {
        get
        {
            return nt_Size;
        }
        set
        {
            if (value <= 0 || nt_Size == value) return;
            nt_Size = value;
            nt_SizeChanged = true;
        }
    }

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
        m_SceneObject = GetComponent<SceneObject>();
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

    void Update()
    {
        if (m_Jump.triggered)
        {
            m_JumpPressed = true;
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
            nt_Size = 1;
        }
        m_SceneObject.AddScenePlayer(this);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        m_SceneObject.RemoveScenePlayer(this);
    }

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false)
        {
            return;
        }

        // Test if on the ground or touch ceiling.
        if (m_Controller.isGrounded || (m_Controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            m_Impact = new Vector3(m_Impact.x, 0, m_Impact.z);
        }

        // Get move vector in camera space.
        nt_MoveVecInput = m_Move.ReadValue<Vector2>();
        Vector3 moveVec = Vector3.zero;

        if (nt_MoveVecInput !=  Vector2.zero)
        {
            // Convert camera space to world.
            Quaternion cameraRotationY = Quaternion.Euler(0, m_Camera.transform.rotation.eulerAngles.y, 0);
            Vector3 vecMoveWorld = cameraRotationY * new Vector3(nt_MoveVecInput.x, 0, nt_MoveVecInput.y);

            // Rotate Character gradually.
            Quaternion q1 = Quaternion.LookRotation(gameObject.transform.forward);
            Quaternion q2 = Quaternion.LookRotation(vecMoveWorld);
            Vector3 vecForward = Quaternion.Lerp(q1, q2, RorateSpeed * Runner.DeltaTime) * Vector3.forward;
            moveVec = vecForward.normalized * Runner.DeltaTime * MoveForce;
            gameObject.transform.forward = vecForward.normalized;
        }

        // Calculate vertical speed.
        m_Impact += GravityValue * Runner.DeltaTime * Vector3.up;
        if (m_JumpPressed && m_Controller.isGrounded)
        {
            // Start Jump
            m_Impact += JumpForce * Vector3.up;
            nt_jumpCount ++;
        }

        // Apply air resistence.
        m_Impact *= Mathf.Pow(1 - AirResistence * Runner.DeltaTime, 2);

        // Change size if needed. Including visual and collider.
        if (nt_SizeChanged)
        {
            float sqrt3 = Mathf.Pow(nt_Size, 1f / 3f);
            float r = sqrt3 * UNIT_SIZE_RADIUS;
            m_SlimeModel.transform.localScale = sqrt3 * UNIT_SIZE_SCALE;
            m_Controller.center = new Vector3(0, r, 0);
            m_Controller.radius = r;
            m_Controller.height = 2 * r;
            nt_SizeChanged = false;
        }

        // Move character & set forward.
        m_Controller.Move(moveVec + m_Impact * Runner.DeltaTime);

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
        }
        m_LastVisibleJump = nt_jumpCount;

        // Detect Grounded
        if (nt_isGroundedCurrent == true && m_IsGroundedPrevious == false)
        {
            m_SlimeAnimator.SetTrigger("Grounded");
        } else if (m_SlimeAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("SlimeJump") && m_IsGroundedPrevious == true)
        {
            // Reach here if there is a bug that makes animator stranded at Jump state
            // even when the character is on the ground.
            m_SlimeAnimator.SetTrigger("Grounded");
        }
        m_IsGroundedPrevious = nt_isGroundedCurrent;
    }

    public void EatTrigger()
    {
        m_SlimeAnimator.SetTrigger("Eat");
    }

    public void AddForce(Vector3 force)
    {
        m_Impact += force / Size;
    }
}