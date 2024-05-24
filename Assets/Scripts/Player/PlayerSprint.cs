using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerSprint : MonoBehaviour
{
    [SerializeField]
    private InputAction m_Sprint;
    [SerializeField]
    private float m_NextSprintDelay = 0.1f;
    public float SprintForce = 10f;
    public float SprintTime = 0.2f;
    public Player player { get { return GetComponent<Player>(); } }
    public UnityEvent OnSprintBegin;

    private bool m_SprintTrigger = false;
    bool m_InSprintDelay = false;

    private void OnEnable()
    {
        m_Sprint.Enable();
    }

    private void OnDisable()
    {
        m_Sprint.Disable();
    }

    private void FixedUpdate()
    {
        if (!ReferenceEquals(player, Player.main)) return;

        if (m_Sprint.triggered && !m_InSprintDelay && player.droplet.size > 1)
        {
            m_SprintTrigger = true;
        }

        // Apply sprint force
        if (m_SprintTrigger)
        {
            // Start Sprint
            OnSprintBegin.Invoke();
            m_SprintTrigger = false;
            player.slimeAudioPlayer.Sprint();
            StartCoroutine(SprintRoutine());

            // Apply delay since idk why the new input system
            //  sometimes trigger twice with only one press.
            StartCoroutine(NextSprintDelay());
        }
    }

    IEnumerator SprintRoutine()
    {
        // Add force for SprintTime and
        //  totally add force SprintForce
        float timeNow = 0f;
        while (timeNow < SprintTime)
        {
            timeNow += Time.fixedDeltaTime;
            float force = SprintForce * Time.fixedDeltaTime / SprintTime;
            Vector3 dir = transform.forward.normalized;
            player.rigidBody.AddForce(force * dir, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator NextSprintDelay()
    {
        m_InSprintDelay = true;
        yield return new WaitForSeconds(m_NextSprintDelay);
        m_InSprintDelay = false;
    }
}
