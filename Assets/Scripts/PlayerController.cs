using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;
    private Camera _camera;

    private CharacterController _controller;

    [SerializeField]
    private InputAction _move, _jump;

    public float MoveSpeed = 2f;
    public float RorateSpeed = 18f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _camera = Camera.main;
            _camera.GetComponent<ThirdPersonCamera>().Target = transform;
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
        }

        // Calculate vertical speed.
        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += JumpForce;
        }

        // Move character & set forward.
        _controller.Move(move + _velocity * Runner.DeltaTime);
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move.normalized;
        }

        // Already got jump if true, so reset _jumpPressed.
        _jumpPressed = false;
    }
}