using Fusion;
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
            DropletLocal dropletLocal = collision.gameObject.GetComponent<DropletLocal>();
            if (ReferenceEquals(player, Player.main))
            {
                if (dropletNetwork.size < dropletNetwork.SizeMax)
                {
                    GameObject droplet = Instantiate(DropletLocalPrefab);
                    droplet.GetComponent<DropletLocal>().Size = dropletNetwork.SizeMax;
                    droplet.transform.position = player.transform.position;
                    droplet.GetComponent<DropletLocal>().isEatable = true;
                }
            }
            else if (player == null && dropletLocal != null)
            {
                Destroy(dropletLocal.gameObject);
            }
        }
    }
}
