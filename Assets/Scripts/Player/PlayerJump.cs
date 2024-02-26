using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerJump : NetworkBehaviour
{
    [SerializeField]
    private InputAction m_Jump;
    public float JumpForce = 10f;
    public Player player { get { return GetComponent<Player>(); } }
    public UnityEvent OnJumpBegin;

    float KeepJumpTrigger = 0f;
    private bool m_JumpPressed = false;

    [Networked]
    int nt_jumpCount { get; set; }

    int m_LastVisibleJump = 0;

    private void OnEnable()
    {
        m_Jump.Enable();
    }

    private void OnDisable()
    {
        m_Jump.Disable();
    }

    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        player.OnLeaveGround.AddListener(OnPlayerLeaveGround);
        player.OnTouchGround.AddListener(OnPlayerGrounded);
        nt_jumpCount = 0;
    }

    private void Update()
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

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        // Apply jump force
        if (m_JumpPressed && player.PlayerController.isGrounded)
        {
            // Start Jump
            player.AddImpact(JumpForce * Vector3.up);
            nt_jumpCount++;
        }
    }
    public override void Render()
    {
        // Detect Jump
        if (m_LastVisibleJump < nt_jumpCount)
        {
            player.SlimeAnimator.SetTrigger("Jump");
            player.SlimeAnimator.ResetTrigger("Grounded");
            OnJumpBegin.Invoke();
        }
        m_LastVisibleJump = nt_jumpCount;

        if (player.SlimeAnimator.GetCurrentAnimatorStateInfo(1).shortNameHash == Animator.StringToHash("SlimeJump") && player.IsGrounded == true)
        {
            // Reach here if there is a bug that makes animator stranded at Jump state
            // even when the character is on the ground.
            player.SlimeAnimator.SetTrigger("Grounded");
        }
    }

    private void OnPlayerLeaveGround()
    {
        m_JumpPressed = false;
        KeepJumpTrigger = 0f;
    }

    private void OnPlayerGrounded()
    {
        player.SlimeAnimator.SetTrigger("Grounded");
    }
}
