using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomHead : MonoBehaviour
{
    [SerializeField]
    AudioSource m_MounceAudio;
    public Animator MushroomAnimator;
    public float force = 10;
    private bool hit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (ReferenceEquals(Player.main?.gameObject, collision.gameObject))
        {
            // Check if player touches the up surface.
            if (Player.main.transform.position.y > transform.position.y + 0.3f)
            {
                hit = true;
            }
            else
            {
                hit = false;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (ReferenceEquals(Player.main?.gameObject, collision.gameObject))
        {
            hit = false;
        }
    }

    private void Start()
    {
        StartCoroutine(WaitPlayerJoin());
    }

    IEnumerator WaitPlayerJoin()
    {
        while (true)
        {
            if (Player.main == null)
            {
                yield return null;
            }
            else
            {
                break;
            }
        }
        InitPlayerEvent();
    }

    private void InitPlayerEvent()
    {
        Player.main.playerJump.OnJumpBegin.AddListener(PlayerJumpBegin);
    }

    private void PlayerJumpBegin()
    {
        if (hit)
        {
            Player.main.rigidBody.AddForce(Vector3.up * force, ForceMode.Impulse);
            MushroomAnimator?.SetTrigger("Bounce");
            m_MounceAudio.Play();
        }
    }
}
