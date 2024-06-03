using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// The script is attach to the "slot" with a collider in trigger mode.

public class MoundSlot : MonoBehaviour
{
    public int MinSizeToInvoke = 1;
    public int MaxSizeToInvoke = 100;
    public UnityEvent OnDropletInRange;

    public List<DropletLocal> currentDroplets { get; private set; }
    public int currentSize { get; private set; }
    public bool isPlayerIn { get; private set; }

    void Start()
    {
        currentSize = 0;
        isPlayerIn = false;
        currentDroplets = new List<DropletLocal>();
        OnDropletInRange.AddListener(MissionComplete);
    }

    private void OnTriggerEnter(Collider other)
    {
        var droplet = other.gameObject.GetComponent<DropletLocal>();
        if (droplet != null)
        {
            if (droplet.GetComponent<Player>() == null)
            {
                currentDroplets.Add(droplet);
                droplet.OnResize.AddListener(CheckAndInvokeOnSize);
                droplet.OnEaten.AddListener((size) => {
                    OnDropletEaten(droplet);
                });
                CheckAndInvokeOnSize(droplet.Size);
            }
        }

        var player = other.gameObject.GetComponent<Player>();
        if (player != null && ReferenceEquals(player, Player.main))
        {
            isPlayerIn = true;
            CheckAndInvokeOnSize(player.droplet.size);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var droplet = other.gameObject.GetComponent<DropletLocal>();
        if (droplet != null)
        {
            if (droplet.GetComponent<Player>() == null)
            {
                currentDroplets.Remove(droplet);
                CheckAndInvokeOnSize(droplet.Size);
            }
        }

        var player = other.gameObject.GetComponent<Player>();
        if (player != null && ReferenceEquals(player, Player.main))
        {
            isPlayerIn = false;
            CheckAndInvokeOnSize(player.droplet.size);
        }
    }

    private void CalcCurrentSize()
    {
        currentSize = 0;
        foreach (var droplet in currentDroplets) {
            currentSize += droplet.Size;
        }
        if (isPlayerIn)
        {
            currentSize += Player.main.droplet.size - 1;
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
        if (isPlayerIn)
        {
            Player.main.droplet.size = 1;
            Player.main.droplet.EatAnime();
        }
        gameObject.SetActive(false);
    }
}
