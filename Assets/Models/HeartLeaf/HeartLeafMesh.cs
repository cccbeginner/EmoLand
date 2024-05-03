using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeartLeafMesh : MonoBehaviour
{
    public UnityEvent OnPlayerStep;
    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null && ReferenceEquals(player, Player.main))
        {
            if (collision.GetContact(0).normal.y < -0.5)
            {
                OnPlayerStep.Invoke();
            }
        }
    }
}
