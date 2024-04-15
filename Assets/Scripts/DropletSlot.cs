using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropletSlot : MonoBehaviour
{
    public bool countPlayer = false;
    public UnityEvent<int> OnDropletSize;
    public UnityEvent<DropletNetwork> OnDropletEnter;
    public UnityEvent<DropletNetwork> OnDropletExit;

    public int dropletSize { get; private set; }
    public DropletNetwork currentDroplet { get; private set; }

    void Start()
    {
        dropletSize = 0;
        currentDroplet = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        var droplet = other.gameObject.GetComponent<DropletNetwork>();
        if (droplet != null)
        {
            if (countPlayer || droplet.GetComponent<Player>() == null)
            {
                if (currentDroplet == null)
                {
                    currentDroplet = droplet;
                    currentDroplet.OnResize.AddListener(OnDropletSize.Invoke);
                    OnDropletEnter.Invoke(droplet);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var droplet = other.gameObject.GetComponent<DropletNetwork>();
        if (droplet != null)
        {
            if (ReferenceEquals(currentDroplet, droplet))
            {
                currentDroplet = null;
                OnDropletExit.Invoke(droplet);
            }
        }
    }

    public void DiscardDroplet()
    {
        currentDroplet.Runner.Despawn(currentDroplet.GetComponent<NetworkObject>());
        currentDroplet = null;
    }
}
