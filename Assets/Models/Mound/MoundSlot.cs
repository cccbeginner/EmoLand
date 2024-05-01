using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// The script is attach to the "slot" with a collider in trigger mode.
// Be sure there is only one droplet in slot at the same time, or the script could work unexpectedly.

public class MoundSlot : MonoBehaviour
{
    public bool countPlayer = false;
    public int MinSizeToInvoke = 1;
    public int MaxSizeToInvoke = 100;
    public UnityEvent OnDropletInRange;

    public DropletNetwork currentDroplet { get; private set; }

    void Start()
    {
        currentDroplet = null;
        OnDropletInRange.AddListener(MissionComplete);
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
                    currentDroplet.OnResize.AddListener(CheckAndInvokeOnSize);
                    CheckAndInvokeOnSize(currentDroplet.size);
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
                currentDroplet.OnResize.RemoveListener(CheckAndInvokeOnSize);
                currentDroplet = null;
            }
        }
    }

    private void CheckAndInvokeOnSize(int size)
    {
        if (MinSizeToInvoke <= size && size <= MaxSizeToInvoke)
        {
            OnDropletInRange.Invoke();
        }
    }

    public void DiscardDroplet()
    {
        currentDroplet.Runner.Despawn(currentDroplet.GetComponent<NetworkObject>());
        currentDroplet = null;
    }

    private void MissionComplete()
    {
        Destroy(currentDroplet.gameObject);
        gameObject.SetActive(false);
    }
}
