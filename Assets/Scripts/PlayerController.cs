using Fusion;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;
    private Camera _camera;

    private CharacterController _controller;
    private Animator _slimeAnimator;
    private SceneObject _sceneObject;

    [SerializeField]
    private InputAction _move, _jump;

    public float MoveSpeed = 2f;
    public float RorateSpeed = 18f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    [Networked]
    Vector2 nt_vecMove { get; set; }
    [Networked]
    int nt_jumpCount { get; set; }
    [Networked]
    bool nt_isGroundedCurrent { get; set; }

    int _lastVisibleJump = 0;
    bool _isGroundedPrevious = true;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _slimeAnimator = GetComponentInChildren<Animator>();
        _sceneObject = GetComponent<SceneObject>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _camera = Camera.main;
            _camera.GetComponent<ThirdPersonCamera>().Target = transform;
            _isGroundedPrevious = true;
        }
        _sceneObject.AddScenePlayer(this);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _sceneObject.RemoveScenePlayer(this);
    }

    private void OnEnable()
    {
        _move.Enable();
        _jump.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _jump.Disable();
    }

    void Update()
    {
        if (_jump.triggered)
        {
            _jumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false)
        {
            return;
        }

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        // Get move vector in camera space.
        nt_vecMove = _move.ReadValue<Vector2>();
        Vector3 move = Vector3.zero;

        if (nt_vecMove !=  Vector2.zero)
        {
            // Convert camera space to world.
            Quaternion cameraRotationY = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0);
            Vector3 vecMoveWorld = cameraRotationY * new Vector3(nt_vecMove.x, 0, nt_vecMove.y);

            // Rotate Character gradually.
            Quaternion q1 = Quaternion.LookRotation(gameObject.transform.forward);
            Quaternion q2 = Quaternion.LookRotation(vecMoveWorld);
            Vector3 vecForward = Quaternion.Lerp(q1, q2, RorateSpeed * Time.deltaTime) * Vector3.forward;
            move = vecForward.normalized * Runner.DeltaTime * MoveSpeed;
            gameObject.transform.forward = vecForward.normalized;
        }

        // Calculate vertical speed.
        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            // Start Jump
            _velocity.y += JumpForce;
            nt_jumpCount ++;
        }

        // Move character & set forward.
        _controller.Move(move + _velocity * Runner.DeltaTime);

        // Already got jump if true, so reset _jumpPressed.
        _jumpPressed = false;

        nt_isGroundedCurrent = _controller.isGrounded;
    }

    public override void Render()
    {
        base.Render();

        // Update Animatio
        // DetectMove
        if (nt_vecMove != Vector2.zero)
        {
            _slimeAnimator.SetBool("Move", true);
        }
        else
        {
            _slimeAnimator.SetBool("Move", false);
        }

        // Detect Jump
        if (_lastVisibleJump < nt_jumpCount)
        {
            _slimeAnimator.SetTrigger("Jump");
            _slimeAnimator.ResetTrigger("Grounded");
        }
        _lastVisibleJump = nt_jumpCount;

        // Detect Grounded
        if (nt_isGroundedCurrent == true && _isGroundedPrevious == false)
        {
            _slimeAnimator.SetTrigger("Grounded");
        } else if (_slimeAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("SlimeJump") && _isGroundedPrevious == true)
        {
            // Reach here if there is a bug that makes animator stranded at Jump state
            // even when the character is on the ground.
            _slimeAnimator.SetTrigger("Grounded");
        }
        _isGroundedPrevious = nt_isGroundedCurrent;
    }
}