using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerDrag : MonoBehaviour
{
    public float HorizontalDrag = 5f;
    public float VerticalDrag = 0f;
    public Player player { get { return GetComponent<Player>(); } }

    private void FixedUpdate()
    {
        if (!ReferenceEquals(player, Player.main)) return;

        Vector3 velocityChange = player.rigidBody.velocity;
        velocityChange.x *= HorizontalDrag;
        velocityChange.y *= VerticalDrag;
        velocityChange.z *= HorizontalDrag;

        player.rigidBody.AddForce(-velocityChange * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}
