using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerSprint : MonoBehaviour
{
    [SerializeField]
    private InputAction m_Sprint;
    public float SprintForce = 10f;
    public float SprintTime = 0.2f;
    public Player player { get { return GetComponent<Player>(); } }
    public UnityEvent OnSprintBegin;

    private bool m_SprintTrigger = false;

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

        if (m_Sprint.triggered && player.droplet.size > 1)
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
}
