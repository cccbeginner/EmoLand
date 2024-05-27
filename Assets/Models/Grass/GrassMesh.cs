using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrassMesh : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnPlayerTouchTop, OnPlayerExit;

    bool hit = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (ReferenceEquals(Player.main?.gameObject, collision.gameObject))
        {
            // Check if player touches the up surface.
            if (collision.contactCount > 0 && collision.GetContact(0).normal.y < -0.5)
            {
                OnPlayerTouchTop.Invoke();
                hit = true;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (ReferenceEquals(Player.main?.gameObject, collision.gameObject))
        {
            if (hit)
            {
                hit = false;
                OnPlayerExit.Invoke();
            }
        }
    }
}
