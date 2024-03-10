using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private InputAction m_Move;

    [Networked]
    Vector2 m_MoveVecInput { get; set; }

    public float MoveForce = 4f;
    public float RorateSpeed = 90f;
    public Player player { get { return GetComponent<Player>(); } }
    private void OnEnable()
    {
        m_Move.Enable();
    }

    private void OnDisable()
    {
        m_Move.Disable();
    }

    private Vector3 WorldMoveForce()
    {
        // Get move vector in camera space.
        m_MoveVecInput = m_Move.ReadValue<Vector2>();
        Vector3 moveVec = Vector3.zero;

        if (m_MoveVecInput != Vector2.zero)
        {
            // Convert camera space to world.
            Quaternion cameraRotationY = Quaternion.Euler(0, player.PlayerCamera.transform.rotation.eulerAngles.y, 0);
            Vector3 vecMoveWorld = cameraRotationY * new Vector3(m_MoveVecInput.x, 0, m_MoveVecInput.y);

            // Rotate Character gradually.
            Quaternion q1 = Quaternion.LookRotation(gameObject.transform.forward);
            Quaternion q2 = Quaternion.LookRotation(vecMoveWorld);
            Vector3 vecForward = Quaternion.Lerp(q1, q2, RorateSpeed * Time.deltaTime) * Vector3.forward;

            moveVec = vecForward.normalized * MoveForce;
        }
        return moveVec;
    }

    public void Update()
    {
        if (!ReferenceEquals(player, Player.main)) return;
        Vector3 moveVec = WorldMoveForce();
        if (moveVec != Vector3.zero)
        {
            gameObject.transform.forward = moveVec.normalized;
        }
        player.rigidBody.AddForce(moveVec);

        if (m_MoveVecInput != Vector2.zero)
        {
            player.slimeAnimator.SetBool("Move", true);
        }
        else
        {
            player.slimeAnimator.SetBool("Move", false);
        }
    }
}
