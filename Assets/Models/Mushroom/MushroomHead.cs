using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomHead : MonoBehaviour
{
    public Animator MushroomAnimator;
    public float force = 10;
    private bool detect = true;
    private bool hit = false;

    private void OnTriggerEnter(Collider collider)
    {
        if (Player.main?.gameObject == collider.gameObject)
        {
            // Check if player touches the up surface.
            if (detect && Player.main.transform.position.y > transform.position.y)
            {
                Player.main.AddForce(Vector3.up * force);
                detect = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        detect = true;
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
        Player.main.OnJumpBegin.AddListener(PlayerJumpBegin);
        Player.main.OnHitCollider.AddListener(PlayerHit);
        Player.main.OnLeaveGround.AddListener(PlayerLeaveGround);
    }

    private void PlayerJumpBegin()
    {
        if (hit)
        {
            Player.main.AddForce(Vector3.up * force);
            MushroomAnimator?.SetTrigger("Bounce");
            hit = false;
        }
    }

    private void PlayerHit(ControllerColliderHit colliderHit)
    {
        if (!ReferenceEquals(colliderHit.gameObject, gameObject)) return;

        // Check if player touches the up surface.
        if (detect && Player.main.GetComponent<CharacterController>().isGrounded)
        {
            detect = false;
            hit = true;
        }
    }

    private void PlayerLeaveGround()
    {
        detect = true;
        hit = false;
    }
}
