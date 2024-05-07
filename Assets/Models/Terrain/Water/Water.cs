using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] GameObject DropletLocalPrefab;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject != null)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            DropletNetwork dropletNetwork = collision.gameObject.GetComponent<DropletNetwork>();
            if (ReferenceEquals(player, Player.main))
            {
                if (dropletNetwork.size < dropletNetwork.SizeMax)
                {
                    GameObject dropletLocal = Instantiate(DropletLocalPrefab);
                    dropletLocal.GetComponent<DropletLocal>().Size = dropletNetwork.SizeMax;
                    dropletLocal.transform.position = player.transform.position;
                }
            }
            else if (player == null && dropletNetwork != null)
            {
                dropletNetwork.Runner.Despawn(dropletNetwork.GetComponent<NetworkObject>());
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        DropletNetwork dropletNetwork = collision.gameObject.GetComponent<DropletNetwork>();
        if (ReferenceEquals(player, Player.main))
        {
            if (dropletNetwork.size < dropletNetwork.SizeMax)
            {
                GameObject dropletLocal = Instantiate(DropletLocalPrefab);
                dropletLocal.GetComponent<DropletLocal>().Size = dropletNetwork.SizeMax;
                dropletLocal.transform.position = player.transform.position;
            }
        }
        else if (player == null && dropletNetwork != null)
        {
            dropletNetwork.Runner.Despawn(dropletNetwork.GetComponent<NetworkObject>());
        }
    }
}
