using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] GameObject DropletLocalPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (ReferenceEquals(player, Player.main))
            {
                DropletNetwork dropletNetwork = player.GetComponent<DropletNetwork>();
                GameObject dropletLocal = Instantiate(DropletLocalPrefab);
                dropletLocal.GetComponent<DropletLocal>().Size = dropletNetwork.SizeMax;
                dropletLocal.transform.position = player.transform.position;
            }
        }
    }
}
