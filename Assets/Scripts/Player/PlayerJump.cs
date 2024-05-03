using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField]
    private InputAction m_Jump;
    [SerializeField]
    private float m_NextJumpDelay = 0.3f;
    public float JumpForce = 10f;
    public Player player { get { return GetComponent<Player>(); } }
    public UnityEvent OnJumpBegin;
    private bool m_JumpPressed = false;

    bool m_InJumpDelay = false;

    private void OnEnable()
    {
        m_Jump.Enable();
    }

    private void OnDisable()
    {
        m_Jump.Disable();
    }

    private void FixedUpdate()
    {
        if (!ReferenceEquals(player, Player.main)) return;

        if (m_Jump.triggered && !m_InJumpDelay && (player.droplet.size > 1 || player.droplet.isGrounded))
        {
            m_JumpPressed = true;
        }

        // Apply jump force
        if (m_JumpPressed /*&& player.droplet.isGrounded*/)
        {
            // Start Jump
            Vector3 curV = player.rigidBody.velocity;
            player.rigidBody.velocity = new Vector3(curV.x, 0, curV.z);
            player.rigidBody.AddForce(JumpForce * Vector3.up, ForceMode.Impulse);
            OnJumpBegin.Invoke();
            m_JumpPressed = false;
            StartCoroutine(NextJumpDelay());
        }
    }

    IEnumerator NextJumpDelay()
    {
        m_InJumpDelay = true;
        yield return new WaitForSeconds(m_NextJumpDelay);
        m_InJumpDelay = false;
    }
}
