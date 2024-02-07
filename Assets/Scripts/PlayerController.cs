using Fusion;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private Vector3 m_Velocity;
    private bool m_JumpPressed;
    private Camera m_Camera;

    private CharacterController m_Controller;
    private Animator m_SlimeAnimator;
    private SceneObject m_SceneObject;

    [SerializeField]
    private InputAction m_Move, m_Jump;

    public float MoveSpeed = 2f;
    public float RorateSpeed = 18f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    // Variables for animation.
    [Networked]
    Vector2 nt_vecMove { get; set; }
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

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            m_Camera = Camera.main;
            m_Camera.GetComponent<ThirdPersonCamera>().Target = transform;
            m_IsGroundedPrevious = true;
        }
        m_SceneObject.AddScenePlayer(this);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        m_SceneObject.RemoveScenePlayer(this);
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

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false)
        {
            return;
        }

        if (m_Controller.isGrounded)
        {
            m_Velocity = new Vector3(0, -1, 0);
        }

        // Get move vector in camera space.
        nt_vecMove = m_Move.ReadValue<Vector2>();
        Vector3 move = Vector3.zero;

        if (nt_vecMove !=  Vector2.zero)
        {
            // Convert camera space to world.
            Quaternion cameraRotationY = Quaternion.Euler(0, m_Camera.transform.rotation.eulerAngles.y, 0);
            Vector3 vecMoveWorld = cameraRotationY * new Vector3(nt_vecMove.x, 0, nt_vecMove.y);

            // Rotate Character gradually.
            Quaternion q1 = Quaternion.LookRotation(gameObject.transform.forward);
            Quaternion q2 = Quaternion.LookRotation(vecMoveWorld);
            Vector3 vecForward = Quaternion.Lerp(q1, q2, RorateSpeed * Time.deltaTime) * Vector3.forward;
            move = vecForward.normalized * Runner.DeltaTime * MoveSpeed;
            gameObject.transform.forward = vecForward.normalized;
        }

        // Calculate vertical speed.
        m_Velocity.y += GravityValue * Runner.DeltaTime;
        if (m_JumpPressed && m_Controller.isGrounded)
        {
            // Start Jump
            m_Velocity.y += JumpForce;
            nt_jumpCount ++;
        }

        // Move character & set forward.
        m_Controller.Move(move + m_Velocity * Runner.DeltaTime);

        // Already got jump if true, so reset _jumpPressed.
        m_JumpPressed = false;

        nt_isGroundedCurrent = m_Controller.isGrounded;
    }

    public override void Render()
    {
        base.Render();

        // Update Animation
        // DetectMove
        if (nt_vecMove != Vector2.zero)
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
}