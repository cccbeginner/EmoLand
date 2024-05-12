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

    public List<DropletLocal> currentDroplets { get; private set; }
    public int currentSize = 0;

    void Start()
    {
        currentSize = 0;
        currentDroplets = new List<DropletLocal>();
        OnDropletInRange.AddListener(MissionComplete);
    }

    private void OnTriggerEnter(Collider other)
    {
        var droplet = other.gameObject.GetComponent<DropletLocal>();
        if (droplet != null)
        {
            if (countPlayer || droplet.GetComponent<Player>() == null)
            {
                currentDroplets.Add(droplet);
                droplet.OnResize.AddListener(CheckAndInvokeOnSize);
                droplet.OnEaten.AddListener((size) => {
                    OnDropletEaten(droplet);
                });
                CheckAndInvokeOnSize(droplet.Size);
            }
        }
    }

    private void CalcCurrentSize()
    {
        currentSize = 0;
        foreach (var droplet in currentDroplets) {
            currentSize += droplet.Size;
        }
    }

    private void OnDropletEaten(DropletLocal droplet)
    {
        currentDroplets.Remove(droplet);
        CalcCurrentSize();
    }

    private void CheckAndInvokeOnSize(int size)
    {
        CalcCurrentSize();
        if (MinSizeToInvoke <= currentSize && currentSize <= MaxSizeToInvoke)
        {
            OnDropletInRange.Invoke();
        }
    }

    private void MissionComplete()
    {
        foreach (var droplet in currentDroplets)
        {
            Destroy(droplet.gameObject);
        }
        gameObject.SetActive(false);
    }
}
