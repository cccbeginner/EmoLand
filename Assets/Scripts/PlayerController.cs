using Fusion;
using System.Collections;
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
    private NetworkMecanimAnimator _networkAnimator;

    [SerializeField]
    private InputAction _move, _jump;

    public float MoveSpeed = 2f;
    public float RorateSpeed = 18f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    [Networked]
    bool isGroundedPrevious { get; set; }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _slimeAnimator = GetComponentInChildren<Animator>();
        _networkAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _camera = Camera.main;
            _camera.GetComponent<ThirdPersonCamera>().Target = transform;
            isGroundedPrevious = true;
        }
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
        Vector2 vecMove = _move.ReadValue<Vector2>();
        Vector3 move = Vector3.zero;

        if (vecMove !=  Vector2.zero)
        {
            // Convert camera space to world.
            Quaternion cameraRotationY = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0);
            Vector3 vecMoveWorld = cameraRotationY * new Vector3(vecMove.x, 0, vecMove.y);

            // Rotate Character gradually.
            Quaternion q1 = Quaternion.LookRotation(gameObject.transform.forward);
            Quaternion q2 = Quaternion.LookRotation(vecMoveWorld);
            Vector3 vecForward = Quaternion.Lerp(q1, q2, RorateSpeed * Time.deltaTime) * Vector3.forward;
            move = vecForward.normalized * Runner.DeltaTime * MoveSpeed;
            gameObject.transform.forward = vecForward.normalized;

            // Start Move Animation
            _slimeAnimator.SetBool("Move", true);
        }
        else
        {
            // Stop Move Animation
            _slimeAnimator.SetBool("Move", false);
        }

        // Calculate vertical speed.
        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            // Start Jump
            _velocity.y += JumpForce;
            _slimeAnimator.SetTrigger("Jump");
            _slimeAnimator.ResetTrigger("Grounded");
        }

        // Move character & set forward.
        _controller.Move(move + _velocity * Runner.DeltaTime);

        // Already got jump if true, so reset _jumpPressed.
        _jumpPressed = false;

        // Test Grounded
        if (_controller.isGrounded && isGroundedPrevious == false)
        {
            _slimeAnimator.SetTrigger("Grounded");
        }
        isGroundedPrevious = _controller.isGrounded;
    }
}