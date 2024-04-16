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

    private bool ForwardSlopeNormal(Vector3 forward, out Vector3 slopeNormal)
    {
        // return trap if there is, else return any slope, 
        bool found = false;
        slopeNormal = -forward;
        foreach(Collision collision in player.CollisionList)
        {
            if (collision.contactCount == 0) continue;
            Vector3 normal = collision.GetContact(0).normal;
            if (normal.y < -0.4f)
            {
                if(Vector3.Angle(forward, normal) < Vector3.Angle(forward, slopeNormal))
                {
                    found = true;
                    slopeNormal = normal;
                }
            }
        }
        slopeNormal = -slopeNormal;
        return found;
    }
    private Vector3 WorldMoveForce()
    {
        // Get move vector in camera space.
        m_MoveVecInput = m_Move.ReadValue<Vector2>();
        Vector3 moveVec = Vector3.zero;

        if (m_MoveVecInput != Vector2.zero)
        {
            // Convert camera space to world.
            Quaternion cameraRotationY = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            Vector3 vecMoveWorld = cameraRotationY * new Vector3(m_MoveVecInput.x, 0, m_MoveVecInput.y);

            // Rotate Character gradually.
            Quaternion q1 = Quaternion.LookRotation(gameObject.transform.forward);
            Quaternion q2 = Quaternion.LookRotation(vecMoveWorld);
            Vector3 vecForward = Quaternion.Lerp(q1, q2, RorateSpeed * Time.fixedDeltaTime) * Vector3.forward;

            // Calculate direction aligned with ground normal
            if (ForwardSlopeNormal(vecForward, out Vector3 normal))
            {
                Vector3 forwardUpPlaneAxis = Vector3.Cross(vecForward, Vector3.up);
                Vector3 fixedNormal = Vector3.ProjectOnPlane(normal, forwardUpPlaneAxis).normalized;
                float angle = Vector3.Angle(Vector3.up, fixedNormal);
                Quaternion rotation = Quaternion.AngleAxis(angle, forwardUpPlaneAxis);
                vecForward = rotation * vecForward;
            }
            moveVec = vecForward.normalized * MoveForce;
        }
        return moveVec;
    }

    public void FixedUpdate()
    {
        if (!ReferenceEquals(player, Player.main)) return;

        // Apply physics base movement
        Vector3 moveVec = WorldMoveForce();
        if (moveVec != Vector3.zero)
        {
            gameObject.transform.forward = moveVec.normalized;
        }
        player.rigidBody.AddForce(moveVec);
    }

    private void Update()
    {
        // Mov(ing) animation
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
