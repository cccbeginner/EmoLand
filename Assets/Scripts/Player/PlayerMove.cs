using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : NetworkBehaviour
{

    [SerializeField]
    private InputAction m_Move;

    [Networked]
    Vector2 nt_MoveVecInput { get; set; }

    public float GroundMoveForce = 4f;
    public float AirMoveForce = 4f;
    public float RorateSpeed = 90f;
    public Player player { get { return GetComponent<Player>(); } }

    private Vector3 m_PrevImpact = Vector3.zero;
    private void OnEnable()
    {
        m_Move.Enable();
    }

    private void OnDisable()
    {
        m_Move.Disable();
    }

    private Vector3 NT_GetMoveForce()
    {
        // Get move vector in camera space.
        nt_MoveVecInput = m_Move.ReadValue<Vector2>();
        Vector3 moveVec = Vector3.zero;

        if (nt_MoveVecInput != Vector2.zero)
        {
            // Convert camera space to world.
            Quaternion cameraRotationY = Quaternion.Euler(0, player.PlayerCamera.transform.rotation.eulerAngles.y, 0);
            Vector3 vecMoveWorld = cameraRotationY * new Vector3(nt_MoveVecInput.x, 0, nt_MoveVecInput.y);

            // Rotate Character gradually.
            Quaternion q1 = Quaternion.LookRotation(gameObject.transform.forward);
            Quaternion q2 = Quaternion.LookRotation(vecMoveWorld);
            Vector3 vecForward = Quaternion.Lerp(q1, q2, RorateSpeed * Runner.DeltaTime) * Vector3.forward;

            if (player.IsGrounded)
            {
                moveVec = vecForward.normalized * GroundMoveForce;
            }
            else
            {
                moveVec = vecForward.normalized * AirMoveForce;
            }
        }
        return moveVec;
    }

    public override void FixedUpdateNetwork()
    {
        Vector3 moveVec = NT_GetMoveForce();
        if (moveVec != Vector3.zero)
        {
            gameObject.transform.forward = moveVec.normalized;
        }
        player.AddConstantImpact(-m_PrevImpact);
        player.AddConstantImpact(moveVec);
        m_PrevImpact = moveVec;
    }

    public override void Render()
    {

        if (nt_MoveVecInput != Vector2.zero)
        {
            player.SlimeAnimator.SetBool("Move", true);
        }
        else
        {
            player.SlimeAnimator.SetBool("Move", false);
        }
    }
}
